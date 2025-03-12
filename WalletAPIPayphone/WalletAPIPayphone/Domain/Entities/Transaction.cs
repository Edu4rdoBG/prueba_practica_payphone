using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletAPIPayphone.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [ForeignKey("WalletId")]
        [JsonIgnore] 
        public Wallet Wallet { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Type { get; set; } = "Credit"; // Puede ser "Credit" o "Debit"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
