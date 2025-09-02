using BankWalletApi.Services;
using BankWalletEFDAL.DataAccess;
using BankWalletEFDAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankWalletApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankWalletController : ControllerBase
    {
        private readonly IBankWalletService BankWalletService;

        public BankWalletController(IBankWalletService bankWalletService)
        {
            BankWalletService = bankWalletService;     
        }

        [HttpGet]
        public async Task<ActionResult<Wallet>> GetBankWallet()
        {
            var bankWallet = await BankWalletService.GetBankWallet();
            return Ok(bankWallet);
        }

        [HttpPost]
        public Task<ActionResult> UpdateBankWallet()
        {

        }

        [HttpPost("deposit")]
        public Task<ActionResult> Deposit(DepositDto depositDto)
        {

        }

        [HttpPost("withdraw")]
        public Task<ActionResult> Withdraw(WithdrawDto depositDto)
        {

        }
    }
}
