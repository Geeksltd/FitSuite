-- UserInfos Table ========================
IF NOT EXISTS(SELECT * FROM sys.schemas WHERE name = 'PeopleService')
BEGIN
EXEC('CREATE SCHEMA PeopleService')
END

CREATE TABLE PeopleService.UserInfos (
    [.Deleted] bit NOT NULL,
    Id uniqueidentifier PRIMARY KEY NONCLUSTERED,
    Email nvarchar(100)  NULL,
    DisplayName nvarchar(MAX)  NULL,
    ImageUrl nvarchar(MAX)  NULL,
    IsActive bit  NOT NULL,
    Roles nvarchar(MAX)  NULL
);
CREATE INDEX [IX_UserInfos->Soft_Delete] ON PeopleService.UserInfos ([.Deleted]);

GO

