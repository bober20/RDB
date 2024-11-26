namespace Entities;

public class Answer
{
    public int Id { get; set; }
    public int? EmployeeId { get; set; }
    public int QuestionId { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
}