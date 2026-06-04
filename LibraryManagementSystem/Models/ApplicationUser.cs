using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string Imie { get; set; } = string.Empty;

    public string Nazwisko { get; set; } = string.Empty;
}