namespace TokenApi.Utils.Extensions
{
    public static class ConvertIntToByteArray
    {
        public static byte[] ToByteArray(this int value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
