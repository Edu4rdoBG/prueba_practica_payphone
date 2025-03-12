using Microsoft.EntityFrameworkCore;
using WalletAPIPayphone.Application.Services;
using WalletAPIPayphone.Domain.Entities;
using WalletAPIPayphone.Infrastructure.Persistence;
using WalletAPIPayphone.Infrastructure.Repositories;

namespace WalletAPIPayphone.Tests
{
    public class TransactionServiceTests
    {
        private readonly WalletDbContext _context;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionService _transactionService;

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(databaseName: "WalletAPIDatabase")
                .Options;

            _context = new WalletDbContext(options);
            _transactionRepository = new TransactionRepository(_context);
            _walletRepository = new WalletRepository(_context); // Debes implementar este repositorio también
            _transactionService = new TransactionService(_transactionRepository, _walletRepository);
        }

        [Fact]
        public async Task CreateTransaction_ShouldCreateTransaction_WhenValidData()
        {
            // Arrange
            var wallet = new Wallet
            {
                DocumentId = "123456789",
                Name = "Juan Pérez",
                Balance = 1000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act
            var transaction = await _transactionService.CreateTransactionAsync(wallet.Id, 500, "Debit");

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal(wallet.Id, transaction.WalletId);
            Assert.Equal(500, transaction.Amount);
            Assert.Equal("Debit", transaction.Type);
            Assert.Equal(wallet.Balance, 500);
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowError_WhenInsufficientBalance()
        {
            // Arrange
            var wallet = new Wallet
            {
                DocumentId = "123456789",
                Name = "Juan Pérez",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _transactionService.CreateTransactionAsync(wallet.Id, 200, "Debit"));

            Assert.Equal("Saldo insuficiente para la operación.", exception.Message);
        }

        [Fact]
        public async Task CreateTransaction_ShouldThrowError_WhenInvalidTransactionType()
        {
            // Arrange
            var wallet = new Wallet
            {
                DocumentId = "123456789",
                Name = "Juan Pérez",
                Balance = 1000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                async () => await _transactionService.CreateTransactionAsync(wallet.Id, 500, "InvalidType"));

            Assert.Equal("El tipo de transacción debe ser 'Debit' o 'Credit'.", exception.Message);
        }
    }
}
