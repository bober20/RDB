namespace Entities;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int EmployeeId { get; set; }
    public int BonusId { get; set; }
    public string ClientAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public List<Service> Services { get; set; }
    public int OrderSum { get; set; }
}