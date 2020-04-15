CREATE TABLE [dbo].[knoop]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [puntid] INT NOT NULL, 
    CONSTRAINT [FK_knoop_punt] FOREIGN KEY ([puntid]) REFERENCES [punt]([id])
)
