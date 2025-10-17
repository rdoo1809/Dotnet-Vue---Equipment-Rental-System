namespace Midterm_PROG3340_RDooley;

public class Equipment
{
    public int Id { get; set; }
    public required String Name { get; set; }
    public required String Description { get; set; }
    public String Category { get; set; }
    public String Condition { get; set; }
    public double RentalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}
