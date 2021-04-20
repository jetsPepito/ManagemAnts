CREATE TABLE [dbo].[Users] (
    [id]        BIGINT       IDENTITY (1, 1) NOT NULL,
    [firstname] VARCHAR (50) NOT NULL,
    [lastname]  VARCHAR (50) NOT NULL,
    [pseudo]    VARCHAR (50) NOT NULL,
    [password]  TEXT         NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([id] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users]
    ON [dbo].[Users]([pseudo] ASC);

