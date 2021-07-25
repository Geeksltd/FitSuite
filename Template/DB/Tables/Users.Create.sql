-- Users Table ========================
CREATE TABLE Users (
    [.Deleted] bit NOT NULL,
    Id uniqueidentifier PRIMARY KEY NONCLUSTERED,
    Email nvarchar(200)  NULL,
    Info uniqueidentifier  NULL
);
CREATE INDEX [IX_Users->Soft_Delete] ON Users ([.Deleted]);

GO

