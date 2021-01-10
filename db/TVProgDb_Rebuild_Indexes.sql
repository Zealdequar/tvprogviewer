USE [TVProgCoreDb]
GO

/****** Object:  Index [IX_Deleted]    Script Date: 03.07.2019 22:49:21 ******/
ALTER INDEX IX_Channels_Deleted ON [dbo].[Channels] REBUILD
GO

/****** Object:  Index [IX_InternalID]    Script Date: 03.07.2019 22:50:54 ******/
ALTER INDEX [IX_Channels_InternalID] ON [dbo].[Channels] REBUILD
GO

/****** Object:  Index [IX_IconID]    Script Date: 03.07.2019 22:52:47 ******/
ALTER INDEX [IX_Genres_IconID] ON [dbo].[Genres] REBUILD
GO

/****** Object:  Index [IX_Category]    Script Date: 03.07.2019 22:54:06 ******/
ALTER INDEX [IX_Programmes_Category] ON [dbo].[Programmes] REBUILD
GO

/****** Object:  Index [IX_InternalChanID]    Script Date: 03.07.2019 22:54:37 ******/
ALTER INDEX [IX_Programmes_InternalChanId] ON [dbo].[Programmes] REBUILD
GO

/****** Object:  Index [IX_Title]    Script Date: 03.07.2019 22:55:12 ******/
ALTER INDEX [IX_Programmes_Title] ON [dbo].[Programmes] REBUILD
GO

/****** Object:  Index [IX_TsStartMO]    Script Date: 03.07.2019 22:55:23 ******/
ALTER INDEX [IX_Programmes_TsStartMo] ON [dbo].[Programmes] REBUILD
GO

/****** Object:  Index [IX_TsStopMO]    Script Date: 03.07.2019 22:55:39 ******/
ALTER INDEX [IX_Programmes_TsStopMo] ON [dbo].[Programmes] REBUILD
GO

/****** Object:  Index [IX_GID]    Script Date: 03.07.2019 22:57:48 ******/
ALTER INDEX [IX_UsersPrograms_GenreId] ON [dbo].[UsersPrograms] REBUILD
GO


/****** Object:  Index [IX_PID]    Script Date: 03.07.2019 22:58:01 ******/
ALTER INDEX [IX_UsersPrograms_ProgrammesId] ON [dbo].[UsersPrograms] REBUILD
GO

/****** Object:  Index [IX_RID]    Script Date: 03.07.2019 22:58:13 ******/
ALTER INDEX [IX_UsersPrograms_RatingId] ON [dbo].[UsersPrograms] REBUILD
GO

/****** Object:  Index [IX_UCID]    Script Date: 03.07.2019 22:58:25 ******/
ALTER INDEX [IX_UsersPrograms_UserChannelId] ON [dbo].[UsersPrograms] REBUILD
GO

/****** Object:  Index [IX_UID]    Script Date: 03.07.2019 22:58:37 ******/
ALTER INDEX [IX_UsersPrograms_UserId] ON [dbo].[UsersPrograms] REBUILD
GO