namespace Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PositionId { get; set; }
        public DateTime DateOfEmployment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}