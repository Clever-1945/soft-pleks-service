if not exists (select * from sysobjects where name='EventLog' and xtype='U') BEGIN
create table EventLog (
                          Id uniqueidentifier not null PRIMARY KEY,
                          EventDate date DEFAULT GETUTCDATE(),
                          Description nvarchar(MAX),
)

CREATE INDEX I_EventLog_EventDate ON EventLog (EventDate);
END


if not exists (select * from sysobjects where name='Product' and xtype='U') BEGIN

create table Product (
                         Id uniqueidentifier not null PRIMARY KEY DEFAULT (NEWID()),
                         Name nvarchar(255) not null,
                         Description nvarchar(MAX)
);

CREATE UNIQUE INDEX UK_Product_Name ON Product (Name);

END

if not exists (select * from sysobjects where name='ProductVersion' and xtype='U') BEGIN
create table ProductVersion (
                                Id uniqueidentifier not null PRIMARY KEY DEFAULT (NEWID()),
                                ProductId uniqueidentifier not null,
                                Name nvarchar(255) not null,
                                Description nvarchar(MAX),
                                CreatingDate date DEFAULT GETUTCDATE(),
                                Wdith int not null,
                                Height int not null,
                                Length int not null,
)


ALTER TABLE ProductVersion ADD CONSTRAINT FK_ProductVersion_ProductId FOREIGN KEY (ProductId) REFERENCES Product (Id) ON DELETE CASCADE

CREATE INDEX I_ProductVersion_CreatingDate ON ProductVersion (CreatingDate);
CREATE INDEX I_ProductVersion_Wdith ON ProductVersion (Wdith);
CREATE INDEX I_ProductVersion_Height ON ProductVersion (Height);
CREATE INDEX I_ProductVersion_Length ON ProductVersion (Length);

END
