using System.ComponentModel.DataAnnotations;

namespace CareerConnect.Models
{
    public enum UserType { 
        JOB_SEEKER,
        EMPLOYER
    }
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Name { get; set; } = String.Empty;

        [Required]
        public string Email { get; set; } = String.Empty;

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        //public string? Experience { get; set; }


    }
}
