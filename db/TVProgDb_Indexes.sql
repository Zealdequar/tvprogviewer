USE [TVProgDb]
GO

/****** Object:  Index [IX_Deleted]    Script Date: 03.07.2019 22:49:21 ******/
CREATE NONCLUSTERED INDEX [IX_Channels_Deleted] ON [dbo].[Channels]
(
	[Deleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_IconID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Channels_IconID] ON [dbo].[Channels]
(
	[IconID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_InternalID]    Script Date: 03.07.2019 22:50:54 ******/
CREATE NONCLUSTERED INDEX [IX_Channels_InternalID] ON [dbo].[Channels]
(
	[InternalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_TVProgProviderID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Channels_TVProgProviderID] ON [dbo].[Channels]
(
	[TVProgProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_GID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_GenreClassificator_GenreId] ON [dbo].[GenreClassificator]
(
	[GenreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_GenreClassificator_UserId]    Script Date: 10.01.2021 17:58:38 ******/
CREATE NONCLUSTERED INDEX [IX_GenreClassificator_UserId] ON [dbo].[GenreClassificator]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_IconID]    Script Date: 03.07.2019 22:52:47 ******/
CREATE NONCLUSTERED INDEX [IX_Genres_IconID] ON [dbo].[Genres]
(
	[IconId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Genres_UserId] ON [dbo].[Genres]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [_dta_index_MediaPic_5_1045578763__K2_K1_9]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [_dta_index_MediaPic_5_1045578763__K2_K1_9] ON [dbo].[MediaPic]
(
	[FileName] ASC,
	[Id] ASC
)
INCLUDE([Path25]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_Category]    Script Date: 03.07.2019 22:54:06 ******/
CREATE NONCLUSTERED INDEX [IX_Programmes_Category] ON [dbo].[Programmes]
(
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_CID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Programmes_ChannelId] ON [dbo].[Programmes]
(
	[ChannelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_InternalChanID]    Script Date: 03.07.2019 22:54:37 ******/
CREATE NONCLUSTERED INDEX [IX_Programmes_InternalChanId] ON [dbo].[Programmes]
(
	[InternalChanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_TID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Programmes_TypeProgId] ON [dbo].[Programmes]
(
	[TypeProgId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_Title]    Script Date: 03.07.2019 22:55:12 ******/
CREATE NONCLUSTERED INDEX [IX_Programmes_Title] ON [dbo].[Programmes]
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_RID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_RatingClassificator_RatingId] ON [dbo].[RatingClassificator]
(
	[RatingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_RatingClassificator_UserId] ON [dbo].[RatingClassificator]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_IconID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Ratings_IconId] ON [dbo].[Ratings]
(
	[IconId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UID]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED INDEX [IX_Ratings_UserId] ON [dbo].[Ratings]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_GID]    Script Date: 03.07.2019 22:57:48 ******/
CREATE NONCLUSTERED INDEX [IX_UsersPrograms_GenreId] ON [dbo].[UsersPrograms]
(
	[GenreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/****** Object:  Index [IX_PID]    Script Date: 03.07.2019 22:58:01 ******/
CREATE NONCLUSTERED INDEX [IX_UsersPrograms_ProgrammesId] ON [dbo].[UsersPrograms]
(
	[ProgrammesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_RID]    Script Date: 03.07.2019 22:58:13 ******/
CREATE NONCLUSTERED INDEX [IX_UsersPrograms_RatingId] ON [dbo].[UsersPrograms]
(
	[RatingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UCID]    Script Date: 03.07.2019 22:58:25 ******/
CREATE NONCLUSTERED INDEX [IX_UsersPrograms_UserChannelId] ON [dbo].[UsersPrograms]
(
	[UserChannelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_UID]    Script Date: 03.07.2019 22:58:37 ******/
CREATE NONCLUSTERED INDEX [IX_UsersPrograms_UserId] ON [dbo].[UsersPrograms]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [_dta_index_Channels_5_1093578934__col__]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED COLUMNSTORE INDEX [_dta_index_Channels_5_1093578934__col__] ON [dbo].[Channels]
(
	[Id],
	[TVProgProviderID],
	[InternalID],
	[IconID],
	[CreateDate],
	[TitleChannel],
	[IconWebSrc],
	[Deleted]
)WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
/****** Object:  Index [_dta_index_MediaPic_5_1045578763__col__]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED COLUMNSTORE INDEX [_dta_index_MediaPic_5_1045578763__col__] ON [dbo].[MediaPic]
(
	[Id],
	[FileName],
	[ContentType],
	[ContentCoding],
	[Length],
	[Length25],
	[IsSystem],
	[PathOrig],
	[Path25]
)WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
/****** Object:  Index [_dta_index_Programmes_5_1701581100__col__]    Script Date: 10.01.2021 17:47:39 ******/
CREATE NONCLUSTERED COLUMNSTORE INDEX [_dta_index_Programmes_5_1701581100__col__] ON [dbo].[Programmes]
(
	[Id],
	[TypeProgId],
	[ChannelId],
	[InternalChanID],
	[TsStart],
	[TsStop],
	[TsStartMO],
	[TsStopMO],
	[Title],
	[Descr],
	[Category]
)WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO













