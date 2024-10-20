create TRIGGER T_Product_DELETE ON Product after delete
AS
BEGIN
	-- SET NOCOUNT ON;

INSERT INTO EventLog (ID, Description )

SELECT
    (SELECT NEWID()),
    ('delete Product id = ' + convert(nvarchar(36), Id) + ', name = ' + Name)
FROM DELETED

END;