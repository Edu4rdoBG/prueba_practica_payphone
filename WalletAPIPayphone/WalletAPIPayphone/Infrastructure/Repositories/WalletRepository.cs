using Microsoft.EntityFrameworkCore;
using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Infrastructure.Persistence;

namespace WalletAPIPayphone.Infrastructure.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> GetByIdAsync(int id);
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task<bool> DeleteAsync(int id);
    }

    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _context;

        public WalletRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> GetByIdAsync(int id)
        {
            return await _context.Wallets
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var wallet = await GetByIdAsync(id);
            if (wallet == null) return false;

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
