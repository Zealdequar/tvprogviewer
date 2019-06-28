use [TVProgBase]
go

if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spInsChannel'))
drop procedure spInsChannel;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUdtChannelImageByID'))
drop procedure spUdtChannelImageByID;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUdtChannelImage'))
drop procedure spUdtChannelImage;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spGetProgrammes'))
drop procedure spGetProgrammes;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spInsUserChannel'))
drop procedure spInsUserChannel;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spInsUserChannelExt'))
drop procedure spInsUserChannelExt;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUdtUserChannelInfo'))
drop procedure spUdtUserChannelInfo;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUdtUserChannelImageByID'))
drop procedure spUdtUserChannelImageByID;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUdtUserChannelImage'))
drop procedure spUdtUserChannelImage;
go

-- Вставка канала с картинкой
create procedure spInsChannel (
	@TVProgProviderID int,
	@InternalID int, 
	@TitleChannel nvarchar(300),
	@IconWebSrc nvarchar(550),
	@ChannelIconName nvarchar(256),
	@ContentType nvarchar(256),
	@ContentCoding nvarchar(256),
	@ChannelOrigIcon varbinary(7168),
	@ChannelIcon25 varbinary(7168),
	@IsSystem bit,
	@ErrCode int out)
as 
declare @cntChannels int;
declare @IconID bigint;
declare @LengthIcon int;
declare @Length25Icon int;
begin
	/* ErrCode = 73 - произошла ошибка вставки канала
	   ErrCode = 75 - произошла ошибка обновления пиктограммы канала
	   ErrCode = 79 - слишком большая длина файла  */
	set @ErrCode = 0;
	select @cntChannels = COUNT(*)
	from dbo.Channels
	where TVProgProviderID = @TVProgProviderID and TitleChannel = @TitleChannel;
	if (@cntChannels = 0)
	begin
	    begin try
			set @LengthIcon = LEN(@ChannelOrigIcon); 
			set @Length25Icon = len(@ChannelIcon25);
	        if (@LengthIcon > 7168 or @Length25Icon > 7168) 
	        begin 
				set @ErrCode = 79;
				return;
			end;
	        insert into dbo.MediaPic (FileName, ContentType, ContentCoding, Length, Length25, IsSystem)
			values (@ChannelIconName, @ContentType, @ContentCoding, @LengthIcon, @Length25Icon, @IsSystem);
	    end try
	    begin catch
			set @ErrCode = 75;
	    end catch
	    set @IconID = SCOPE_IDENTITY();
		begin try
		    insert into dbo.Channels (TVProgProviderID, InternalID, IconID, TitleChannel, IconWebSrc)
			values (@TVProgProviderID, @InternalID, @IconID, @TitleChannel, @IconWebSrc);
		end try
		begin catch
			set @ErrCode = 73;
		end catch
	end	
end
go

-- Обновление пиктограммы канала
create procedure spUdtChannelImageByID(@CID int, @IconID bigint)
as
declare @ErrCode int;
begin
	/* ErrCode = 76 - произошла ошибка обновления пиктограммы канала*/
	set @ErrCode = 0;
	begin try
		update dbo.Channels set IconID = @IconID
		where InternalID = @CID;
	end try
	begin catch 
		set @ErrCode = 76;
	end catch 
end
go

-- Обновление/вставка пиктограммы канала
create procedure [dbo].[spUdtChannelImage](
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
	/* ErrCode = 75 - произошла ошибка вставки/обновления пиктограммы
		ErrCode = 79 - слишком большая длина файла */
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
go

-- Получение списка системной программы
/* Mode = 1 - на сейчас, Mode = 2 - следом */
create procedure spGetProgrammes (@TID int, @TsDate datetimeoffset, @Mode int)
as
declare @ErrCode int;
declare @MinDate datetimeoffset = CAST('0001-01-01 00:00:00 +00:00' AS DATETIMEOFFSET);
declare @now datetimeoffset = sysdatetimeoffset(); 
begin 
    /* ErrCode 78 - Ошибка выборки системной программы телепередач за сейчас */
	set @ErrCode = 0;
	begin try
	if (@Mode = 1)  
	begin
		select pr1.ProgrammesID,
			   pr1.CID,
			   pr1.TitleChannel,
			   pr1.InternalChanID,
			   pr1.TsStart,
			   pr1.TsStartMO,
			   pr1.TsStop,
			   pr1.TsStopMO,
			   pr1.Title,
			   pr1.Descr,
			   pr1.AnonsContent,
			   pr1.Category,
			   pr1.Remain 
		from (
				select pr.ProgrammesID,
				   pr.CID, 
				   ch.TitleChannel,
				   pr.InternalChanID, 
				   pr.TsStart, 
				   pr.TsStop, 
			       pr.TsStartMO, 
			       pr.TsStopMO, 
			       pr.Title,
			       pr.Descr,
			       case when pr.Descr is not null and pr.Descr != '' then 
					(select top 1 ContentOrig from dbo.MediaPic where IconID = 35)
				   end as AnonsContent,  
				   pr.Category,
			       cast (ROUND(DATEDIFF(ss, @TsDate, pr.TsStop) *1.0 / DATEDIFF(ss, pr.TsStart, pr.TsStop)*1.0 * 100,0) as int) as Remain
				from dbo.Programmes pr
				join dbo.Channels ch
				on pr.CID = ch.ChannelID
				left join dbo.MediaPic mp
				on mp.IconID = ch.IconID
				where pr.TID = @TID and 
				pr.TsStart <= @TsDate and @TsDate < pr.TsStop
			   ) pr1
		where (pr1.Category != 'Для взрослых' or pr1.Category IS NULL) and not pr1.Title Like '%(18+)%'
		order by pr1.InternalChanID, pr1.TsStart
	end
	else if (@Mode = 2 and @TsDate = @MinDate)
	begin
		select pr1.ProgrammesID,
			   pr1.CID,
			   pr1.TitleChannel,
			   pr1.InternalChanID,
			   pr1.TsStart,
			   pr1.TsStartMO,
			   pr1.TsStop,
			   pr1.TsStopMO,
			   pr1.Title,
			   pr1.Descr,
			   pr1.AnonsContent,
			   pr1.Category,
			   pr1.Remain 
		from (
				select pr.ProgrammesID,
				   pr.CID, 
				   ch.TitleChannel,
				   pr.InternalChanID, 
				   pr.TsStart, 
				   pr.TsStop, 
			       pr.TsStartMO, 
			       pr.TsStopMO, 
			       pr.Title,
			       pr.Descr,
			       case when pr.Descr is not null and pr.Descr != '' then 
					(select top 1 ContentOrig from dbo.MediaPic where IconID = 35)
				   end as AnonsContent,  
				   pr.Category,
			       DATEDIFF(ss, @now, pr.TsStart)  as Remain
			     from dbo.Programmes pr
				 join dbo.Channels ch
				 on pr.CID = ch.ChannelID
				left join dbo.MediaPic mp
				on mp.IconID = ch.IconID
				 where pr.TID = @TID and 
						pr.TsStart <= (select top 1 DATEADD(ss,1,prin.TsStop)
			                              from dbo.Programmes prin
			                              where prin.TsStart <= @now and @now < prin.TsStop and
			                              prin.CID = pr.CID and prin.TID = pr.TID) and 
			                              (select top 1 DATEADD(ss,1,prin.TsStop)
			                              from dbo.Programmes prin
			                              where prin.TsStart <= @now and @now < prin.TsStop and
			                              prin.CID = pr.CID and prin.TID = pr.TID) < pr.TsStop
			 ) pr1
		where (pr1.Category != 'Для взрослых' or pr1.Category IS NULL) and not pr1.Title Like '%(18+)%'
		order by pr1.InternalChanID, pr1.TsStart
		end
		else if (@Mode = 2 and @TsDate > @MinDate)
		begin
		select pr1.ProgrammesID,
			   pr1.CID,
			   pr1.TitleChannel,
			   pr1.InternalChanID,
			   pr1.TsStart,
			   pr1.TsStartMO,
			   pr1.TsStop,
			   pr1.TsStopMO,
			   pr1.Title,
			   pr1.Descr,
			   pr1.AnonsContent,
			   pr1.Category,
			   pr1.Remain 
		from (
			select pr.ProgrammesID,
				   pr.CID, 
				   ch.TitleChannel,
				   pr.InternalChanID, 
				   pr.TsStart, 
				   pr.TsStop, 
			       pr.TsStartMO, 
			       pr.TsStopMO, 
			       pr.Title,
			       pr.Descr,
			       case when pr.Descr is not null and pr.Descr != '' then 
					(select top 1 ContentOrig from dbo.MediaPic where IconID = 35)
				   end as AnonsContent,  
				   pr.Category,
			       DATEDIFF(ss, @now, pr.TsStart)  as Remain
			from dbo.Programmes pr
			join dbo.Channels ch
			on pr.CID = ch.ChannelID
			left join dbo.MediaPic mp
			on mp.IconID = ch.IconID
			where pr.TID = @TID 
				and pr.TsStart > @now 
				and pr.TsStart <= @TsDate
			) pr1
		where (pr1.Category != 'Для взрослых' or pr1.Category IS NULL) and not pr1.Title Like '%(18+)%'
		order by pr1.InternalChanID, pr1.TsStart
	end
	end try
	begin catch
		set @ErrCode = 78;
	end catch
end
go

-- Вставка пользовательского канала с картинкой
create procedure spInsUserChannelExt (
	@UID bigint, 
	@CID int,
	@DisplayName nvarchar(300),
	@IconWebSrc nvarchar(550),
	@ChannelIconName nvarchar(256),
	@ContentType nvarchar(256),
	@ContentCoding nvarchar(256),
	@ChannelOrigIcon varbinary(7168),
	@ChannelIcon25 varbinary(7168),
	@ErrCode int out)
as 
declare @cntUserChannels int;
declare @IconID bigint;
declare @LengthOrigIcon int;
declare @LengthIcon25 int;
declare @orderNum int;
begin
	/* 
	   ErrCOde = 75 - ошибка вставки/обновления пиктограммы
	   ErrCode = 79 - слишком большая длина файла
	   ErrCode = 80 - произошла ошибка вставки пользовательского канала
	   */
	set @ErrCode = 0;
	select @orderNum = ISNULL(MAX(OrderCol), 0)
	from dbo.UserChannels 
	where UID = @UID;
	select @cntUserChannels = COUNT(*)
	from dbo.UserChannels
	where DisplayName = @DisplayName and UID = @UID and CID = @CID;
	if (@cntUserChannels = 0)
	begin
	    begin try
			set @LengthOrigIcon = LEN(@ChannelOrigIcon); 
			set @LengthIcon25 = len(@ChannelIcon25);
	        if (@LengthOrigIcon > 7168 or @LengthIcon25 > 7168) 
	        begin 
				set @ErrCode = 79;
				return;
			end;
	        insert into dbo.MediaPic (FileName, ContentType, ContentCoding, 
			ContentOrig, Length, Content25, Length25, IsSystem)
			values (@ChannelIconName, @ContentType, @ContentCoding, 
			@ChannelOrigIcon, @LengthOrigIcon, @ChannelIcon25, @LengthIcon25, 0);
	    end try
	    begin catch
			set @ErrCode = 75;
	    end catch
	    set @IconID = SCOPE_IDENTITY();
		begin try
		    insert into dbo.UserChannels (UID, CID, IconID, DisplayName, OrderCol)
			values (@UID, @CID, @IconID, @DisplayName, @orderNum + 1);
		end try
		begin catch
			set @ErrCode = 80;
		end catch
	end	
end
go

-- Обновление скалярных данных пользовательского канала
create procedure spUdtUserChannelInfo (
	@UserChannelID int,
	@DisplayName nvarchar(300),
	@OrderCol int,
	@ErrCode int out)
as
begin
	/* ErrCode = 82 - произошла ошибка обновления скалярных данных канала */
	set @ErrCode = 0;
	begin try
		update dbo.UserChannels set DisplayName = @DisplayName,
		OrderCol = @OrderCol
		where UserChannelID = @UserChannelID;
	end try
	begin catch
		set @ErrCode = 82;
	end catch	
end		
go

-- Обновление пиктограммы пользовательского канала
create procedure spUdtUserChannelImageByID(@UserChannelID int, @IconID bigint)
as
declare @ErrCode int;
begin
	/* ErrCode = 83 - произошла ошибка обновления кастомной пиктограммы пользовательского канала*/
	set @ErrCode = 0;
	begin try
		update dbo.UserChannels set IconID = @IconID
		where UserChannelID = @UserChannelID;
	end try
	begin catch 
		set @ErrCode = 83;
	end catch 
end
go

-- Обновление/вставка пиктограммы пользовательского канала
create procedure [dbo].[spUdtUserChannelImage](
	@UserChannelID int,
	@IconWebSrc nvarchar(550),
	@ChannelIconName nvarchar(256),
	@ContentType nvarchar(256),
	@ContentCoding nvarchar(256),
	@ChannelOrigIcon varbinary(7168),
	@ChannelIcon25 varbinary(7168),
	@IsSystem bit,
	@ErrCode int out
)
as
declare @cntIcon int;
declare @LengthOrigIcon int;
declare @LengthIcon25 int;
declare @IconID bigint;
begin
	/* ErrCode = 75 - произошла ошибка вставки/обновления пиктограммы
		ErrCode = 79 - слишком большая длина файла */
	set @ErrCode = 0;	
	set @LengthOrigIcon = LEN(@ChannelOrigIcon);
	set @LengthIcon25 = len(@ChannelIcon25);
    if (@LengthOrigIcon > 7168 or @LengthIcon25 > 7168)
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
		ContentOrig, Length, Content25, Length25, IsSystem)
		values (@ChannelIconName, @ContentType, @ContentCoding, 
		@ChannelOrigIcon, @LengthOrigIcon, @ChannelIcon25, @LengthIcon25, @IsSystem);
	    set @IconID = SCOPE_IDENTITY();
	    exec dbo.spUdtUserChannelImageByID @UserChannelID, @IconID;
	end
	else 
	begin
	    update dbo.MediaPic set ContentType = @ContentType, 
			ContentCoding = @ContentCoding, 
			ContentOrig = @ChannelOrigIcon, 
			Length = @LengthOrigIcon,
			Content25 = @ChannelIcon25,
			Length25 = @LengthIcon25,
			IsSystem = @IsSystem  
		where FileName Like @ChannelIconName	
	end	
	end try
	begin catch
		set @ErrCode = 75;
	end catch
end;
go