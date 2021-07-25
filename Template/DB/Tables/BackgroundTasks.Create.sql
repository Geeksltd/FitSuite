-- BackgroundTasks Table ========================
CREATE TABLE BackgroundTasks (
    Id uniqueidentifier PRIMARY KEY NONCLUSTERED,
    Name nvarchar(200)  NOT NULL,
    ExecutingInstance uniqueidentifier  NULL,
    Heartbeat datetime2  NULL,
    LastExecuted datetime2  NULL,
    IntervalInMinutes int  NOT NULL,
    TimeoutInMinutes int  NOT NULL
);

