CREATE TABLE [dbo].[segment]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [beginknoopid] INT NOT NULL, 
    [eindknoopid] INT NOT NULL, 
    CONSTRAINT [FK_segment_beginknoop] FOREIGN KEY ([beginknoopid]) REFERENCES [knoop]([id]), 
    CONSTRAINT [FK_segment_eindknoop] FOREIGN KEY ([eindknoopid]) REFERENCES [knoop]([id])
)
