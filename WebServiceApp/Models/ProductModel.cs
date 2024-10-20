namespace WebServiceApp.Models;

public class ProductModel
{
    public Guid? Id { set; get; }
    public string? Name { set; get; }
    public string? Description { set; get; }
    
    public ProductVersionModel[]? Versions  { set; get; }
}


public class ProductVersionModel
{
    public string Id { set; get; }
    public string? Name { set; get; }
    public string? Description { set; get; }
    
    public DateTime? CreatingDate { set; get; }
    public int? Wdith { set; get; }
    public int? Height { set; get; }
    public int? Length { set; get; }
}