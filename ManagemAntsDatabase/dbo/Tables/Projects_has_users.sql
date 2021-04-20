CREATE TABLE [dbo].[Projects_has_users] (
    [id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [project_id] BIGINT NOT NULL,
    [user_id]    BIGINT NOT NULL,
    [role]       INT    NOT NULL,
    CONSTRAINT [PK_Projects_has_users] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Projects_has_users_Projects1] FOREIGN KEY ([project_id]) REFERENCES [dbo].[Projects] ([id]),
    CONSTRAINT [FK_Projects_has_users_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([id])
);






GO
CREATE NONCLUSTERED INDEX [IX_Projects_has_users_user_id]
    ON [dbo].[Projects_has_users]([user_id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Projects_has_users_project_id]
    ON [dbo].[Projects_has_users]([project_id] ASC);

