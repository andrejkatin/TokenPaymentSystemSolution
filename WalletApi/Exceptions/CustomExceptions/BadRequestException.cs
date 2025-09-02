using WalletApi.Exceptions.ExceptionErrorCodes;

namespace WalletApi.Exceptions.CustomExceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestExceptionErrorCodes ErrorCode { get; }
        public BadRequestException(BadRequestExceptionErrorCodes code, string message) : base(message)
        {
            ErrorCode = code;
        }      
    }
}
