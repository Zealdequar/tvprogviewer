INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Fields.AcceptPersonalDataAgreement.Required'), 'Для регистрации необходимо согласиться с условиями обработки персональных данных', 2); 
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Fields.AcceptPersonalDataAgreement'), 'Я согласен(согласна) на обработку моих персональных данных', 2);
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Admin.Users.Users.Fields.AcceptPersonalDataAgreement'), 'Согласие на обработку персональных данных', 2); 
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Field.AcceptPersonalDataAgreementDescription'),'В соответствии со статьей 9 ФЗ-152 от 27 июля 2006 года «О персональных данных», даю согласие на обработку в документальной и/или электронной форме моих персональных данных сайту TVProgViewer.Ru', 2);
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Admin.Configuration.Settings.UserUser.AcceptPersonalDataAgreementEnabled'), '''Согласие на обработку персональных данных'' включено', 2);
GO

UPDATE LocaleStringResource SET ResourceName = replace(ResourceName, 'customer', 'user'); 
GO

INSERT INTO [dbo].[Setting] (Name, Value, StoreId)
VALUES (LOWER('UserSettings.AcceptPersonalDataAgreementEnabled'), 'True', 0);
GO

INSERT INTO [dbo].[Setting] (Name, Value, StoreId)
VALUES (LOWER('UserSettings.AcceptPersonalDataAgreementRequired'), 'True', 0);
GO

SELECT * FROM [dbo].[Setting]
WHERE Name IN ('UserSettings.AcceptPersonalDataAgreementEnabled', 'UserSettings.AcceptPersonalDataAgreementRequired')

SELECT * FROM [dbo].[LocaleStringResource]
WHERE ResourceName  IN ('Account.Fields.AcceptPersonalDataAgreement.Required'
, 'Account.Fields.AcceptPersonalDataAgreement'
, 'Admin.Users.Users.Fields.AcceptPersonalDataAgreement'
, 'Account.Field.AcceptPersonalDataAgreementDescription'
, 'Admin.Configuration.Settings.UserUser.AcceptPersonalDataAgreementEnabled')