namespace CareerConnect.Models;

public class Offer
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public int EmployerId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
