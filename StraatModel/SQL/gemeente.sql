CREATE TABLE [dbo].[gemeente]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [naam] NVARCHAR(150) NULL, 
    [provincieid] INT NOT NULL, 
    CONSTRAINT [FK_gemeente_provincie] FOREIGN KEY ([provincieid]) REFERENCES [provincie]([id])
)
