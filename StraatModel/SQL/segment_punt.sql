CREATE TABLE [dbo].[segment_punt]
(
	[segmentid] INT NOT NULL , 
    [puntid] INT NOT NULL, 
    PRIMARY KEY ([segmentid], [puntid]), 
    CONSTRAINT [FK_segment_punt_segment] FOREIGN KEY ([segmentid]) REFERENCES [segment]([id]), 
    CONSTRAINT [FK_segment_punt_punt] FOREIGN KEY ([puntid]) REFERENCES [punt]([id])
)
