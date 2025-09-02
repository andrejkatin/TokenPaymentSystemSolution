namespace TokenApi.Utils.Extensions
{
    public static class ConvertDecimalToByteArray
    {
        public static byte[] ToByteArray(this decimal value)
        {
            // Get the bits of the decimal
            int[] bits = decimal.GetBits(value);

            // Create a byte array to hold 16 bytes (4 int * 4 bytes each)
            byte[] bytes = new byte[16];

            // Copy each int into the byte array
            for (int i = 0; i < bits.Length; i++)
            {
                byte[] intBytes = BitConverter.GetBytes(bits[i]);
                Array.Copy(intBytes, 0, bytes, i * 4, 4);
            }

            return bytes;
        }
    }
}
