-- Ôóíêöèÿ ïîëó÷åíèÿ ChannelID ïî InternalID
CREATE function [dbo].[fnGetCIDByInternalID](@TVProgProviderID int,@InternalID int)
returns int
as
begin 
	declare @ChannelID int;
	set @ChannelID = -1;
	select @ChannelID = chans.ID 
	from dbo.Channels chans
	where TVProgProviderID = @TVProgProviderID and InternalID = @InternalID;
	return @ChannelID;
end;
GO

-- Ôóíêöèÿ ïîëó÷åíèÿ çîíèðîâàííîãî âðåìåíè èç ñòðîêè
CREATE function [dbo].[fnStrToDateTimeOffset](@dateStr nvarchar(20))
returns datetimeoffset
as
begin 
  if len(@dateStr) = 14 set @dateStr = @dateStr + ' +0300';
  return CONVERT(datetimeoffset, STUFF(STUFF(STUFF(STUFF(@dateStr, 9, 0, ' '),12,0,':'), 15, 0, ':'), 22, 0, ':'), 112);
end
GO

CREATE procedure [dbo].[spDelTwoWeekAgo]
as 
begin
   delete from dbo.Programmes
   where TsStartMO >= dateadd(week, 2, getdate());
   
   delete from dbo.Programmes
   where TsStartMO < 
   (select top 1 dateMin3weeks from
   (select dateadd(day, N.num, dateadd(week, -3, MaxTsStartMO))  dateMin3weeks
   from (
         select max(TsStartMO) MaxTsStartMO 
         from dbo.Programmes
		   ) pr,
   (select num from (values (0), (1), (2), (3),(4), (5),(6)) as N(num)) N)A
   where datepart(dw, dateMin3weeks) = datepart(dw, '1900-01-01'))
 
   ALTER DATABASE [TVProgDb] SET RECOVERY SIMPLE
   
   DBCC SHRINKFILE (TVProgDb_log, 1); 
   ALTER DATABASE [TVProgDb] SET RECOVERY FULL
end
GO

CREATE procedure [dbo].[spUdtChannelImageByID](@CID int, @IconID bigint)
as
declare @ErrCode int;
begin
	/* ErrCode = 76 - ïðîèçîøëà îøèáêà îáíîâëåíèÿ ïèêòîãðàììû êàíàëà*/
	set @ErrCode = 0;
	begin try
		update dbo.Channels set IconID = @IconID
		where InternalID = @CID;
	end try
	begin catch 
		set @ErrCode = 76;
	end catch 
end
GO

CREATE procedure [dbo].[spUdtChannelImage](
	@CID int,
	@IconWebSrc nvarchar(550),
	@ChannelIconName nvarchar(256),
	@ContentType nvarchar(256),
	@ContentCoding nvarchar(256),
	@ChannelOrigIcon varbinary(7168),
	@IsSystem bit,
	@ErrCode int out
)
as
declare @cntIcon int;
declare @LengthIcon int;
declare @LengthIcon25 int;
declare @IconID bigint;
begin
	/* ErrCode = 75 - ïðîèçîøëà îøèáêà âñòàâêè/îáíîâëåíèÿ ïèêòîãðàììû
		ErrCode = 79 - ñëèøêîì áîëüøàÿ äëèíà ôàéëà */
	set @ErrCode = 0;	
	set @LengthIcon = LEN(@ChannelOrigIcon);
    if (@LengthIcon > 7168)
    begin
		set @ErrCode = 79;
		return; 
    end;		
	
	select @cntIcon = COUNT(*) 
	from dbo.MediaPic
	where FileName Like @ChannelIconName;
	begin try
	if (@cntIcon = 0)
	begin 
		insert into dbo.MediaPic (FileName, ContentType, ContentCoding, 
		 Length, Length25, IsSystem, PathOrig, Path25)
		values (@ChannelIconName, @ContentType, @ContentCoding, 
		@LengthIcon, @LengthIcon25, @IsSystem,'/imgs/system/large/', '/imgs/system/small/');
	    set @IconID = SCOPE_IDENTITY();
	    exec dbo.spUdtChannelImageByID @CID, @IconID;
	end
	else 
	begin
	    update dbo.MediaPic set ContentType = @ContentType, 
			ContentCoding = @ContentCoding, 
			
			Length = @LengthIcon,
			Length25 = @LengthIcon25,
			IsSystem = @IsSystem,
			PathOrig = '/imgs/system/large/', 
			Path25 = '/imgs/system/small/'
		where FileName Like @ChannelIconName and Length != @LengthIcon		
	end	
	end try
	begin catch
		set @ErrCode = 75;
	end catch
end;
GO

CREATE procedure [dbo].[spUpdateXmlChansAndProgs](@WRID int, @ChanAndProg xml)
as 
declare 
@intPointer int,
@TsUpdateStart datetimeoffset,
@ElapsedTimeSec int,
@MinProgDate datetimeoffset,
@MaxProgDate datetimeoffset,
@QtyChannels int,
@QtyProgrammes int,
@QtyNewChannels int,
@QtyNewProgrammes int,
@TVProgProviderID int,
@TID int,
@IsSuccess bit,
@ErrMessage nvarchar(4000)
begin
  select @IsSuccess = 0, 
  @ErrMessage = '', 
  @MinProgDate = SYSDATETIMEOFFSET(),
  @MaxProgDate = SYSDATETIMEOFFSET(),
  @QtyChannels = 0,
  @QtyProgrammes = 0,
  @QtyNewChannels = 0,
  @QtyNewProgrammes = 0;
  -- Íà÷èíàåì...
  set @TsUpdateStart = SysDateTimeOffset();	
  begin try
  exec sp_xml_preparedocument @intPointer output, @ChanAndProg;
  
  -- Âûáîðêà êàíàëîâ
  select id, displayname, icon into #TempChannels  
  from OPENXML(@intPointer, '/tv/channel',2) 
  with (
	id int '@id', 
	displayname nvarchar(50) 'display-name', 
	icon nvarchar(100) 'icon/@src' 
		);
		
  -- Âûáîðêà ïðîãðàìì			
  select chanid, 
  dbo.fnStrToDateTimeOffset(TsStart) as TsStart,
  dbo.fnStrToDateTimeOffset(TsStop) as TsStop, 
  Title, 
  Descrip,
  Category into #TempProgrammes
  from OPENXML(@intPointer, '/tv/programme',2) 
  with (
	chanid int '@channel', 
	TsStart nvarchar(20) '@start', 
	TsStop nvarchar(20) '@stop', 
	Title nvarchar(300) 'title',
	Descrip nvarchar(1000) 'desc', 
	Category nvarchar(150) 'category'
		);
  exec sp_xml_removedocument @intPointer;
  -- Âû÷èñëåíèå ïðåäâàðèòåëüíîé ñòàòèñòèêè:
  select @QtyChannels = COUNT(*) from #TempChannels;
  select @MinProgDate = COALESCE(MIN(TsStart),SYSDATETIMEOFFSET()) from #TempProgrammes;
  select @MaxProgDate = COALESCE(MAX(TsStop),SYSDATETIMEOFFSET()) from #TempProgrammes;
  select @QtyProgrammes = COUNT(*) from #TempProgrammes;
  
  -- Ïîëó÷åíèå ïðîâàéäåðà òåëåïðîãðàììû è å¸ òèïà
  select @TVProgProviderID = tp.TVProgProviderID, @TID = tp.ID 
  from dbo.WebResources wr
  join dbo.TypeProg tp
  on wr.TypeProgId = tp.ID
  where wr.ID = @WRID;
  
  -- Âñòàâêà òîëüêî íîâûõ êàíàëîâ
  insert into dbo.Channels (TVProgProviderID, InternalID, TitleChannel, IconWebSrc)
  select @TVProgProviderID, impCh.id, impCh.displayname, impCh.icon 
  from #TempChannels impCh
  left join dbo.Channels chans
  on impCh.id = chans.InternalID and chans.TVProgProviderID = @TVProgProviderID
  where chans.ID IS NULL
  order by impCh.id, impCh.displayname; 
  -- Èçâëå÷åíèå çàêëþ÷èòåëüíîé ñòàòèñòèêè ïî êàíàëàì
  set @QtyNewChannels = @@ROWCOUNT;
    
  drop table #TempChannels;
  
  -- Âñòàâêà òîëüêî íîâûõ òåëåïðîãðàìì
  insert into dbo.Programmes (
	TypeProgId, 
	ChannelId, 
	InternalChanID, 
	TsStart, 
	TsStop, 
	TsStartMO, 
	TsStopMO,
	Title,
	Descr,
	Category)
	select A.TID,
	A.CID,
	A.InternalChanID,
	A.TsStart,
	A.TsStop,
	A.TsStartMO,
	A.TsStopMO,
	A.Title,
	A.Descrip,
	A.Category
	from (
  select @TID TID, 
  dbo.fnGetCIDByInternalID(@TVProgProviderID, impProg.chanid) CID, 
  impProg.chanid InternalChanID, 
  impProg.TsStart,
  coalesce(impProg.TsStop, coalesce(lead(impProg.TsStart) over (
                          partition by impProg.chanid order by impProg.TsStart)
						           , dateadd(hh, 1, impProg.TsStart))) TsStop,
  SWITCHOFFSET(impProg.TsStart, '+03:00') TsStartMO,
  SWITCHOFFSET(coalesce(impProg.TsStop, coalesce(lead(impProg.TsStart) over (
                          partition by impProg.chanid order by impProg.TsStart)
						           , dateadd(hh, 1, impProg.TsStart))), '+03:00') TsStopMO,
  impProg.Title,
  impProg.Descrip,
  impProg.Category
  from #TempProgrammes impProg) A
  left join dbo.Programmes prog
  on A.InternalChanID = prog.InternalChanID and A.TsStart = prog.TsStart 
                          and A.TsStop = prog.TsStop 
   and prog.TypeProgId = @TID
  where prog.ID IS NULL
  order by A.InternalChanID, A.TsStart;
  
  -- Èçâëå÷åíèå çàêëþ÷èòåëüíîé ñòàòèñòèêè ïî ïðîãðàììàì
  set @QtyNewProgrammes = @@ROWCOUNT;
    
  drop table #TempProgrammes;
  
  -- Çàêîí÷èëè...
  set @ElapsedTimeSec = DATEDIFF(ss, @TsUpdateStart, SysDateTimeOffset());
  set @IsSuccess = 1;
  end try
  begin catch
	declare @ErrorNo int,
	        @Severity tinyint,
	        @State smallint,
	        @LineNo int,
	        @Message nvarchar(4000)
	select  @ErrorNo = ERROR_NUMBER(),
	        @Severity = ERROR_SEVERITY(),
	        @State = ERROR_STATE(),
	        @LineNo = ERROR_LINE(),
	        @Message = ERROR_MESSAGE()       
	select 
	@ErrMessage = N'ErrorNo=' + CAST(@ErrorNo as nvarchar(100)) + ', Severity=' + 
	CAST(@Severity as nvarchar(100)) + 
	', State=' + CAST(@State as nvarchar(100)) + ', Line='+ CAST(@LineNo as nvarchar(100)) + 
	', Msg='''+ @Message + '''.',
	@IsSuccess = 0,
	@ElapsedTimeSec = DATEDIFF(ss, @TsUpdateStart, SysDateTimeOffset());
  end catch 
  
  -- ôèêñèðóåì â ëîã:
   insert dbo.UpdateProgLog(
	WebResourceId, 
	TsUpdateStart, 
	UdtElapsedSec, 
	MinProgDate, 
	MaxProgDate, 
	QtyChans, 
	QtyProgrammes,  
	QtyNewChans,
	QtyNewProgrammes,
	IsSuccess,
	ErrorMessage)
  values (
    @WRID, 
    @TsUpdateStart, 
    @ElapsedTimeSec, 
    @MinProgDate,
    @MaxProgDate,
    @QtyChannels,
    @QtyProgrammes,
    @QtyNewChannels,
    @QtyNewProgrammes,
    @IsSuccess,
    @ErrMessage);

	exec dbo.spDelTwoWeekAgo
end    
GO