CREATE TABLE [dbo].[Projects] (
    [id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [name]        VARCHAR (100)  NOT NULL,
    [description] VARCHAR (5000) NULL,
    [owner]       BIGINT         NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Projects_Users] FOREIGN KEY ([owner]) REFERENCES [dbo].[Users] ([id])
);





