namespace CareerConnect.Models.DTOs
{
    public class UserApplication
    {
        public int ApplicationId { get; set; }
        public DateTime AppliedAt { get; set; }
        public Job Job { get; set; } = null!;
    }
}
