using WalletApi.Exceptions.ExceptionErrorCodes;

namespace WalletApi.Exceptions.CustomExceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundExceptionErrorCodes ErrorCode { get; }
        public NotFoundException(NotFoundExceptionErrorCodes code, string message) : base(message)
        {
            ErrorCode = code;
        }
    }
}
