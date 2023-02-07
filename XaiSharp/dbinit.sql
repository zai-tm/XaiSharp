CREATE TABLE [Tags] (
    [Id] INTEGER NOT NULL PRIMARY KEY,
    [Name] INT NOT NULL,
    [Content] INT NOT NULL,
    [UserId] UNSIGNED BIG INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL
);


CREATE TABLE [RepostedMessages] (
    [MessageId] UNSIGNED BIG INT NOT NULL
);
