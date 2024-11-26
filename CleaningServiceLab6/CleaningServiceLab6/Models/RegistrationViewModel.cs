using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CleaningServiceLab6.Models;

public class RegistrationViewModel
{
    [Required]
    [NotNull]
    public string FirstName { get; set; }
    [Required]
    [NotNull]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirmation { get; set; }
    public bool IsStaff { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
}

