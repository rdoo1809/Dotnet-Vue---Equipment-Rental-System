namespace Midterm_PROG3340_RDooley;

public class Book
{
    public int Id { get; set; }
    public required String Title { get; set; }
    public required String Author { get; set; }
    public decimal Price { get; set; }
    public bool IsAvavilable { get; set; }
    public String? Genre { get; set; }
    public int? PublishedYear { get; set; }
}
