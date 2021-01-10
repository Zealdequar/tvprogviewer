DELETE FROM [TVProgDb].[dbo].[Language]
SET IDENTITY_INSERT [dbo].[Language] ON 
GO
INSERT [dbo].[Language] ([EntityCacheKey], [Id], [DisplayOrder], [Published], [DefaultCurrencyId], [LimitedToStores], [Rtl], [FlagImageFileName], [UniqueSeoCode], [LanguageCulture], [Name]) VALUES (N'TVProgViewer.Language.id-0', 1, 1, 1, 0, 0, 0, N'us.png', N'en', N'en-US', N'EN')
GO
INSERT [dbo].[Language] ([EntityCacheKey], [Id], [DisplayOrder], [Published], [DefaultCurrencyId], [LimitedToStores], [Rtl], [FlagImageFileName], [UniqueSeoCode], [LanguageCulture], [Name]) VALUES (N'TVProgViewer.Language.id-0', 2, 0, 1, 0, 0, 0, N'ru.png', N'ru', N'ru-RU', N'RU')
GO
SET IDENTITY_INSERT [dbo].[Language] OFF
GO
/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
SELECT TOP (1000) [EntityCacheKey]
      ,[Id]
      ,[DisplayOrder]
      ,[Published]
      ,[DefaultCurrencyId]
      ,[LimitedToStores]
      ,[Rtl]
      ,[FlagImageFileName]
      ,[UniqueSeoCode]
      ,[LanguageCulture]
      ,[Name]
FROM [TVProgDb].[dbo].[Language] 
