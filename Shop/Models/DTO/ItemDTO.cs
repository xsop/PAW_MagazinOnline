using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Shop.Models.DTO;
public class ItemDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string? ItemName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Description { get; set; }
    [Required]
    public double Price { get; set; }
    public string? Image { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public IEnumerable<SelectListItem>? CategoryList { get; set; }
}