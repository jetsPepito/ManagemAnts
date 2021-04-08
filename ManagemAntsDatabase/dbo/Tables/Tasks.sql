CREATE TABLE [dbo].[Tasks] (
    [id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [name]        VARCHAR (200) NOT NULL,
    [description] TEXT          NULL,
    [created_at]  DATE          NOT NULL,
    [duration]    INT           NOT NULL,
    [state]       INT           NOT NULL,
    [time_spent]  INT           NULL,
    [project_id]  BIGINT        NOT NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Tasks_Projects] FOREIGN KEY ([project_id]) REFERENCES [dbo].[Projects] ([id])
);

