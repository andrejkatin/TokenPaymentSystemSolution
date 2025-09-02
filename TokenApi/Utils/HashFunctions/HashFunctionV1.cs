namespace TokenApi.Utils.HashFunctions
{
    public static class HashFunctionV1
    {
        public static int ComputeHash(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0; 

            int hash = 17;
            foreach (byte b in input)
            {
                hash = (hash * 31 + b) % 23; 
            }

            return hash;
        }
    }
}
