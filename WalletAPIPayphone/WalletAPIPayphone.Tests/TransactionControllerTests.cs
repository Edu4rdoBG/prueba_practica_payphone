using System.Net.Http.Json;
using WalletAPIPayphone.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WalletAPIPayphone.Tests
{
    public class TransactionControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private string token = "";
        public TransactionControllerTests(WebApplicationFactory<Program> factory)
        {
            // Crea un cliente HTTP para interactuar con la API en memoria
            _client = factory.CreateClient();
        }

        private async Task CrearToken() {
            if (token == "")
            {
                var loginRequest = new { username = "admin", password = "1234" };
                var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
                loginResponse.EnsureSuccessStatusCode();

                var jsonString = await loginResponse.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(jsonString);
                token = jsonDoc.RootElement.GetProperty("token").GetString()!;

            }
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnCreated_WhenValidTransaction()
        {
            await CrearToken();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

            // Arrange: Crear una billetera para simular una transferencia
            var wallet = new Wallet
            {
                DocumentId = "987654321",
                Name = "Francisco Tenorio",
                Balance = 1000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Crear la billetera
            var response = await _client.PostAsJsonAsync("/api/Wallet", wallet);
            response.EnsureSuccessStatusCode();
            var createdWallet = await response.Content.ReadFromJsonAsync<Wallet>();

            // Act: Realizar una transferencia
            var transactionRequest = new { Amount = 500, Type = "Debit" };
            var transactionResponse = await _client.PostAsJsonAsync($"/api/Transaction/wallet/{createdWallet.Id}", transactionRequest);

            // Assert: Verificar que la respuesta sea exitosa
            Assert.Equal(System.Net.HttpStatusCode.Created, transactionResponse.StatusCode);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnBadRequest_WhenInsufficientFunds()
        {
            await CrearToken();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

            // Arrange: Crear una billetera con un saldo insuficiente
            var wallet = new Wallet
            {
                DocumentId = "987654321",
                Name = "Francisco Tenorio",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var response = await _client.PostAsJsonAsync("/api/Wallet", wallet);
            response.EnsureSuccessStatusCode();
            var createdWallet = await response.Content.ReadFromJsonAsync<Wallet>();

            // Act: Intentar realizar una transferencia con saldo insuficiente
            var transactionRequest = new { Amount = 200, Type = "Debit" };
            var transactionResponse = await _client.PostAsJsonAsync($"/api/Transaction/wallet/{createdWallet.Id}", transactionRequest);

            // Assert: Verificar que la respuesta sea BadRequest
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, transactionResponse.StatusCode);
        }
    }
}
