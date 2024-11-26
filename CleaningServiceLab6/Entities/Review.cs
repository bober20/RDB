namespace Entities;

public class Review
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public short Rate { get; set; }
    public string ReviewContent { get; set; }
    public DateTime ReviewDate { get; set; } = DateTime.Now;
}