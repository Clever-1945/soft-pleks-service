create TRIGGER T_Product_AFTER_UPDATE ON Product AFTER UPDATE
AS
BEGIN
INSERT INTO EventLog (ID, Description)
SELECT
    (SELECT NEWID()),
    ('updated Product id = ' + convert(nvarchar(36), Id) + ', new name = ' + Name)
FROM INSERTED
END;