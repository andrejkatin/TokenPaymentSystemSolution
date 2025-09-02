namespace TokenApi.Utils.Extensions
{
    public static class ConvertIntArrayToByteArray
    {
        public static byte[] ToByteArray(this int[] intArray)
        {
            byte[] byteArray = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }
}
