create TRIGGER T_ProductVersion_AFTER_INSERT ON ProductVersion AFTER INSERT
AS
BEGIN
INSERT INTO EventLog (ID, Description )
SELECT
    (SELECT NEWID()),
    ('added Version id = ' + convert(nvarchar(36), Id) + ', new name = ' + Name)
FROM INSERTED
END;