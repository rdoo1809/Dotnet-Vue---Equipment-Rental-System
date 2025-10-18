namespace Midterm_PROG3340_RDooley;

public class RentalDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public int CustomerId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
