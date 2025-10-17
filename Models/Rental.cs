namespace Midterm_PROG3340_RDooley;

public class Rental
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public int CustomerId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
