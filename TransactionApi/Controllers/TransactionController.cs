using Microsoft.AspNetCore.Mvc;
using TransactionApi.Dtos.TokenDtos;
using TransactionApi.Dtos.TransactionDtos;
using TransactionApi.Services;
using TransactionEFDAL.Models.Enums;

namespace TransactionApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService TransactionService;
        public TransactionController(ITransactionService transactionService)
        {
            TransactionService = transactionService;
        }

        [HttpPost("payment")]
        public async Task<ActionResult> CreateTransactionPayment([FromBody] PaymentTransactionCreateDto transactionCreateDto)
        {
            if (transactionCreateDto.TransactionType != TransactionTypeEnum.Payment) {
                return BadRequest("Wrong transaction type.");
            }

            Guid transactionId = Guid.NewGuid();
            var currentTokens = await TransactionService.GetTokensForTransaction(transactionCreateDto.TokenType, transactionCreateDto.From, transactionCreateDto.To);
            var createdTokens = await TransactionService.CreateNewTokens(transactionId, currentTokens, transactionCreateDto.Amount, transactionCreateDto.TokenType);
            var transactionToProcess = TransactionService.CreateTransactionToProcess(transactionId, TransactionStatus.Success, transactionCreateDto.From, currentTokens, createdTokens, transactionCreateDto.Amount, transactionCreateDto.TokenType);
            TransactionService.AddTransactionToPool(transactionToProcess);

            return Ok();
        }


        [HttpPost("deposit")]
        public async Task<ActionResult> CreateTransactionDeposit([FromBody] DepositOrWithdrawTransactionCreateDto transactionCreateDto)
        {
            if (transactionCreateDto.TransactionType != TransactionTypeEnum.Deposit)
            {
                return BadRequest("Wrong transaction type.");
            }

            Guid transactionId = Guid.NewGuid();
            var currentToken = await TransactionService.GetTokenForTransaction(transactionCreateDto.TokenType, transactionCreateDto.Address);
            var createdToken = await TransactionService.CreateNewToken(currentToken.WalletAddress, transactionId, TransactionTypeEnum.Deposit, currentToken, transactionCreateDto.Amount, transactionCreateDto.TokenType);
            var transactionToProcess = TransactionService.CreateTransactionToProcess(transactionId, 
                TransactionStatus.Success, 
                transactionCreateDto.Address, 
                new TokensForTransactionDto() { TokenFrom = null, TokenTo = currentToken }, 
                new TokensForTransactionDto() { TokenFrom = null, TokenTo = createdToken }, 
                transactionCreateDto.Amount, transactionCreateDto.TokenType);
            TransactionService.AddTransactionToPool(transactionToProcess);

            return Ok();
        }

        [HttpPost("withdraw")]
        public async Task<ActionResult> CreateTransactionWithDraw([FromBody] DepositOrWithdrawTransactionCreateDto transactionCreateDto)
        {
            if (transactionCreateDto.TransactionType != TransactionTypeEnum.Withdraw)
            {
                return BadRequest("Wrong transaction type.");
            }

            Guid transactionId = Guid.NewGuid();
            var currentToken = await TransactionService.GetTokenForTransaction(transactionCreateDto.TokenType, transactionCreateDto.Address);
            var createdToken = await TransactionService.CreateNewToken(currentToken.WalletAddress, transactionId, TransactionTypeEnum.Withdraw, currentToken, transactionCreateDto.Amount, transactionCreateDto.TokenType);
            var transactionToProcess = TransactionService.CreateTransactionToProcess(
                transactionId,
                TransactionStatus.Success,
                transactionCreateDto.Address,
                new TokensForTransactionDto() { TokenFrom = currentToken, TokenTo = null },
                new TokensForTransactionDto() { TokenFrom = createdToken, TokenTo = null },
                transactionCreateDto.Amount, transactionCreateDto.TokenType);
            TransactionService.AddTransactionToPool(transactionToProcess);

            return Ok();
        }
    }
}
