CREATE OR ALTER PROC FindProductVersion @ProductName nvarchar(255), @ProductVersionName nvarchar(255), @MinVolume int, @MaxVolume int
AS
BEGIN
select
    pv.Id as ProductVersion,
    p.Name as ProductName,
    pv.Name as ProductVersionName,
    pv.Wdith,
    pv.Height,
    pv.Length
from ProductVersion pv
         join Product p on (p.id = pv.ProductId)
where
        p.Name like '%' + @ProductName + '%' or
        pv.Name like '%' + @ProductVersionName + '%' or
        (pv.Wdith * pv.Height * pv.Length) >= @MinVolume or
        (pv.Wdith * pv.Height * pv.Length) <= @MaxVolume
END