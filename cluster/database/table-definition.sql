CREATE TABLE [dbo].[Users] (
    [Id]                                 NVARCHAR (65)  NOT NULL,
    [User]                               NVARCHAR (256) NOT NULL,
    [PasswordHash]                       NVARCHAR (256) NOT NULL,
    [Role]                               NVARCHAR (256) NOT NULL,
    [FitbitAuthenticationResponseAsJson] NVARCHAR (MAX) NULL
);