namespace TokenApi.Utils.Extensions
{
    public static class ConvertDataTimeToByteArray
    {
        public static byte[] ToByteArray(this DateTime dateTime)
        {
            return BitConverter.GetBytes(dateTime.ToBinary());
        }
    }
}
