USE [TVProgDb]
GO

ALTER TABLE [dbo].[Channels] ADD  DEFAULT (sysdatetimeoffset()) FOR [CreateDate]
GO

