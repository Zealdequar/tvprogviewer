use [TVProgBase]
go

if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnStrToDateTimeOffset'))
drop function fnStrToDateTimeOffset;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnGetCIDByInternalID'))
drop function fnGetCIDByInternalID;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUpdateXmlChansAndProgs'))
drop procedure spUpdateXmlChansAndProgs;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spGetWebResources'))
drop procedure spGetWebResources;
go

-- Функция получения зонированного времени из строки
create function fnStrToDateTimeOffset(@dateStr nvarchar(20))
returns datetimeoffset
as
begin 
  if len(@dateStr) = 14 set @dateStr = @dateStr + ' +0300';
  return CONVERT(datetimeoffset, STUFF(STUFF(STUFF(STUFF(@dateStr, 9, 0, ' '),12,0,':'), 15, 0, ':'), 22, 0, ':'), 112);
end
go

-- Функция получения ChannelID по InternalID
create function fnGetCIDByInternalID(@TVProgProviderID int,@InternalID int)
returns int
as
begin 
	declare @ChannelID int;
	set @ChannelID = -1;
	select @ChannelID = chans.ChannelID 
	from dbo.Channels chans
	where TVProgProviderID = @TVProgProviderID and InternalID = @InternalID;
	return @ChannelID;
end;
go

-- Обновление телеканалов и телепрограммы	
create procedure [dbo].[spUpdateXmlChansAndProgs](@WRID int, @ChanAndProg xml)
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
  -- Начинаем...
  set @TsUpdateStart = SysDateTimeOffset();	
  begin try
  exec sp_xml_preparedocument @intPointer output, @ChanAndProg;
  
  -- Выборка каналов
  select id, displayname, icon into #TempChannels  
  from OPENXML(@intPointer, '/tv/channel',2) 
  with (
	id int '@id', 
	displayname nvarchar(50) 'display-name', 
	icon nvarchar(100) 'icon/@src' 
		);
		
  -- Выборка программ			
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
  -- Вычисление предварительной статистики:
  select @QtyChannels = COUNT(*) from #TempChannels;
  select @MinProgDate = COALESCE(MIN(TsStart),SYSDATETIMEOFFSET()) from #TempProgrammes;
  select @MaxProgDate = COALESCE(MAX(TsStop),SYSDATETIMEOFFSET()) from #TempProgrammes;
  select @QtyProgrammes = COUNT(*) from #TempProgrammes;
  
  -- Получение провайдера телепрограммы и её типа
  select @TVProgProviderID = tp.TVProgProviderID, @TID = tp.TypeProgID 
  from dbo.WebResources wr
  join dbo.TypeProg tp
  on wr.TPID = tp.TypeProgID
  where wr.WebResourceID = @WRID;
  
  -- Вставка только новых каналов
  insert into dbo.Channels (TVProgProviderID, InternalID, TitleChannel, IconWebSrc)
  select @TVProgProviderID, impCh.id, impCh.displayname, impCh.icon 
  from #TempChannels impCh
  left join dbo.Channels chans
  on impCh.id = chans.InternalID and chans.TVProgProviderID = @TVProgProviderID
  where chans.ChannelID IS NULL
  order by impCh.id, impCh.displayname; 
  -- Извлечение заключительной статистики по каналам
  set @QtyNewChannels = @@ROWCOUNT;
    
  drop table #TempChannels;
  
  -- Вставка только новых телепрограмм
  insert into dbo.Programmes (
	TID, 
	CID, 
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
   and prog.TID = @TID
  where prog.ProgrammesID IS NULL
  order by A.InternalChanID, A.TsStart;
  
  -- Извлечение заключительной статистики по программам
  set @QtyNewProgrammes = @@ROWCOUNT;
    
  drop table #TempProgrammes;
  
  -- Закончили...
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
  
  -- фиксируем в лог:
  insert dbo.UpdateProgLog(
	WRID, 
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
end    
  
go