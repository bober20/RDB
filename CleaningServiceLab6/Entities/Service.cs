using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public int Price { get; set; }
    public int Area { get; set; }
    public string ExtraInfo { get; set; }
}