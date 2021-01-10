if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUserStart'))
drop procedure spUserStart;
GO

-- Процедура регистрации нового пользователя в системе 
create procedure [dbo].[spUserStart] (
@UserId int,
@ErrCode int out
)
as 
declare @GenreId bigint
begin
/*ErrCodes: 70 — Произошла ошибка при регистрации в системе. Попробуйте совершить регистрацию позднее. 
			72 - Ошибка при вставке жанров или классов жанров
			73 - Ошибка при вставке рейтингов или классов рейтингов
			*/	
	
	begin try
	
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 1, 'Без типа', 1);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 2, 'Художественный фильм', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'фильм', 'мульт;док;Д/ф', 1, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'х.ф', '', 2, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'х. ф', '', 3, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'т. ф', '', 4, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'драм', '', 5,NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'х/ф', '', 6, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'х\\ф', '', 7, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Режиссер', '', 8, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 3, 'Сериал', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'т/с', 'м/с', 9, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Теленовелла', '', 10, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Сери', 'М/;мульт;турнир', 11, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 4, 'Детям', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Детям', '', 12, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'детск', '', 13, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'дете', '', 14, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'сказка', '', 15, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Спокойной ночи, малыши', '', 16, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'До 16', '', 17, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Ералаш', '', 18,  NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'АБВГДейка', '', 19, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 5, 'Спорт', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'чемпион','', 20, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'гран-при','', 21, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'турнир','', 22, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'первенство','', 23, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Футбол','', 24, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Кубок','', 25, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'атлет','', 26, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'стадион','', 27, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'хоккей','', 28, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'спартак','', 29, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Физра','', 30, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Бои без правил','', 31, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Пенальти','', 32, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Трюкачи','', 33, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'НХЛ','', 34, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'матч','', 35, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'волейбол','', 36, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'бокс','', 37, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'НБА','', 38, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'ринг','оринг', 39, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'рекорд','', 40, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'физкульт','', 41, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'баскетбол','', 42, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'корт','', 43, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'ралли','', 44, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'гонки','', 45, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'олимп','', 46, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'финал', '', 47, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Боевые искус', '', 48, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Спорт', '', 49, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 6, 'Документальный фильм', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Док.', '', 50, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'д. ф', '', 51, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'документ', '', 52, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'репор', '', 53, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'т. ж.', '', 54, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Д/ф', '', 55, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Д/с', '', 56, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 7, 'Информ. программа', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'информац', 'тайна', 57, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Новост', 'в перерыве', 58, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Время', 'муз;личное', 59, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Вести', 'палаты', 60, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Итоговая программа', '', 61, NULL)
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Сегодня', '', 62, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'хроник', '', 63, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'События', '', 64, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Афиша', '', 65, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'News', '', 66, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Настроен', '', 67, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Доброе утро', '', 68, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'День за днем', '', 69, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Доска', '', 70, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, '24', '', 71, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Неделя', '', 72, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Личный вклад', '', 73, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Постскриптум', '', 74, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Страна и мир', '', 75, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'ньюс', '', 76, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 8, 'Юмор', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'комеди', '', 77, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'юмор', '', 78, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'КВН', '', 79, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Клуб Веселых и Находчивых', '', 80, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Городок;городке', '', 81, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Жванецкий', '', 82, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Задорнов', '', 83, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Смех', '', 84, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Аншлаг', '', 85, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'О. С. П.', '', 86, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Большая разница', '', 87, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Каламбур', '', 88, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'фитиль', '', 89, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'шутка', '', 90, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'розыгрыш', '', 91, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Смешн', '', 92, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 9, 'Теле-шоу', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Шоу', '', 93, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'конкурс', '', 94, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Сам себе режиссер', '', 95, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Модный приговор', '', 96, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Давай поженимся!', '', 97, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Пусть говорят', '', 98, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дом 2', '', 99, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Час суда', '', 100, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'НТВшники', '', 101, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 10, 'Теле-игра', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'игра', 'гармонь;тигра;концерт', 102, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Поле чудес', '', 103, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'лото', 'олото', 104, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Умницы и умники', '', 105, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Десять миллионов', '', 106, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Кто хочет стать миллионером', '', 107, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Что? Где? Когда?', '', 108, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Своя игра', '', 109, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 11, 'Театр и кино', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'артист', '', 110, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'театр', '', 111, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Опера', 'операб;операц', 112, NULL)
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'спектакл', '', 113, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'балет', '', 114, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'актер', '', 115, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'кино', '', 116, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Stop! Снято!', '', 117, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Третий звонок', '', 118, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Большой экран', '', 119, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'сп.', '', 120, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'кумиры', '', 121, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Синемания', '', 122, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 12, 'Мультфильм', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'М/', '', 123, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'М. ф', '', 124, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'М/ сериал', '', 125, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дисней', '', 126, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'мульт', '', 127, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Футурама', '', 128, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Симпсоны', '', 129, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Fox Kids', '', 130, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 13, 'Криминал', 1	);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'детектив', '', 131, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'комиссар', '', 132, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Боевик', '', 133, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'ужас', '', 134, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'триллер', '', 135, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'криминал', '', 136, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дорожный патруль', '', 137, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дорожная часть', '', 138, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Чрезвычайное происшествие',  '', 139, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Петровка', '', 140, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Телеспецназ', '', 141, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'следствие', '', 142, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'расследование', '', 143, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'убийств', '', 144, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Чистосердечное признание', '', 145, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 14, 'Музыка', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Муз', 'музе', 146, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Концерт', '', 147, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Диск', '', 148, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'фестивал', '', 149, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'песн', 'чудес', 150, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'поют', '', 151, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'джаз', '', 152, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'БКЗ', '', 153, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'гармонь', '', 154, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Утренняя почта', '', 155, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Два рояля', '', 156, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'оркестр', '', 157, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'хит', '', 158, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'романс', '', 159, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'клип', '', 160, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'шансон', '', 161, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Танцпол', '', 162, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Русская 10-ка', '', 163, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Мобильная 10-ка', '', 164, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Танцкласс', '', 165, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Зажигай!', '', 166, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Extra', '', 167, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Impulse', '', 168, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'чарт', '', 169, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Горячая десятка', '', 170, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Утренняя звезда', '', 171, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, '10 наших', '', 172, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 15, 'Интернет', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Интернет', '', 173, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, '.Ru', '', 174, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, '.NET', '', 175, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Глобаль', '', 176, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Всемирная паутина', '', 177, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'сеть', '', 178, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'www', '', 179, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 16, 'О животных', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'животн', '', 180, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'зоо', '', 181, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'звер', '', 182, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'природ', '', 183, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'я и моя собака', '', 184, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'диалоги о рыбалке', '', 185, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Рыболов', '', 186, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'сами с усами', '', 187, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'динозавр', '', 188, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'пород', '', 189, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'ветерин', '', 190, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'хищник', '', 191, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 17, 'Погода', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Погод', '', 192, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Солнечный прогноз', '', 193, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 18, 'Искусство и культура', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'искусств', 'боев', 194, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'музе', '', 195, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'культур', '', 196, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'интерьер', '', 197, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Артэкспресс', '', 198, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Архтектур', '', 199, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дворцовые тайны', '', 200, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Царская ложа', '', 201, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'эрмитаж', '', 202, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 19, 'Политика и социология', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'полити', '', 203, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Международ', '', 204, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Съезд', '', 205, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Федерация', '', 206, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Парламентский', '', 207, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'зеркало', 'Петросян', 208, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 20, 'Научно-популярная передача', 1);
	set @GenreId = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Xфактор', '', 209, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Цивилизац', '', 210, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Наук', '', 211, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Экологи', '', 212, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Космос', '', 213, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'тайна', '', 214, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 21, 'Путешествие и краеведение', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'город','', 215, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Путе', '', 216, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Непутевые заметки', '', 217, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Хочу всё знать', '', 218, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Турбюро', '', 219, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Знаменитые замки', '', 220, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Планета Земля', '', 221, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Вокруг света', '', 222, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Вояж без саквояжа', '', 223, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Чудес', '', 224, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Приключени', '', 225, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Гербы России', '', 226, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Окно в мир', '', 227, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'История', '', 228, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Поля сражений', '', 229, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Их нравы', '', 230, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'экспедиц', '', 231, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 22, 'Персона', 1); 
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'в гостях', '', 232, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Антропология', '', 233, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Пока все дома', '', 234, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Голливуд', '', 235, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Женский взгляд', '', 236, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Встреча с', '', 237, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Жизнь замечательных людей', '', 238, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'мужчина и женщина', '', 239, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'завтрак', '', 240, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Без протокола', '', 241, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Момент истины', '', 242, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 23, 'Техника', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'компьютер', '', 243, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Авиац', '', 244, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'изобрет', '', 245, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'монитор', '', 246, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'PRO-новости', '', 247, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 24, 'Мода', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'мода', '', 248, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'моды', '', 249, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'fashion', '', 250, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Мир красоты', '', 251, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Шпилька', '', 252, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Видеомода', '', 253, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Стильные штучки', '', 254, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Стилиссимо', '', 255, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 25, 'Авто', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Авто;мото', 'Автор;стаи', 256, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Карданный вал', '', 257, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Фаркоп', '', 258, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Машин', '', 259, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Top Gear', '', 260, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 26, 'Здоровье', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'здоров', '', 261, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Без рецепта', '', 262, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'доктор', '', 263, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'мед.', '', 264, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'медиц', '', 265, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 27, 'Религия', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'пастыря', '', 266, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'религ', '', 267, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Православ', '', 268, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Победоносный голос верующего', '', 269, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Библ', 'мания', 270, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Из библейсиких пророчеств', '', 271, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Благая весть', '', 272, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Коран', '', 273, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Воскресная школа', '', 274, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'христ', '', 275, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Патриарх', '', 276, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Муфтий', '', 277, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Час силы духа', '', 278, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Священ', '', 279, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 28, 'Садоводство', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'сад', 'кольцо' , 280, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Растительная жизнь', '', 281, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'урожай', '', 282, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'даче', 'передаче', 283, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 29, 'Реклама', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'магазин', '', 284, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'реклам', '', 285, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'телепокупка', '', 286, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 30, 'Женские', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Из жизни женщины', '', 287, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дамский клуб', '', 288, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Женские истории', '', 289, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Для женщин', '', 290, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'мама;мать', '', 291, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 31, 'Кулинария', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Кухня', '', 292, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Вкусные истории', '', 293, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Сладкие истории', '', 294, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Вкус', '', 295, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'деликатес', '', 296, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'кулинар', '', 297, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'смак', '', 298, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Лакомый кусочек', '', 299, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'блюд', '', 300, NULL);
	
	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 32, 'Экстрим', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'экстр', 'машин', 301, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Служба спасения', '', 302, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Катастрофы недели', '', 303, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Дорожные войны', '', 304, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'каскаде', '', 305, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 33, 'Экономика и бизнес', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Деньги', '', 306, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Бизнес', '', 307, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Инвестиции', '', 308, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Финанс', '', 309, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Делов', '', 310, NULL);

	insert dbo.Genres (UserId, IconId, GenreName, Visible) values (@UserId, 34, 'Эротика', 1);
	set @GenreId = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Секс', '', 311, NULL);
	insert dbo.GenreClassificator(GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GenreId, @UserId, 'Эрот', '', 312, NULL);
	end try
	begin catch
		set @ErrCode = 72;
	end catch 
	
	begin try    
		insert dbo.Ratings (UserId, IconId, RatingName, Visible) 
		values (@UserId, 37, 'Без рейтинга', 1),
		(@UserId, 38, 'Можно посмотреть', 1),
		(@UserId, 39, 'Приличные', 1),
		(@UserId, 40, 'Нормальные',1),
		(@UserId, 41, 'Хорошие', 1),
		(@UserId, 42, 'Отличные',1);
	end try
	begin catch
		set @ErrCode = 73;
	end catch 
end;
GO

CREATE FUNCTION [nop_splitstring_to_table]
(
    @string NVARCHAR(MAX),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(MAX)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO

CREATE PROCEDURE [ProductLoadAllPaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerId		int = 0,
	@StoreId			int = 0,
	@VendorId			int = 0,
	@WarehouseId		int = 0,
	@ProductTypeId		int = null, --product type identifier, null - load all products
	@VisibleIndividuallyOnly bit = 0, 	--0 - load all products , 1 - "visible indivially" only
	@MarkedAsNewOnly	bit = 0, 	--0 - load all products , 1 - "marked as new" only
	@ProductTaGenreId		int = 0,
	@FeaturedProducts	bit = null,	--0 featured only , 1 not featured only, null - load all products
	@PriceMin			decimal(18, 4) = null,
	@PriceMax			decimal(18, 4) = null,
	@Keywords			nvarchar(4000) = null,
	@SearchDescriptions bit = 0, --a value indicating whether to search by a specified "keyword" in product descriptions
	@SearchManufacturerPartNumber bit = 0, -- a value indicating whether to search by a specified "keyword" in manufacturer part number
	@SearchSku			bit = 0, --a value indicating whether to search by a specified "keyword" in product SKU
	@SearchProductTags  bit = 0, --a value indicating whether to search by a specified "keyword" in product tags
	@UseFullTextSearch  bit = 0,
	@FullTextMode		int = 0, --0 - using CONTAINS with <prefix_term>, 5 - using CONTAINS and OR with <prefix_term>, 10 - using CONTAINS and AND with <prefix_term>
	@FilteredSpecs		nvarchar(MAX) = null,	--filter by specification attribute options (comma-separated list of IDs). e.g. 14,15,16
	@LanguageId			int = 0,
	@OrderBy			int = 0, --0 - position, 5 - Name: A to Z, 6 - Name: Z to A, 10 - Price: Low to High, 11 - Price: High to Low, 15 - creation date
	@AlloweduserRoleIds	nvarchar(MAX) = null,	--a list of user role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@ShowHidden			bit = 0,
	@OverridePublished	bit = null, --null - process "Published" property according to "showHidden" parameter, true - load only "Published" products, false - load only "Unpublished" products
	@LoadFilterableSpecificationAttributeOptionIds bit = 0, --a value indicating whether we should load the specification attribute option identifiers applied to loaded products (all pages)
	@FilterableSpecificationAttributeOptionIds nvarchar(MAX) = null OUTPUT, --the specification attribute option identifiers applied to loaded products (all pages). returned as a comma separated list of identifiers
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	
	/* Products that filtered by keywords */
	CREATE TABLE #KeywordProducts
	(
		[ProductId] int NOT NULL
	)

	DECLARE
		@SearchKeywords bit,
		@OriginalKeywords nvarchar(4000),
		@sql nvarchar(max),
		@sql_orderby nvarchar(max)

	SET NOCOUNT ON
	
	--filter by keywords
	SET @Keywords = isnull(@Keywords, '')
	SET @Keywords = rtrim(ltrim(@Keywords))
	SET @OriginalKeywords = @Keywords
	IF ISNULL(@Keywords, '') != ''
	BEGIN
		SET @SearchKeywords = 1
		
		IF @UseFullTextSearch = 1
		BEGIN
			--remove wrong chars (' ")
			SET @Keywords = REPLACE(@Keywords, '''', '')
			SET @Keywords = REPLACE(@Keywords, '"', '')
			
			--full-text search
			IF @FullTextMode = 0 
			BEGIN
				--0 - using CONTAINS with <prefix_term>
				SET @Keywords = ' "' + @Keywords + '*" '
			END
			ELSE
			BEGIN
				--5 - using CONTAINS and OR with <prefix_term>
				--10 - using CONTAINS and AND with <prefix_term>

				--clean multiple spaces
				WHILE CHARINDEX('  ', @Keywords) > 0 
					SET @Keywords = REPLACE(@Keywords, '  ', ' ')

				DECLARE @concat_term nvarchar(100)				
				IF @FullTextMode = 5 --5 - using CONTAINS and OR with <prefix_term>
				BEGIN
					SET @concat_term = 'OR'
				END 
				IF @FullTextMode = 10 --10 - using CONTAINS and AND with <prefix_term>
				BEGIN
					SET @concat_term = 'AND'
				END

				--now let's build search string
				declare @fulltext_keywords nvarchar(4000)
				set @fulltext_keywords = N''
				declare @index int		
		
				set @index = CHARINDEX(' ', @Keywords, 0)

				-- if index = 0, then only one field was passed
				IF(@index = 0)
					set @fulltext_keywords = ' "' + @Keywords + '*" '
				ELSE
				BEGIN		
                    DECLARE @len_keywords INT
					DECLARE @len_nvarchar INT
					SET @len_keywords = 0
					SET @len_nvarchar = DATALENGTH(CONVERT(NVARCHAR(MAX), 'a'))

					DECLARE @first BIT
					SET  @first = 1			
					WHILE @index > 0
					BEGIN
						IF (@first = 0)
							SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' '
						ELSE
							SET @first = 0

                        --LEN excludes trailing spaces. That is why we use DATALENGTH
						--see https://docs.microsoft.com/sql/t-sql/functions/len-transact-sql?view=sqlallproducts-allversions for more ditails
						SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar

						SET @fulltext_keywords = @fulltext_keywords + '"' + SUBSTRING(@Keywords, 1, @index - 1) + '*"'					
						SET @Keywords = SUBSTRING(@Keywords, @index + 1, @len_keywords - @index)						
						SET @index = CHARINDEX(' ', @Keywords, 0)
					end
					
					-- add the last field
                    SET @len_keywords = DATALENGTH(@Keywords) / @len_nvarchar
					IF LEN(@fulltext_keywords) > 0
						SET @fulltext_keywords = @fulltext_keywords + ' ' + @concat_term + ' ' + '"' + SUBSTRING(@Keywords, 1, @len_keywords) + '*"'	
				END
				SET @Keywords = @fulltext_keywords
			END
		END
		ELSE
		BEGIN
			--usual search by PATINDEX
			SET @Keywords = '%' + @Keywords + '%'
		END
		--PRINT @Keywords

		--product name
		SET @sql = '
		INSERT INTO #KeywordProducts ([ProductId])
		SELECT p.Id
		FROM Product p with (NOLOCK)
		WHERE '
		IF @UseFullTextSearch = 1
			SET @sql = @sql + 'CONTAINS(p.[Name], @Keywords) '
		ELSE
			SET @sql = @sql + 'PATINDEX(@Keywords, p.[Name]) > 0 '

		IF @SearchDescriptions = 1
		BEGIN
			--product short description
			IF @UseFullTextSearch = 1
			BEGIN
				SET @sql = @sql + 'OR CONTAINS(p.[ShortDescription], @Keywords) '
				SET @sql = @sql + 'OR CONTAINS(p.[FullDescription], @Keywords) '
			END
			ELSE
			BEGIN
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[ShortDescription]) > 0 '
				SET @sql = @sql + 'OR PATINDEX(@Keywords, p.[FullDescription]) > 0 '
			END
		END

		--manufacturer part number (exact match)
		IF @SearchManufacturerPartNumber = 1
		BEGIN
			SET @sql = @sql + 'OR p.[ManufacturerPartNumber] = @OriginalKeywords '
		END

		--SKU (exact match)
		IF @SearchSku = 1
		BEGIN
			SET @sql = @sql + 'OR p.[Sku] = @OriginalKeywords '
		END

		--localized product name
		SET @sql = @sql + '
		UNION
		SELECT lp.EntityId
		FROM LocalizedProperty lp with (NOLOCK)
		WHERE
			lp.LocaleKeyGroup = N''Product''
			AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
			AND ( (lp.LocaleKey = N''Name'''
		IF @UseFullTextSearch = 1
			SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
		ELSE
			SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

		IF @SearchDescriptions = 1
		BEGIN
			--localized product short description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''ShortDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '

			--localized product full description
			SET @sql = @sql + '
				OR (lp.LocaleKey = N''FullDescription'''
			IF @UseFullTextSearch = 1
				SET @sql = @sql + ' AND CONTAINS(lp.[LocaleValue], @Keywords)) '
			ELSE
				SET @sql = @sql + ' AND PATINDEX(@Keywords, lp.[LocaleValue]) > 0) '
		END

		SET @sql = @sql + ' ) '

		IF @SearchProductTags = 1
		BEGIN
			--product tags (exact match)
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM Product_ProductTag_Mapping pptm with(NOLOCK) INNER JOIN ProductTag pt with(NOLOCK) ON pt.Id = pptm.ProductTag_Id
			WHERE pt.[Name] = @OriginalKeywords '

			--localized product tags
			SET @sql = @sql + '
			UNION
			SELECT pptm.Product_Id
			FROM LocalizedProperty lp with (NOLOCK) INNER JOIN Product_ProductTag_Mapping pptm with(NOLOCK) ON lp.EntityId = pptm.ProductTag_Id
			WHERE
				lp.LocaleKeyGroup = N''ProductTag''
				AND lp.LanguageId = ' + ISNULL(CAST(@LanguageId AS nvarchar(max)), '0') + '
				AND lp.LocaleKey = N''Name''
				AND lp.[LocaleValue] = @OriginalKeywords '
		END

		--PRINT (@sql)
		EXEC sp_executesql @sql, N'@Keywords nvarchar(4000), @OriginalKeywords nvarchar(4000)', @Keywords, @OriginalKeywords

	END
	ELSE
	BEGIN
		SET @SearchKeywords = 0
	END

	--filter by category IDs
	SET @CategoryIds = isnull(@CategoryIds, '')	
	CREATE TABLE #FilteredCategoryIds
	(
		CategoryId int not null
	)
	INSERT INTO #FilteredCategoryIds (CategoryId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@CategoryIds, ',')	
	DECLARE @CategoryIdsCount int	
	SET @CategoryIdsCount = (SELECT COUNT(1) FROM #FilteredCategoryIds)

	--filter by user role IDs (access control list)
	SET @AlloweduserRoleIds = isnull(@AlloweduserRoleIds, '')	
	CREATE TABLE #FiltereduserRoleIds
	(
		UserRoleId int not null
	)
	INSERT INTO #FiltereduserRoleIds (UserRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AlloweduserRoleIds, ',')
	DECLARE @FiltereduserRoleIdsCount int	
	SET @FiltereduserRoleIdsCount = (SELECT COUNT(1) FROM #FiltereduserRoleIds)
	
	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1
	
	CREATE TABLE #DisplayOrderTmp 
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)

	SET @sql = '
	SELECT p.Id
	FROM
		Product p with (NOLOCK)'
	
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Category_Mapping pcm with (NOLOCK)
			ON p.Id = pcm.ProductId'
	END
	
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_Manufacturer_Mapping pmm with (NOLOCK)
			ON p.Id = pmm.ProductId'
	END
	
	IF ISNULL(@ProductTaGenreId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		INNER JOIN Product_ProductTag_Mapping pptm with (NOLOCK)
			ON p.Id = pptm.Product_Id'
	END
	
	--searching by keywords
	IF @SearchKeywords = 1
	BEGIN
		SET @sql = @sql + '
		JOIN #KeywordProducts kp
			ON  p.Id = kp.ProductId'
	END
	
	SET @sql = @sql + '
	WHERE
		p.Deleted = 0'
	
	--filter by category
	IF @CategoryIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND pcm.CategoryId IN ('
		
		SET @sql = @sql + + CAST(@CategoryIds AS nvarchar(max))

		SET @sql = @sql + ')'

		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pcm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by manufacturer
	IF @ManufacturerId > 0
	BEGIN
		SET @sql = @sql + '
		AND pmm.ManufacturerId = ' + CAST(@ManufacturerId AS nvarchar(max))
		
		IF @FeaturedProducts IS NOT NULL
		BEGIN
			SET @sql = @sql + '
		AND pmm.IsFeaturedProduct = ' + CAST(@FeaturedProducts AS nvarchar(max))
		END
	END
	
	--filter by vendor
	IF @VendorId > 0
	BEGIN
		SET @sql = @sql + '
		AND p.VendorId = ' + CAST(@VendorId AS nvarchar(max))
	END
	
	--filter by warehouse
	IF @WarehouseId > 0
	BEGIN
		--we should also ensure that 'ManageInventoryMethodId' is set to 'ManageStock' (1)
		--but we skip it in order to prevent hard-coded values (e.g. 1) and for better performance
		SET @sql = @sql + '
		AND  
			(
				(p.UseMultipleWarehouses = 0 AND
					p.WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ')
				OR
				(p.UseMultipleWarehouses > 0 AND
					EXISTS (SELECT 1 FROM ProductWarehouseInventory [pwi]
					WHERE [pwi].WarehouseId = ' + CAST(@WarehouseId AS nvarchar(max)) + ' AND [pwi].ProductId = p.Id))
			)'
	END
	
	--filter by product type
	IF @ProductTypeId is not null
	BEGIN
		SET @sql = @sql + '
		AND p.ProductTypeId = ' + CAST(@ProductTypeId AS nvarchar(max))
	END
	
	--filter by "visible individually"
	IF @VisibleIndividuallyOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.VisibleIndividually = 1'
	END
	
	--filter by "marked as new"
	IF @MarkedAsNewOnly = 1
	BEGIN
		SET @sql = @sql + '
		AND p.MarkAsNew = 1
		AND (getutcdate() BETWEEN ISNULL(p.MarkAsNewStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.MarkAsNewEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--filter by product tag
	IF ISNULL(@ProductTaGenreId, 0) != 0
	BEGIN
		SET @sql = @sql + '
		AND pptm.ProductTag_Id = ' + CAST(@ProductTaGenreId AS nvarchar(max))
	END
	
	--"Published" property
	IF (@OverridePublished is null)
	BEGIN
		--process according to "showHidden"
		IF @ShowHidden = 0
		BEGIN
			SET @sql = @sql + '
			AND p.Published = 1'
		END
	END
	ELSE IF (@OverridePublished = 1)
	BEGIN
		--published only
		SET @sql = @sql + '
		AND p.Published = 1'
	END
	ELSE IF (@OverridePublished = 0)
	BEGIN
		--unpublished only
		SET @sql = @sql + '
		AND p.Published = 0'
	END
	
	--show hidden
	IF @ShowHidden = 0
	BEGIN
		SET @sql = @sql + '
		AND p.Deleted = 0
		AND (getutcdate() BETWEEN ISNULL(p.AvailableStartDateTimeUtc, ''1/1/1900'') and ISNULL(p.AvailableEndDateTimeUtc, ''1/1/2999''))'
	END
	
	--min price
	IF @PriceMin is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price >= ' + CAST(@PriceMin AS nvarchar(max)) + ')'
	END
	
	--max price
	IF @PriceMax is not null
	BEGIN
		SET @sql = @sql + '
		AND (p.Price <= ' + CAST(@PriceMax AS nvarchar(max)) + ')'
	END
	
	--show hidden and ACL
	IF  @ShowHidden = 0 and @FiltereduserRoleIdsCount > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FiltereduserRoleIds [fcr]
			WHERE
				[fcr].UserRoleId IN (
					SELECT [acl].UserRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = ''Product''
				)
			))'
	END
	
	--filter by store
	IF @StoreId > 0
	BEGIN
		SET @sql = @sql + '
		AND (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = ''Product'' and [sm].StoreId=' + CAST(@StoreId AS nvarchar(max)) + '
			))'
	END
	
    --prepare filterable specification attribute option identifier (if requested)
    IF @LoadFilterableSpecificationAttributeOptionIds = 1
	BEGIN		
		CREATE TABLE #FilterableSpecs 
		(
			[SpecificationAttributeOptionId] int NOT NULL
		)
        DECLARE @sql_filterableSpecs nvarchar(max)
        SET @sql_filterableSpecs = '
	        INSERT INTO #FilterableSpecs ([SpecificationAttributeOptionId])
	        SELECT DISTINCT [psam].SpecificationAttributeOptionId
	        FROM [Product_SpecificationAttribute_Mapping] [psam] WITH (NOLOCK)
	            WHERE [psam].[AllowFiltering] = 1
	            AND [psam].[ProductId] IN (' + @sql + ')'

        EXEC sp_executesql @sql_filterableSpecs

		--build comma separated list of filterable identifiers
		SELECT @FilterableSpecificationAttributeOptionIds = COALESCE(@FilterableSpecificationAttributeOptionIds + ',' , '') + CAST(SpecificationAttributeOptionId as nvarchar(4000))
		FROM #FilterableSpecs

		DROP TABLE #FilterableSpecs
 	END

	--filter by specification attribution options
	SET @FilteredSpecs = isnull(@FilteredSpecs, '')	
	CREATE TABLE #FilteredSpecs
	(
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecs (SpecificationAttributeOptionId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@FilteredSpecs, ',') 

    CREATE TABLE #FilteredSpecsWithAttributes
	(
        SpecificationAttributeId int not null,
		SpecificationAttributeOptionId int not null
	)
	INSERT INTO #FilteredSpecsWithAttributes (SpecificationAttributeId, SpecificationAttributeOptionId)
	SELECT sao.SpecificationAttributeId, fs.SpecificationAttributeOptionId
    FROM #FilteredSpecs fs INNER JOIN SpecificationAttributeOption sao ON sao.Id = fs.SpecificationAttributeOptionId
    ORDER BY sao.SpecificationAttributeId 

    DECLARE @SpecAttributesCount int	
	SET @SpecAttributesCount = (SELECT COUNT(1) FROM #FilteredSpecsWithAttributes)
	IF @SpecAttributesCount > 0
	BEGIN
		--do it for each specified specification option
		DECLARE @SpecificationAttributeOptionId int
        DECLARE @SpecificationAttributeId int
        DECLARE @LastSpecificationAttributeId int
        SET @LastSpecificationAttributeId = 0
		DECLARE cur_SpecificationAttributeOption CURSOR FOR
		SELECT SpecificationAttributeId, SpecificationAttributeOptionId
		FROM #FilteredSpecsWithAttributes

		OPEN cur_SpecificationAttributeOption
        FOREACH:
            FETCH NEXT FROM cur_SpecificationAttributeOption INTO @SpecificationAttributeId, @SpecificationAttributeOptionId
            IF (@LastSpecificationAttributeId <> 0 AND @SpecificationAttributeId <> @LastSpecificationAttributeId OR @@FETCH_STATUS <> 0) 
			    SET @sql = @sql + '
        AND p.Id in (select psam.ProductId from [Product_SpecificationAttribute_Mapping] psam with (NOLOCK) where psam.AllowFiltering = 1 and psam.SpecificationAttributeOptionId IN (SELECT SpecificationAttributeOptionId FROM #FilteredSpecsWithAttributes WHERE SpecificationAttributeId = ' + CAST(@LastSpecificationAttributeId AS nvarchar(max)) + '))'
            SET @LastSpecificationAttributeId = @SpecificationAttributeId
		IF @@FETCH_STATUS = 0 GOTO FOREACH
		CLOSE cur_SpecificationAttributeOption
		DEALLOCATE cur_SpecificationAttributeOption
	END

	--sorting
	SET @sql_orderby = ''	
	IF @OrderBy = 5 /* Name: A to Z */
		SET @sql_orderby = ' p.[Name] ASC'
	ELSE IF @OrderBy = 6 /* Name: Z to A */
		SET @sql_orderby = ' p.[Name] DESC'
	ELSE IF @OrderBy = 10 /* Price: Low to High */
		SET @sql_orderby = ' p.[Price] ASC'
	ELSE IF @OrderBy = 11 /* Price: High to Low */
		SET @sql_orderby = ' p.[Price] DESC'
	ELSE IF @OrderBy = 15 /* creation date */
		SET @sql_orderby = ' p.[CreatedOnUtc] DESC'
	ELSE /* default sorting, 0 (position) */
	BEGIN
		--category position (display order)
		IF @CategoryIdsCount > 0 SET @sql_orderby = ' pcm.DisplayOrder ASC'
		
		--manufacturer position (display order)
		IF @ManufacturerId > 0
		BEGIN
			IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
			SET @sql_orderby = @sql_orderby + ' pmm.DisplayOrder ASC'
		END
		
		--name
		IF LEN(@sql_orderby) > 0 SET @sql_orderby = @sql_orderby + ', '
		SET @sql_orderby = @sql_orderby + ' p.[Name] ASC'
	END
	
	SET @sql = @sql + '
	ORDER BY' + @sql_orderby
	
    SET @sql = '
    INSERT INTO #DisplayOrderTmp ([ProductId])' + @sql

	--PRINT (@sql)
	EXEC sp_executesql @sql

	DROP TABLE #FilteredCategoryIds
	DROP TABLE #FilteredSpecs
    DROP TABLE #FilteredSpecsWithAttributes
	DROP TABLE #FiltereduserRoleIds
	DROP TABLE #KeywordProducts

	CREATE TABLE #PageIndex 
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int NOT NULL
	)
	INSERT INTO #PageIndex ([ProductId])
	SELECT ProductId
	FROM #DisplayOrderTmp
	GROUP BY ProductId
	ORDER BY min([Id])

	--total records
	SET @TotalRecords = @@rowcount
	
	DROP TABLE #DisplayOrderTmp

	--return products
	SELECT TOP (@RowsToReturn)
		p.*
	FROM
		#PageIndex [pi]
		INNER JOIN Product p with (NOLOCK) on p.Id = [pi].[ProductId]
	WHERE
		[pi].IndexId > @PageLowerBound AND 
		[pi].IndexId < @PageUpperBound
	ORDER BY
		[pi].IndexId
	
	DROP TABLE #PageIndex
END
GO

CREATE PROCEDURE [ProductTagCountLoadAll]
(
	@StoreId int,
	@AlloweduserRoleIds	nvarchar(MAX) = null	--a list of user role IDs (comma-separated list) for which a product should be shown (if a subject to ACL)
)
AS
BEGIN
	SET NOCOUNT ON
		
	--filter by user role IDs (access control list)
	SET @AlloweduserRoleIds = isnull(@AlloweduserRoleIds, '')	
	CREATE TABLE #FiltereduserRoleIds
	(
		UserRoleId int not null
	)
		
	INSERT INTO #FiltereduserRoleIds (UserRoleId)
	SELECT CAST(data as int) FROM [nop_splitstring_to_table](@AlloweduserRoleIds, ',')
	DECLARE @FiltereduserRoleIdsCount int	
	SET @FiltereduserRoleIdsCount = (SELECT COUNT(1) FROM #FiltereduserRoleIds)
	
	SELECT pt.Id as [ProductTaGenreId], COUNT(p.Id) as [ProductCount]
	FROM ProductTag pt with (NOLOCK)
	LEFT JOIN Product_ProductTag_Mapping pptm with (NOLOCK) ON pt.[Id] = pptm.[ProductTag_Id]
	LEFT JOIN Product p with (NOLOCK) ON pptm.[Product_Id] = p.[Id]
	WHERE
		p.[Deleted] = 0
		AND p.Published = 1
		AND (@StoreId = 0 or (p.LimitedToStores = 0 OR EXISTS (
			SELECT 1 FROM [StoreMapping] sm with (NOLOCK)
			WHERE [sm].EntityId = p.Id AND [sm].EntityName = 'Product' and [sm].StoreId=@StoreId
			)))
		AND (@FiltereduserRoleIdsCount = 0 or (p.SubjectToAcl = 0 OR EXISTS (
			SELECT 1 FROM #FiltereduserRoleIds [fcr]
			WHERE
				[fcr].UserRoleId IN (
					SELECT [acl].UserRoleId
					FROM [AclRecord] acl with (NOLOCK)
					WHERE [acl].EntityId = p.Id AND [acl].EntityName = 'Product'
				))
			))
	GROUP BY pt.Id
	ORDER BY pt.Id
END
GO

CREATE PROCEDURE [FullText_IsSupported]
AS
BEGIN	
	EXEC('
	SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END as Value')
END
GO



CREATE PROCEDURE [FullText_Enable]
AS
BEGIN
	--create catalog
	EXEC('
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		CREATE FULLTEXT CATALOG [nopCommerceFullTextCatalog] AS DEFAULT')

	DECLARE @SQL nvarchar(500);
	DECLARE @index_name nvarchar(1000)
	DECLARE @ParmDefinition nvarchar(500);

	SELECT @SQL = N'SELECT @index_name_out = i.name FROM sys.tables AS tbl INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id) WHERE (i.is_unique=1 and i.is_disabled=0) and (tbl.name=@table_name)'
	SELECT @ParmDefinition = N'@table_name varchar(100), @index_name_out nvarchar(1000) OUTPUT'

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'Product', @index_name_out=@index_name OUTPUT
	
	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		CREATE FULLTEXT INDEX ON [Product]([Name], [ShortDescription], [FullDescription])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'LocalizedProperty', @index_name_out=@index_name OUTPUT
	
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	EXEC sp_executesql @SQL, @ParmDefinition, @table_name = 'ProductTag', @index_name_out=@index_name OUTPUT

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		CREATE FULLTEXT INDEX ON [ProductTag]([Name])
		KEY INDEX [' + @index_name +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO



CREATE PROCEDURE [FullText_Disable]
AS
BEGIN
	EXEC('
	--drop indexes
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		DROP FULLTEXT INDEX ON [Product]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		DROP FULLTEXT INDEX ON [LocalizedProperty]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		DROP FULLTEXT INDEX ON [ProductTag]
	')

	--drop catalog
	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		DROP FULLTEXT CATALOG [nopCommerceFullTextCatalog]
	')
END
GO


CREATE PROCEDURE [LanguagePackImport]
(
	@LanguageId int,
	@XmlPackage xml,
	@UpdateExistingResources bit
)
AS
BEGIN
	IF EXISTS(SELECT * FROM [Language] WHERE [Id] = @LanguageId)
	BEGIN
		CREATE TABLE #LocaleStringResourceTmp
			(
				[LanguageId] [int] NOT NULL,
				[ResourceName] [nvarchar](200) NOT NULL,
				[ResourceValue] [nvarchar](MAX) NOT NULL
			)

		INSERT INTO #LocaleStringResourceTmp (LanguageId, ResourceName, ResourceValue)
		SELECT	@LanguageId, nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageId, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName))
			BEGIN
				IF (@UpdateExistingResources = 1)
				BEGIN
					UPDATE [LocaleStringResource]
					SET [ResourceValue]=@ResourceValue
					WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName
				END
			END
			ELSE 
			BEGIN
				INSERT INTO [LocaleStringResource]
				(
					[LanguageId],
					[ResourceName],
					[ResourceValue]
				)
				VALUES
				(
					@LanguageId,
					@ResourceName,
					@ResourceValue
				)
			END
			
			
			FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
			END
		CLOSE cur_localeresource
		DEALLOCATE cur_localeresource

		DROP TABLE #LocaleStringResourceTmp
	END
END
GO


CREATE PROCEDURE [DeleteGuests]
(
	@OnlyWithoutShoppingCart bit = 1,
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (UserId int)
	CREATE TABLE #tmp_adresses (AddressId int)
		
	INSERT #tmp_guests (UserId)
	SELECT c.[Id] 
	FROM [User] c with (NOLOCK)
		LEFT JOIN [ShoppingCartItem] sci with (NOLOCK) ON sci.[UserId] = c.[Id]
		INNER JOIN (
			--guests only
			SELECT ccrm.[User_Id] 
			FROM [User_UserRole_Mapping] ccrm with (NOLOCK)
				INNER JOIN [UserRole] cr with (NOLOCK) ON cr.[Id] = ccrm.[UserRole_Id]
			WHERE cr.[SystemName] = N'Guests'
		) g ON g.[User_Id] = c.[Id]
		LEFT JOIN [Order] o with (NOLOCK) ON o.[UserId] = c.[Id]
		LEFT JOIN [BlogComment] bc with (NOLOCK) ON bc.[UserId] = c.[Id]
		LEFT JOIN [NewsComment] nc with (NOLOCK) ON nc.[UserId] = c.[Id]
		LEFT JOIN [ProductReview] pr with (NOLOCK) ON pr.[UserId] = c.[Id]
		LEFT JOIN [ProductReviewHelpfulness] prh with (NOLOCK) ON prh.[UserId] = c.[Id]
		LEFT JOIN [PollVotingRecord] pvr with (NOLOCK) ON pvr.[UserId] = c.[Id]
		LEFT JOIN [Forums_Topic] ft with (NOLOCK) ON ft.[UserId] = c.[Id]
		LEFT JOIN [Forums_Post] fp with (NOLOCK) ON fp.[UserId] = c.[Id]
	WHERE 1 = 1
		--no orders
		AND (o.Id is null)
		--no blog comments
		AND (bc.Id is null)
		--no news comments
		AND (nc.Id is null)
		--no product reviews
		AND (pr.Id is null)
		--no product reviews helpfulness
		AND (prh.Id is null)
		--no poll voting
		AND (pvr.Id is null)
		--no forum topics
		AND (ft.Id is null)
		--no forum topics
		AND (fp.Id is null)
		--no system accounts
		AND (c.IsSystemAccount = 0)
		--created from
		AND ((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
		--created to
		AND ((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
		--shopping cart items
		AND ((@OnlyWithoutShoppingCart = 0) OR (sci.Id is null))

	INSERT #tmp_adresses (AddressId)
	SELECT [Address_Id] FROM [UserAddresses] WHERE [User_Id] IN (SELECT [UserId] FROM #tmp_guests)

	--delete guests
	DELETE [User]
	WHERE [Id] IN (SELECT [UserId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [UserId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'User')

	--delete addresses
	DELETE [Address]
	WHERE [Id] IN (SELECT [AddressId] FROM #tmp_adresses)
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
	DROP TABLE #tmp_adresses
END
GO


CREATE PROCEDURE [CategoryLoadAllPaged]
(
    @ShowHidden         BIT = 0,
    @Name               NVARCHAR(MAX) = NULL,
    @StoreId            INT = 0,
    @userRoleIds	NVARCHAR(MAX) = NULL,
    @PageIndex			INT = 0,
	@PageSize			INT = 2147483644,
    @TotalRecords		INT = NULL OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

    --filter by user role IDs (access control list)
	SET @userRoleIds = ISNULL(@userRoleIds, '')
	CREATE TABLE #FiltereduserRoleIds
	(
		UserRoleId INT NOT NULL
	)
	INSERT INTO #FiltereduserRoleIds (UserRoleId)
	SELECT CAST(data AS INT) FROM [nop_splitstring_to_table](@userRoleIds, ',')
	DECLARE @FiltereduserRoleIdsCount INT = (SELECT COUNT(1) FROM #FiltereduserRoleIds)

    --ordered categories
    CREATE TABLE #OrderedCategoryIds
	(
		[Id] int IDENTITY (1, 1) NOT NULL,
		[CategoryId] int NOT NULL
	)
    
    --get max length of DisplayOrder and Id columns (used for padding Order column)
    DECLARE @lengthId INT = (SELECT LEN(MAX(Id)) FROM [Category])
    DECLARE @lengthOrder INT = (SELECT LEN(MAX(DisplayOrder)) FROM [Category])

    --get category tree
    ;WITH [CategoryTree]
    AS (SELECT [Category].[Id] AS [Id], 
		(select RIGHT(REPLICATE('0', @lengthOrder)+ RTRIM(CAST([Category].[DisplayOrder] AS NVARCHAR(MAX))), @lengthOrder)) + '-' + (select RIGHT(REPLICATE('0', @lengthId)+ RTRIM(CAST([Category].[Id] AS NVARCHAR(MAX))), @lengthId))  AS [Order]
        FROM [Category] WHERE [Category].[ParentCategoryId] = 0
        UNION ALL
        SELECT [Category].[Id] AS [Id], 
		[CategoryTree].[Order] + '|' + (select RIGHT(REPLICATE('0', @lengthOrder)+ RTRIM(CAST([Category].[DisplayOrder] AS NVARCHAR(MAX))), @lengthOrder)) + '-' + (select RIGHT(REPLICATE('0', @lengthId)+ RTRIM(CAST([Category].[Id] AS NVARCHAR(MAX))), @lengthId))  AS [Order]
        FROM [Category]
        INNER JOIN [CategoryTree] ON [CategoryTree].[Id] = [Category].[ParentCategoryId])
    INSERT INTO #OrderedCategoryIds ([CategoryId])
    SELECT [Category].[Id]
    FROM [CategoryTree]
    RIGHT JOIN [Category] ON [CategoryTree].[Id] = [Category].[Id]

    --filter results
    WHERE [Category].[Deleted] = 0
    AND (@ShowHidden = 1 OR [Category].[Published] = 1)
    AND (@Name IS NULL OR @Name = '' OR [Category].[Name] LIKE ('%' + @Name + '%'))
    AND (@ShowHidden = 1 OR @FiltereduserRoleIdsCount  = 0 OR [Category].[SubjectToAcl] = 0
        OR EXISTS (SELECT 1 FROM #FiltereduserRoleIds [roles] WHERE [roles].[UserRoleId] IN
            (SELECT [acl].[UserRoleId] FROM [AclRecord] acl WITH (NOLOCK) WHERE [acl].[EntityId] = [Category].[Id] AND [acl].[EntityName] = 'Category')
        )
    )
    AND (@StoreId = 0 OR [Category].[LimitedToStores] = 0
        OR EXISTS (SELECT 1 FROM [StoreMapping] sm WITH (NOLOCK)
			WHERE [sm].[EntityId] = [Category].[Id] AND [sm].[EntityName] = 'Category' AND [sm].[StoreId] = @StoreId
		)
    )
    ORDER BY ISNULL([CategoryTree].[Order], 1)

    --total records
    SET @TotalRecords = @@ROWCOUNT

    --paging
    SELECT [Category].* FROM #OrderedCategoryIds AS [Result] INNER JOIN [Category] ON [Result].[CategoryId] = [Category].[Id]
    WHERE ([Result].[Id] > @PageSize * @PageIndex AND [Result].[Id] <= @PageSize * (@PageIndex + 1))
    ORDER BY [Result].[Id]

    DROP TABLE #FiltereduserRoleIds
    DROP TABLE #OrderedCategoryIds
END
GO