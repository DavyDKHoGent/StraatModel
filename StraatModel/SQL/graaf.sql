CREATE TABLE [dbo].[graaf]
(
	[knoopid] INT NOT NULL , 
    [segmentid] INT NOT NULL, 
    [straatid] INT NOT NULL, 
    PRIMARY KEY ([knoopid], [segmentid], [straatid]), 
    CONSTRAINT [FK_graaf_knoop] FOREIGN KEY ([knoopid]) REFERENCES [knoop]([id]), 
    CONSTRAINT [FK_graaf_segment] FOREIGN KEY ([segmentid]) REFERENCES [segment]([id]), 
    CONSTRAINT [FK_graaf_straat] FOREIGN KEY ([straatid]) REFERENCES [straat]([id])
)
