USE [TVProgBase]
GO
SET IDENTITY_INSERT [dbo].[TVProgProviders] ON 
GO
INSERT [dbo].[TVProgProviders] ([TVProgProviderID], [ProviderName], [ProviderWebSite], [ContactName], [ContactEmail], [Rss]) VALUES (1, N'ТелеГид.ИНФО', N'http://www.teleguide.info', N'', N'', N'https://www.teleguide.info/news.xml')
GO
INSERT [dbo].[TVProgProviders] ([TVProgProviderID], [ProviderName], [ProviderWebSite], [ContactName], [ContactEmail], [Rss]) VALUES (2, N'Телепрограмма Перми', N'http://tele.perm.ru', NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[TVProgProviders] OFF
GO
SET IDENTITY_INSERT [dbo].[TypeProg] ON 
GO
INSERT [dbo].[TypeProg] ([TypeProgID], [TVProgProviderID], [TypeName], [FileFormat]) VALUES (1, 1, N'Формат XMLTV', N'xml')
GO
INSERT [dbo].[TypeProg] ([TypeProgID], [TVProgProviderID], [TypeName], [FileFormat]) VALUES (2, 1, N'Формат Интер-ТВ', N'txt')
GO
INSERT [dbo].[TypeProg] ([TypeProgID], [TVProgProviderID], [TypeName], [FileFormat]) VALUES (3, 2, N'Формат XMLTV', N'xml')
GO
SET IDENTITY_INSERT [dbo].[TypeProg] OFF
GO
SET IDENTITY_INSERT [dbo].[WebResources] ON 
GO
INSERT [dbo].[WebResources] ([WebResourceID], [TPID], [FileName], [ResourceName], [ResourceUrl]) VALUES (1, 1, N'TGXmlTvOld.xml', N'Ресурс teleguide.info старой телепрограммы (XMLTV)', N'http://www.teleguide.info/download/old/xmltv.xml.gz')
GO
INSERT [dbo].[WebResources] ([WebResourceID], [TPID], [FileName], [ResourceName], [ResourceUrl]) VALUES (2, 1, N'TGXmlTvNew.xml', N'Ресурс teleguide.info новой телепрограммы (XMLTV)', N'http://teleguide.info/download/tvprogviewer_ru/xmltv/xmltv.xml.gz')
GO
INSERT [dbo].[WebResources] ([WebResourceID], [TPID], [FileName], [ResourceName], [ResourceUrl]) VALUES (3, 2, N'TGInterTVOld.txt', N'Ресурс teleguide.info старой телепрограммы (Интер-ТВ)', N'http://www.teleguide.info/download/old/inter-tv.zip')
GO
INSERT [dbo].[WebResources] ([WebResourceID], [TPID], [FileName], [ResourceName], [ResourceUrl]) VALUES (4, 2, N'TGInterTVNew.txt', N'Ресурс teleguide.info новой телепрограммы (Интер-ТВ)', N'http://teleguide.info/download/tvprogviewer_ru/inter-tv/inter-tv.zip ')
GO
INSERT [dbo].[WebResources] ([WebResourceID], [TPID], [FileName], [ResourceName], [ResourceUrl]) VALUES (6, 3, N'TPXmlTvNew.xml', N'Ресурс tele.perm.ru (XMLTV)', N'http://tele.perm.ru/download/tvguide.xml.gz')
GO
SET IDENTITY_INSERT [dbo].[WebResources] OFF
GO
