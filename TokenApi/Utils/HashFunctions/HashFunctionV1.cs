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

        public static int ComputeFNV1aHash(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            const int fnvPrime = 16777619;
            int hash = unchecked((int)2166136261);

            foreach (byte b in input)
            {
                hash ^= b;
                hash *= fnvPrime;
            }

            return Math.Abs(hash);
        }


        public static int ComputeDJB2Hash(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            int hash = 5381;

            foreach (byte b in input)
            {
                hash = ((hash << 5) + hash) + b; // hash * 33 + b
            }

            return Math.Abs(hash);
        }

        public static int ComputeJenkinsHash(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            uint hash = 0;

            foreach (byte b in input)
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return Math.Abs((int)hash);
        }

        public static uint ComputeCRC32(byte[] input)
        {
            if (input == null || input.Length == 0)
                return 0;

            uint crc = 0xFFFFFFFF;

            foreach (byte b in input)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ 0xEDB88320;
                    else
                        crc >>= 1;
                }
            }

            return ~crc;
        }

    }
}
