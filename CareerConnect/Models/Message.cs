namespace CareerConnect.Models
{
    public enum MessageStatus
    {
        SENT,
        DELIVERED,
        READ
    }

    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public MessageStatus Status { get; set; } = MessageStatus.SENT;
        public bool IsDeleted { get; set; } = false;
        public string? Attachment { get; set; } // Optional attachment field
    }
}
