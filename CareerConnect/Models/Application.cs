namespace CareerConnect.Models;

public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int SeekerId { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}