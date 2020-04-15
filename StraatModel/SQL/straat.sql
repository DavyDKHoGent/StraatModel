CREATE TABLE [dbo].[straat]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [naam] NVARCHAR(150) NOT NULL, 
    [gemeenteid] INT NOT NULL, 
    CONSTRAINT [FK_straat_gemeente] FOREIGN KEY ([gemeenteid]) REFERENCES [gemeente]([id])
)
