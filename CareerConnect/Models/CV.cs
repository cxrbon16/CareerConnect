namespace CareerConnect.Models;

public class CV
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<string>? Education { get; set; } = new();
    public List<string>? Experience { get; set; } = new();
}
