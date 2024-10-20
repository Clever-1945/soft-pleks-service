using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServiceApp.Entities;

[Table("Product")]
public class Product
{
    public Guid Id { set; get; }
    [StringLength(255)]
    public string Name { set; get; }
    [StringLength(Int32.MaxValue)]
    public string Description { set; get; }
}