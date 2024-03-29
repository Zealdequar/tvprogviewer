USE [TVProgCoreDb]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateXmlChansAndProgs]    Script Date: 16.04.2021 14:59:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Îáíîâëåíèå òåëåêàíàëîâ è òåëåïðîãðàììû	
ALTER procedure [dbo].[spUpdateXmlChansAndProgs](@WRID int, @ChanAndProg xml)
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
 /* SELECT @ChanAndProg = x.a
  FROM OPENROWSET(BULK 'C:\Programs\TVProgUpdaterV2\2021-03-28\TGXmlTvOld.xml', SINGLE_BLOB) AS x(a)*/
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
  Descr,
  Category into #TempProgrammes
  from OPENXML(@intPointer, '/tv/programme',2) 
  with (
	chanid int '@channel', 
	TsStart nvarchar(20) '@start', 
	TsStop nvarchar(20) '@stop', 
	Title nvarchar(300) 'title',
	Descr nvarchar(1000) 'desc', 
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
  
  UPDATE ch1 SET ch1.InternalId = impCh.id, ch1.IconWebSrc = impCh.icon
  FROM dbo.Channels ch1
  JOIN #TempChannels impCh ON ch1.TitleChannel = impCh.displayname 
  WHERE ch1.TVProgProviderID = @TVProgProviderID
  AND ch1.Deleted IS NULL

  -- Âñòàâêà òîëüêî íîâûõ êàíàëîâ
  insert into dbo.Channels (TVProgProviderID, InternalID, TitleChannel, IconWebSrc)
  select @TVProgProviderID, impCh.id, impCh.displayname, impCh.icon 
  from #TempChannels impCh
  WHERE impCh.id NOT IN (SELECT chans.InternalId 
                         FROM dbo.Channels chans
					     WHERE chans.Deleted IS NULL
					        AND TvProgProviderId = @TVProgProviderID)
  order by impCh.id, impCh.displayname; 
  -- Èçâëå÷åíèå çàêëþ÷èòåëüíîé ñòàòèñòèêè ïî êàíàëàì
  set @QtyNewChannels = @@ROWCOUNT;
    
  drop table #TempChannels;
  DROP INDEX IF EXISTS IX_Programmes_Unique ON Programmes;
  
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
	A.Descr,
	A.Category
	from (
  select @TID TID, 
  dbo.fnGetCIDByInternalID(@TVProgProviderID, impProg.chanid) CID, 
  impProg.chanid InternalChanID, 
  cast(impProg.TsStart as datetime2) TsStart,
  cast(coalesce(impProg.TsStop, coalesce(lead(impProg.TsStart) over (
                          partition by impProg.chanid order by impProg.TsStart)
						           , dateadd(hh, 1, impProg.TsStart))) as datetime2) TsStop,
  cast(SWITCHOFFSET(impProg.TsStart, '+03:00') as datetime2) TsStartMO,
  cast(SWITCHOFFSET(coalesce(impProg.TsStop, coalesce(lead(impProg.TsStart) over (
                          partition by impProg.chanid order by impProg.TsStart)
						           , dateadd(hh, 1, impProg.TsStart))), '+03:00') as datetime2) TsStopMO,
  impProg.Title,
  impProg.Descr,
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

  DELETE FROM Programmes
  WHERE Id IN (SELECT Id FROM
  (
     SELECT Id, TypeProgId, ChannelId, InternalChanId, TsStartMo, TsStopMo
	 , row_number() over (partition by TypeProgId, InternalChanId, ChannelId, TsStartMo, TsStopMo order by Id Desc) dr
     FROM Programmes
  )A
  WHERE dr > 1)
  
  /****** Object:  Index [IX_Programme_Unique]    Script Date: 14.04.2021 20:41:16 ******/
  CREATE UNIQUE NONCLUSTERED INDEX [IX_Programmes_Unique] ON [dbo].[Programmes]
  (
	[TypeProgId] ASC,
	[ChannelId] ASC,
	[InternalChanId] ASC,
	[TsStartMo] ASC,
	[TsStopMo] ASC
  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
  
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
  
