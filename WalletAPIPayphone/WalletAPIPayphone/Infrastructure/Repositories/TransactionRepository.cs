using Microsoft.EntityFrameworkCore;
using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Infrastructure.Persistence;

namespace WalletAPIPayphone.Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
        Task<Transaction> CreateAsync(Transaction transaction);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly WalletDbContext _context;

        public TransactionRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
