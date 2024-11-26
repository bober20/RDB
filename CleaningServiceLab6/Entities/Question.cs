namespace Entities;

public class Question
{
    public int Id { get; set; }
    public int? AnswerId { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
}