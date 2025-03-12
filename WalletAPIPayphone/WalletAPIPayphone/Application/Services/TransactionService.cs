using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Infrastructure.Repositories;

namespace WalletAPIPayphone.Application.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(int walletId);
        Task<Transaction> CreateTransactionAsync(int walletId, decimal amount, string type);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByWalletIdAsync(int walletId)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("La billetera no existe.");

            return await _transactionRepository.GetByWalletIdAsync(walletId);
        }

        public async Task<Transaction> CreateTransactionAsync(int walletId, decimal amount, string type)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("La billetera no existe.");

            // Validación del tipo de transacción
            if (type != "Debit" && type != "Credit")
                throw new ArgumentException("El tipo de transacción debe ser 'Debit' o 'Credit'.");

            // Validación del saldo suficiente para Débito
            if (type == "Debit" && wallet.Balance < amount)
                throw new InvalidOperationException("Saldo insuficiente para la operación.");

            // Crear la transacción
            var transaction = new Transaction
            {
                WalletId = walletId,
                Amount = amount,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            if (type == "Debit")
                wallet.Balance -= amount;
            else if (type == "Credit")
                wallet.Balance += amount;

            // Guardar cambios en la billetera y la transacción
            await _walletRepository.UpdateAsync(wallet);
            return await _transactionRepository.CreateAsync(transaction);
        }
    }
}
