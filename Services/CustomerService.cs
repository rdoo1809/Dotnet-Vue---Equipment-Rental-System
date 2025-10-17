using System.Security.Claims;

namespace Midterm_PROG3340_RDooley.Services;

public class CustomerService
{
    public (string? customerName, string? customerRole) GetUserNameAndRole(ClaimsPrincipal user)
    {
        var customerRole = user.FindFirstValue(ClaimTypes.Role);
        var customerName = user.FindFirstValue(ClaimTypes.Name);
        return (customerName, customerRole );
    }
}