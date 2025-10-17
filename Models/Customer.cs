using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Midterm_PROG3340_RDooley;

public class Customer
{
    public int Id { get; set; }
    public required String UserName { get; set; }
    public required String Password { get; set; }
    public required String Email { get; set; }
    public String Role { get; set; }
}
