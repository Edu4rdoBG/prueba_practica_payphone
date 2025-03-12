using System.ComponentModel.DataAnnotations;

namespace WalletAPIPayphone.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DocumentId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
