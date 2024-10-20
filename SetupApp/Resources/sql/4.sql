create TRIGGER T_Product_AFTER_INSERT ON Product AFTER INSERT
AS
BEGIN
INSERT INTO EventLog (ID, Description )
SELECT
    (SELECT NEWID()),
    ('added Product id = ' + convert(nvarchar(36), Id) + ', new name = ' + Name)
FROM INSERTED
END;