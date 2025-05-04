namespace CareerConnect.Models;

public enum JobType{
    FULL_TIME,
    PART_TIME,
    INTERNSHIP
}
public class Job
{
    public int Id { get; set; }
    public int EmployerId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public JobType Type { get; set; } = JobType.FULL_TIME;      // Default to Full time
    public string Salary { get; set; } = string.Empty;       // örn: $50,000 - $70,000
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public ICollection<Application> Applications { get; set; } = new List<Application>();

}