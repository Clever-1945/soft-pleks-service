using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServiceApp.Entities;

[Table("ProductVersion")]
public class ProductVersion
{
    public Guid Id { set; get; }
    public Guid ProductId { set; get; }
    [StringLength(255)]
    public string Name { set; get; }
    [StringLength(Int32.MaxValue)]
    public string Description { set; get; }
    
    public DateTime CreatingDate { set; get; }
    public int Wdith { set; get; }
    public int Height { set; get; }
    public int Length { set; get; }
}