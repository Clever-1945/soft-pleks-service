create TRIGGER T_ProductVersion_AFTER_UPDATE ON ProductVersion AFTER UPDATE
AS
BEGIN
INSERT INTO EventLog (ID, Description)
SELECT
    (SELECT NEWID()),
    ('updated Version id = ' + convert(nvarchar(36), Id) + ', new name = ' + Name)
FROM INSERTED
END;