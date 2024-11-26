namespace CleaningServiceLab6.Models;

public class EmployeeViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public int PositionId { get; set; }
}