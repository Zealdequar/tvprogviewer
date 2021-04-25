/****** Script for SelectTopNRows command from SSMS  ******/
UPDATE EmailAccount SET DisplayName = 'TVProgViewer'
, Email = 'tvprogviewer@gmail.com'
, Host = 'smtp.gmail.com'
, Username = 'TVProgViewer'
, Password = ''
, Port = 465
, EnableSsl = 1
, FriendlyName = 'TVProgViewer@gmail.com (TVProgViewer)'
WHERE Id = 1

SELECT TOP (1000) [Id]
      ,[DisplayName]
      ,[Email]
      ,[Host]
      ,[Username]
      ,[Password]
      ,[Port]
      ,[EnableSsl]
      ,[UseDefaultCredentials]
      ,[FriendlyName]
  FROM [TVProgDb].[dbo].[EmailAccount]