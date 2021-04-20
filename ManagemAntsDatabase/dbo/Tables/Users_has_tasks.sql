CREATE TABLE [dbo].[Users_has_tasks] (
    [id]      BIGINT IDENTITY (1, 1) NOT NULL,
    [user_id] BIGINT NOT NULL,
    [task_id] BIGINT NOT NULL,
    CONSTRAINT [PK_Users_has_tasks] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Users_has_tasks_Tasks] FOREIGN KEY ([task_id]) REFERENCES [dbo].[Tasks] ([id]),
    CONSTRAINT [FK_Users_has_tasks_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([id])
);

