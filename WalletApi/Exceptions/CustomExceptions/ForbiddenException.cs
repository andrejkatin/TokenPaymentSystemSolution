using WalletApi.Exceptions.ExceptionErrorCodes;

namespace WalletApi.Exceptions.CustomExceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenExceptionErrorCodes ErrorCode { get; }
        public ForbiddenException(ForbiddenExceptionErrorCodes code, string message) : base(message)
        {
            ErrorCode = code;
        }
    }
}
