CREATE TABLE [dbo].[Projects_has_users] (
    [project_id] BIGINT NOT NULL,
    [user_id]    BIGINT NOT NULL,
    [role]       INT    NOT NULL,
    CONSTRAINT [FK_Projects_has_users_Projects] FOREIGN KEY ([project_id]) REFERENCES [dbo].[Projects] ([id]),
    CONSTRAINT [FK_Projects_has_users_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([id])
);

