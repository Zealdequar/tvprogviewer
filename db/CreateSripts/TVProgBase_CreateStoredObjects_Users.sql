use TVProgBase;
GO

if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnIsUserNameExists'))
drop function fnIsUserNameExists;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnIsEmailExists'))
drop function fnIsEmailExists;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUserStart'))
drop procedure spUserStart;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnGetUserIDByUserName'))
drop function fnGetUserIDByUserName;
GO

-- Проверка пользовательского имени на совпадение в системе
create function fnIsUserNameExists (@UserName nvarchar(70))
returns bit
as 
begin
	if (exists(select 1	
	from dbo.SystemUsers su
	where su.UserName Like @UserName))
		return 1;
	else
		return 0;
	return 0;
end
go

-- Проверка на совпадение такого же email в системе
create function fnIsEmailExists(@Email nvarchar(300))
returns bit
as
begin
	if (exists(select 1
	from dbo.SystemUsers su
	where su.Email Like @Email))
		return 1;
	else 
	return 0;
	return 0;		
end;
go

-- Процедура регистрации нового пользователя в системе 
create procedure [dbo].[spUserStart] (
@UserName nvarchar(70), 
@PassHash nvarchar(100),
@PassExtend nvarchar(100),
@LastName nvarchar(150),
@FirstName nvarchar(150),
@MiddleName nvarchar(150),
@BirthDate datetime,
@Gender bit,
@Email nvarchar(300),
@MobPhoneNumber nvarchar(25),
@OtherPhoneNumber1 nvarchar(25),
@OtherPhoneNumber2 nvarchar(25),
@Address nvarchar(1000),
@GmtZone nvarchar(10),
@Catalog nvarchar(36),
@ErrCode int out
)
as 
declare 
@cntEmail int,
@UID bigint,
@GID bigint
begin
/*ErrCodes: 2 - Такое имя пользователя уже существует. Укажите другое имя пользователя.;
			3 - Пользователь с таким e-mail уже зарегистрирован в системе. Укажите другой e-mail. 
			70 — Произошла ошибка при регистрации в системе. Попробуйте совершить регистрацию позднее. 
			72 - Ошибка при вставке жанров или классов жанров
			73 - Ошибка при вставке рейтингов или классов рейтингов
			*/	
	set @ErrCode = 0;			
	if (dbo.fnIsUserNameExists(@UserName) = 1)
	begin
		set @ErrCode = 2;
		return;
	end;	
	
	if (dbo.fnIsEmailExists(@Email) = 1)
	begin
		set @ErrCode = 3;
		return;
	end;
	
	begin try
	insert into dbo.SystemUsers (
	UserName, 
	PassHash, 
	PassExtend, 
	LastName, 
	FirstName, 
	MiddleName, 
	BirthDate,
	Gender,
	Email,
	MobPhoneNumber,
	OtherPhoneNumber1,
	OtherPhoneNumber2,
	Address,
	GmtZone,
	Status,
	Catalog
	)
	values
	(
	@UserName,
	@PassHash,
	@PassExtend,
	@LastName,
	@FirstName,
	@MiddleName,
	@BirthDate,
	@Gender,
	@Email,
	@MobPhoneNumber,
	@OtherPhoneNumber1,
	@OtherPhoneNumber2,
	@Address,
	@GmtZone,
	3,
	@Catalog
	);
	end try
	begin catch
		select @ErrCode = 70;
	end catch
	set @UID = SCOPE_IDENTITY();
	
	begin try
	insert dbo.ExtUserSettings (UID, TVProgProviderID, UncheckedChannels)
	values (@UID, 1, 1);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 1, 'Без типа', 1);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 2, 'Художественный фильм', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'фильм', 'мульт;док;Д/ф', 1, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'х.ф', '', 2, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'х. ф', '', 3, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'т. ф', '', 4, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'драм', '', 5,NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'х/ф', '', 6, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'х\\ф', '', 7, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Режиссер', '', 8, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 3, 'Сериал', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'т/с', 'м/с', 9, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Теленовелла', '', 10, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Сери', 'М/;мульт;турнир', 11, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 4, 'Детям', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Детям', '', 12, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'детск', '', 13, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'дете', '', 14, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'сказка', '', 15, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Спокойной ночи, малыши', '', 16, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'До 16', '', 17, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Ералаш', '', 18,  NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'АБВГДейка', '', 19, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 5, 'Спорт', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'чемпион','', 20, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'гран-при','', 21, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'турнир','', 22, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'первенство','', 23, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Футбол','', 24, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Кубок','', 25, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'атлет','', 26, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'стадион','', 27, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'хоккей','', 28, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'спартак','', 29, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Физра','', 30, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Бои без правил','', 31, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Пенальти','', 32, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Трюкачи','', 33, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'НХЛ','', 34, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'матч','', 35, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'волейбол','', 36, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'бокс','', 37, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'НБА','', 38, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'ринг','оринг', 39, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'рекорд','', 40, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'физкульт','', 41, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'баскетбол','', 42, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'корт','', 43, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'ралли','', 44, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'гонки','', 45, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'олимп','', 46, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'финал', '', 47, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Боевые искус', '', 48, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Спорт', '', 49, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 6, 'Документальный фильм', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Док.', '', 50, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'д. ф', '', 51, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'документ', '', 52, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'репор', '', 53, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'т. ж.', '', 54, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Д/ф', '', 55, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Д/с', '', 56, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 7, 'Информ. программа', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'информац', 'тайна', 57, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Новост', 'в перерыве', 58, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Время', 'муз;личное', 59, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Вести', 'палаты', 60, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Итоговая программа', '', 61, NULL)
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Сегодня', '', 62, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'хроник', '', 63, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'События', '', 64, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Афиша', '', 65, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'News', '', 66, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Настроен', '', 67, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Доброе утро', '', 68, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'День за днем', '', 69, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Доска', '', 70, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '24', '', 71, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Неделя', '', 72, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Личный вклад', '', 73, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Постскриптум', '', 74, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Страна и мир', '', 75, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'ньюс', '', 76, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 8, 'Юмор', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'комеди', '', 77, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'юмор', '', 78, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'КВН', '', 79, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Клуб Веселых и Находчивых', '', 80, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Городок;городке', '', 81, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Жванецкий', '', 82, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Задорнов', '', 83, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Смех', '', 84, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Аншлаг', '', 85, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'О. С. П.', '', 86, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Большая разница', '', 87, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Каламбур', '', 88, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'фитиль', '', 89, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'шутка', '', 90, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'розыгрыш', '', 91, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Смешн', '', 92, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 9, 'Теле-шоу', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Шоу', '', 93, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'конкурс', '', 94, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Сам себе режиссер', '', 95, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Модный приговор', '', 96, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Давай поженимся!', '', 97, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Пусть говорят', '', 98, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дом 2', '', 99, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Час суда', '', 100, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'НТВшники', '', 101, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 10, 'Теле-игра', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'игра', 'гармонь;тигра;концерт', 102, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Поле чудес', '', 103, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'лото', 'олото', 104, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Умницы и умники', '', 105, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Десять миллионов', '', 106, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Кто хочет стать миллионером', '', 107, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Что? Где? Когда?', '', 108, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Своя игра', '', 109, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 11, 'Театр и кино', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'артист', '', 110, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'театр', '', 111, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Опера', 'операб;операц', 112, NULL)
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'спектакл', '', 113, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'балет', '', 114, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'актер', '', 115, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'кино', '', 116, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Stop! Снято!', '', 117, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Третий звонок', '', 118, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Большой экран', '', 119, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'сп.', '', 120, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'кумиры', '', 121, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Синемания', '', 122, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 12, 'Мультфильм', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'М/', '', 123, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'М. ф', '', 124, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'М/ сериал', '', 125, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дисней', '', 126, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'мульт', '', 127, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Футурама', '', 128, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Симпсоны', '', 129, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Fox Kids', '', 130, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 13, 'Криминал', 1	);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'детектив', '', 131, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'комиссар', '', 132, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Боевик', '', 133, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'ужас', '', 134, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'триллер', '', 135, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'криминал', '', 136, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дорожный патруль', '', 137, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дорожная часть', '', 138, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Чрезвычайное происшествие',  '', 139, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Петровка', '', 140, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Телеспецназ', '', 141, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'следствие', '', 142, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'расследование', '', 143, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'убийств', '', 144, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Чистосердечное признание', '', 145, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 14, 'Музыка', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Муз', 'музе', 146, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Концерт', '', 147, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Диск', '', 148, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'фестивал', '', 149, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'песн', 'чудес', 150, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'поют', '', 151, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'джаз', '', 152, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'БКЗ', '', 153, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'гармонь', '', 154, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Утренняя почта', '', 155, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Два рояля', '', 156, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'оркестр', '', 157, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'хит', '', 158, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'романс', '', 159, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'клип', '', 160, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'шансон', '', 161, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Танцпол', '', 162, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Русская 10-ка', '', 163, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Мобильная 10-ка', '', 164, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Танцкласс', '', 165, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Зажигай!', '', 166, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Extra', '', 167, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Impulse', '', 168, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'чарт', '', 169, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Горячая десятка', '', 170, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Утренняя звезда', '', 171, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '10 наших', '', 172, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 15, 'Интернет', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Интернет', '', 173, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '.Ru', '', 174, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '.NET', '', 175, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Глобаль', '', 176, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Всемирная паутина', '', 177, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'сеть', '', 178, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'www', '', 179, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 16, 'О животных', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'животн', '', 180, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'зоо', '', 181, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'звер', '', 182, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'природ', '', 183, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'я и моя собака', '', 184, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'диалоги о рыбалке', '', 185, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Рыболов', '', 186, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'сами с усами', '', 187, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'динозавр', '', 188, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'пород', '', 189, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'ветерин', '', 190, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'хищник', '', 191, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 17, 'Погода', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Погод', '', 192, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Солнечный прогноз', '', 193, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 18, 'Искусство и культура', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'искусств', 'боев', 194, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'музе', '', 195, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'культур', '', 196, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'интерьер', '', 197, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Артэкспресс', '', 198, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Архтектур', '', 199, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дворцовые тайны', '', 200, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Царская ложа', '', 201, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'эрмитаж', '', 202, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 19, 'Политика и социология', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'полити', '', 203, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Международ', '', 204, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Съезд', '', 205, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Федерация', '', 206, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Парламентский', '', 207, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'зеркало', 'Петросян', 208, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 20, 'Научно-популярная передача', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Xфактор', '', 209, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Цивилизац', '', 210, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Наук', '', 211, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Экологи', '', 212, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Космос', '', 213, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'тайна', '', 214, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 21, 'Путешествие и краеведение', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'город','', 215, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Путе', '', 216, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Непутевые заметки', '', 217, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Хочу всё знать', '', 218, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Турбюро', '', 219, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Знаменитые замки', '', 220, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Планета Земля', '', 221, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Вокруг света', '', 222, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Вояж без саквояжа', '', 223, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Чудес', '', 224, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Приключени', '', 225, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Гербы России', '', 226, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Окно в мир', '', 227, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'История', '', 228, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Поля сражений', '', 229, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Их нравы', '', 230, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'экспедиц', '', 231, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 22, 'Персона', 1); 
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'в гостях', '', 232, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Антропология', '', 233, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Пока все дома', '', 234, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Голливуд', '', 235, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Женский взгляд', '', 236, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Встреча с', '', 237, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Жизнь замечательных людей', '', 238, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'мужчина и женщина', '', 239, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'завтрак', '', 240, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Без протокола', '', 241, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Момент истины', '', 242, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 23, 'Техника', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'компьютер', '', 243, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Авиац', '', 244, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'изобрет', '', 245, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'монитор', '', 246, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'PRO-новости', '', 247, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 24, 'Мода', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'мода', '', 248, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'моды', '', 249, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'fashion', '', 250, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Мир красоты', '', 251, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Шпилька', '', 252, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Видеомода', '', 253, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Стильные штучки', '', 254, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Стилиссимо', '', 255, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 25, 'Авто', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Авто;мото', 'Автор;стаи', 256, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Карданный вал', '', 257, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Фаркоп', '', 258, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Машин', '', 259, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Top Gear', '', 260, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 26, 'Здоровье', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'здоров', '', 261, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Без рецепта', '', 262, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'доктор', '', 263, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'мед.', '', 264, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'медиц', '', 265, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 27, 'Религия', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'пастыря', '', 266, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'религ', '', 267, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Православ', '', 268, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Победоносный голос верующего', '', 269, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Библ', 'мания', 270, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Из библейсиких пророчеств', '', 271, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Благая весть', '', 272, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Коран', '', 273, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Воскресная школа', '', 274, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'христ', '', 275, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Патриарх', '', 276, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Муфтий', '', 277, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Час силы духа', '', 278, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 28, 'Садоводство', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'сад', 'кольцо' , 279, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Растительная жизнь', '', 280, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'урожай', '', 281, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'даче', 'передаче', 282, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 29, 'Реклама', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'магазин', '', 283, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'реклам', '', 284, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'телепокупка', '', 285, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 30, 'Женские', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Из жизни женщины', '', 286, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дамский клуб', '', 287, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Женские истории', '', 288, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Для женщин', '', 289, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'мама;мать', '', 290, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 31, 'Кулинария', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Кухня', '', 291, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Вкусные истории', '', 292, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Сладкие истории', '', 293, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Вкус', '', 294, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'деликатес', '', 295, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'кулинар', '', 296, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'смак', '', 297, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Лакомый кусочек', '', 298, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'блюд', '', 299, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 32, 'Экстрим', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'экстр', 'машин', 300, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Служба спасения', '', 301, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Катастрофы недели', '', 302, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Дорожные войны', '', 303, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'каскаде', '', 304, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 33, 'Экономика и бизнес', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Деньги', '', 305, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Бизнес', '', 306, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Инвестиции', '', 307, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Финанс', '', 308, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Делов', '', 309, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 34, 'Эротика', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Секс', '', 310, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Эрот', '', 311, NULL);
	end try
	begin catch
		set @ErrCode = 72;
	end catch 
	
	begin try    
		insert dbo.Ratings (uid, IconID, RatingName, Visible) 
		values (@UID, 35, 'Без рейтинга', 1),
		(@UID, 36, 'Можно посмотреть', 1),
		(@UID, 37, 'Приличные', 1),
		(@UID, 38, 'Нормальные',1),
		(@UID, 39, 'Хорошие', 1),
		(@UID, 40, 'Отличные',1);
	end try
	begin catch
		set @ErrCode = 73;
	end catch 
end;
GO

-- Получение UserID по UserNanme
create function fnGetUserIDByUserName(@UserName nvarchar(70))
returns int
as
begin
	declare @UserID int;
	select @UserID = su.UserID 
	from dbo.SystemUsers su
	where su.UserName Like @UserName;
	return @UserID;
end;
go