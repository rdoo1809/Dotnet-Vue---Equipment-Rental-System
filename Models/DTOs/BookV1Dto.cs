using System.ComponentModel.DataAnnotations;

namespace Midterm_PROG3340_RDooley.Models.DTOs;

public class BookV1Dto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Author { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    public bool IsAvailable { get; set; }
}