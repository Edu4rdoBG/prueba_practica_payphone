using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Infrastructure.Repositories;

namespace WalletAPIPayphone.Application.Services
{
    public interface IWalletService
    {
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task<bool> DeleteAsync(int id);
        Task<Wallet> GetByIdAsync(int id);
        Task<IEnumerable<Wallet>> GetAllAsync();
    }

    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(wallet.Name))
                throw new ArgumentException("El nombre es obligatorio.");

            if (wallet.Balance < 0)
                throw new ArgumentException("El saldo no puede ser negativo.");

            wallet.CreatedAt = DateTime.UtcNow;
            wallet.UpdatedAt = DateTime.UtcNow;

            return await _walletRepository.CreateAsync(wallet);
        }

        public async Task<Wallet> UpdateAsync(Wallet wallet)
        {
            var existingWallet = await _walletRepository.GetByIdAsync(wallet.Id);
            if (existingWallet == null)
                throw new KeyNotFoundException("La billetera no existe.");

            // Validaciones básicas
            existingWallet.Name = wallet.Name;
            existingWallet.Balance = wallet.Balance;
            existingWallet.UpdatedAt = DateTime.UtcNow;

            return await _walletRepository.UpdateAsync(existingWallet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                throw new KeyNotFoundException("La billetera no existe.");

            return await _walletRepository.DeleteAsync(id);
        }

        public async Task<Wallet> GetByIdAsync(int id)
        {
            var wallet = await _walletRepository.GetByIdAsync(id);
            if (wallet == null)
                throw new KeyNotFoundException("La billetera no existe.");

            return wallet;
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            return await _walletRepository.GetAllAsync();
        }
    }
}
