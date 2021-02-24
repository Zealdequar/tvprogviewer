
UPDATE TVProgCoreDb.dbo.[User] SET LastName = (SELECT TOP 1 LastName FROM TVProgBase.dbo.SystemUsers WHERE UserId = 1),
FirstName = (SELECT TOP 1 FirstName FROM TVProgBase.dbo.SystemUsers WHERE UserId = 1),
MiddleName = (SELECT TOP 1 MiddleName FROM TVProgBase.dbo.SystemUsers WHERE UserId = 1),
BirthDate = (SELECT TOP 1 BirthDate FROM TVProgBase.dbo.SystemUsers WHERE UserId = 1)
WHERE Id = 1

SET IDENTITY_INSERT TVProgCoreDb.dbo.MediaPic ON;
INSERT INTO TVProgCoreDb.dbo.MediaPic (Id, Path25, PathOrig, IsSystem, Length25, Length, ContentCoding, ContentType, FileName)
SELECT IconID,
Path25,
PathOrig,
IsSystem,
Length25,
Length,
ContentCoding,
ContentType,
FileName
FROM TVProgBase.dbo.MediaPic
SET IDENTITY_INSERT TVProgCoreDb.dbo.MediaPic OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.TvProgProviders ON;
INSERT INTO TVProgCoreDb.dbo.TvProgProviders (Id, Rss, ContactEmail, ContactName, ProviderWebSite, ProviderName)
SELECT TVProgProviderID,
 Rss,
 ContactEmail,
 ContactName,
 ProviderWebSite,
 ProviderName
FROM TVProgBase.dbo.TVProgProviders
SET IDENTITY_INSERT TVProgCoreDb.dbo.TvProgProviders OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.ExtUserSettings ON;
INSERT INTO TVProgCoreDb.dbo.ExtUserSettings (Id, UserId, TvProgProviderId, UncheckedChannels)
SELECT ExtUserSettingsID,
UID,
TVProgProviderID,
UncheckedChannels
FROM TVProgBase.dbo.ExtUserSettings
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.ExtUserSettings OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.Channels ON;
INSERT INTO TVProgCoreDb.dbo.Channels (Id, TvProgProviderId, InternalId, IconId, CreateDate, TitleChannel, IconWebSrc, Deleted) 
SELECT ChannelID as Id
, TVProgProviderID as TvProgProviderId
, InternalId
, IconId
, CreateDate
, TitleChannel
, IconWebSrc
, Deleted 
FROM TVProgBase.dbo.Channels
WHERE ChannelID NOT IN (SELECT ID FROM TVProgCoreDb.dbo.Channels);
SET IDENTITY_INSERT TVProgCoreDb.dbo.Channels OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.Genres ON;
INSERT INTO TVProgCoreDb.dbo.Genres (Id, UserId, IconId, CreateDate, GenreName, Visible, DeleteDate)
SELECT GenreID,
UID,
IconID,
CreateDate,
GenreName,
Visible,
DeleteDate
FROM TVProgBase.dbo.Genres
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.Genres OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.GenreClassificator ON;
INSERT INTO TVProgCoreDb.dbo.GenreClassificator(Id, GenreId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate)
SELECT GenreClassificatorID,
GID,
UID,
ContainPhrases,
NonContainPhrases,
OrderCol,
DeleteAfterDate
FROM TVProgBase.dbo.GenreClassificator
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.GenreClassificator OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.Ratings ON;
INSERT INTO TVProgCoreDb.dbo.Ratings(Id, UserId, IconId, CreateDate, RatingName, Visible, DeleteDate)
SELECT RatingID,
UID, 
IconID,
CreateDate,
RatingName,
Visible,
DeleteDate
FROM TVProgBase.dbo.Ratings
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.Ratings OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.RatingClassificator ON;
INSERT INTO TVProgCoreDb.dbo.RatingClassificator (Id, RatingId, UserId, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate)
SELECT RatingClassificatorID,
RID,
UID,
ContainPhrases,
NonContainPhrases,
OrderCol,
DeleteAfterDate
FROM TvProgBase.dbo.RatingClassificator
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.RatingClassificator OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.TypeProg ON;
INSERT INTO TVProgCoreDb.dbo.TypeProg(Id, TvProgProviderId, TypeName, FileFormat)
SELECT TypeProgID,
TVProgProviderID,
TypeName,
FileFormat
FROM TVProgBase.dbo.TypeProg;
SET IDENTITY_INSERT TVProgCoreDb.dbo.TypeProg OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.WebResources ON;
INSERT INTO TVProgCoreDb.dbo.WebResources(Id, TypeProgId, FileName, ResourceName, ResourceUrl)
SELECT WebResourceID,
TPID,
FileName,
ResourceName,
ResourceUrl
FROM TVProgBase.dbo.WebResources;
SET IDENTITY_INSERT TVProgCoreDb.dbo.WebResources OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.Programmes ON;
INSERT INTO TVProgCoreDb.dbo.Programmes(Id, TypeProgId, ChannelId, InternalChanId, TsStart, TsStop, TsStartMo, TsStopMo, Title, Descr, Category)
SELECT ProgrammesID,
TID,
CID,
InternalChanID,
TsStart,
TsStop,
TsStartMo,
TsStopMo,
Title,
Descr,
Category
FROM TVProgBase.dbo.Programmes
WHERE ProgrammesID NOT IN (SELECT Id FROM TVProgCoreDb.dbo.Programmes);
SET IDENTITY_INSERT TVProgCoreDb.dbo.Programmes OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.SearchSettings ON;
INSERT INTO TVProgCoreDb.dbo.SearchSettings(Id, UserId, LoadSettings, Match, NotMatch, InAnons, TsFinalFrom, TsFinalTo, TrackBarFrom, TrackBarTo)
SELECT SearchSettingsID,
UID,
LoadSettings,
Match,
NotMatch,
InAnons,
TsFinalFrom,
TsFinalTo,
TrackBarFrom,
TrackBarTo
FROM TVProgBase.dbo.SearchSettings
WHERE UID = 1
SET IDENTITY_INSERT TVProgCoreDb.dbo.SearchSettings OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.UpdateProgLog ON;
INSERT INTO TVProgCoreDb.dbo.UpdateProgLog(Id, WebResourceId, TsUpdateStart, TsUpdateEnd, UdtElapsedSec, MinProgDate, MaxProgDate, QtyChans, QtyProgrammes,
                QtyNewChans, QtyNewProgrammes, IsSuccess, ErrorMessage)
SELECT UpdateProgLogID,
WRID,
TsUpdateStart,
TsUpdateEnd,
UdtElapsedSec,
MinProgDate,
MaxProgDate,
QtyChans,
QtyProgrammes,
QtyNewChans,
QtyNewProgrammes,
IsSuccess,
ErrorMessage
FROM TVProgBase.dbo.UpdateProgLog
SET IDENTITY_INSERT TVProgCoreDb.dbo.UpdateProgLog OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.UserChannels ON;
INSERT INTO TVProgCoreDb.dbo.UserChannels(Id, UserId, ChannelId, IconId, DisplayName, OrderCol)
SELECT UserChannelID,
UID,
CID,
IconId,
DisplayName,
OrderCol
FROM TVProgBase.dbo.UserChannels
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.UserChannels OFF;

SET IDENTITY_INSERT TVProgCoreDb.dbo.UsersPrograms ON;
INSERT INTO TVProgCoreDb.dbo.UsersPrograms(Id, UserId, UserChannelId, ProgrammesId, GenreId, RatingId, Anons, Remind)
SELECT UserProgramsID,
UID,
UCID,
PID,
GID,
RID,
Anons,
Remind
FROM TVProgBase.dbo.UsersPrograms
WHERE UID = 1 OR UID IS NULL;
SET IDENTITY_INSERT TVProgCoreDb.dbo.UsersPrograms OFF;

UPDATE TVProgCoreDb.dbo.MediaPic SET Path25 = replace(Path25, 'imgs', 'images')
                                   , PathOrig = replace(PathOrig, 'imgs', 'images');
GO

UPDATE LocaleStringResource SET ResourceValue = 'Сообщите о нас'
WHERE ResourceName = 'footer.followus'
GO

INSERT INTO LocaleStringResource (ResourceName, ResourceValue, LanguageId)
VALUES ('footer.userservice', 'Сервисы', 2)
GO

INSERT INTO LocaleStringResource (ResourceName, ResourceValue, LanguageId)
VALUES ('account.login.newuser', 'Новый пользователь', 2)
GO
