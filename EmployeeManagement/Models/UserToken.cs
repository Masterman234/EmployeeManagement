using System.ComponentModel.DataAnnotations;
namespace EmployeeManagement.Models
{
    public class UserToken
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        
        [Required, MaxLength(200)]
        public string Token { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Type { get; set; } = "Refresh";
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
