using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletAPIPayphone.Application.Services;
using WalletAPIPayphone.Domain.Entities;

namespace WalletAPIPayphone.API.Controllers
{

    [Authorize] // Solo usuarios autenticados pueden acceder a Wallet
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wallet>>> GetAll()
        {
            var wallets = await _walletService.GetAllAsync();
            return Ok(wallets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Wallet>> GetById(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
                return NotFound("Billetera no encontrada.");

            return Ok(wallet);
        }

        [HttpPost]
        public async Task<ActionResult<Wallet>> Create(Wallet wallet)
        {
            try
            {
                var createdWallet = await _walletService.CreateAsync(wallet);
                return CreatedAtAction(nameof(GetById), new { id = createdWallet.Id }, createdWallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Wallet>> Update(int id, Wallet wallet)
        {
            if (id != wallet.Id)
                return BadRequest("El ID de la billetera no coincide.");

            try
            {
                var updatedWallet = await _walletService.UpdateAsync(wallet);
                return Ok(updatedWallet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _walletService.DeleteAsync(id);
                if (!success) return NotFound("Billetera no encontrada.");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
