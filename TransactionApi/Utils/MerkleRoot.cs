namespace TransactionApi.Utils
{
    public static class MerkleRoot
    {
        private readonly static Func<string,string> HashFunc = HashFunctions.ComputeMd5Hash;

        public static string GenerateMerkleRoot(List<string> transactions)
        {
            if (transactions.Count == 0) return string.Empty;

            List<string> currentLevel = new List<string>();

            foreach (var transaction in transactions)
            {
                currentLevel.Add(HashFunc(transaction));
            }

            while (currentLevel.Count > 1)
            {
                List<string> nextLevel = new List<string>();

                for (int i = 0; i < currentLevel.Count; i += 2)
                {
                    if (i + 1 < currentLevel.Count)
                    {
                        nextLevel.Add(HashFunc(currentLevel[i] + currentLevel[i + 1]));
                    }
                    else
                    {
                        nextLevel.Add(currentLevel[i]);
                    }
                }

                currentLevel = nextLevel;
            }

            return currentLevel[0];
        }

        public static List<string> GetMerklePath(List<string> transactions, string transaction, string merkleRoot)
        {
            return GenerateMerklePath(transactions, transaction, merkleRoot);
        }

        private static List<string> GenerateMerklePath(List<string> transactions, string targetTransaction, string merkleRoot)
        {
            List<string> merklePath = new List<string>();
            if (transactions.Count == 0)
                return merklePath;

            List<string> currentLevel = new List<string>();
            foreach (var transaction in transactions)
            {
                currentLevel.Add(HashFunc(transaction));
            }

            while (currentLevel.Count > 1)
            {
                List<string> nextLevel = new List<string>();
                for (int i = 0; i < currentLevel.Count; i += 2)
                {
                    if (i + 1 < currentLevel.Count)
                    {
                        string combinedHash = HashFunc(currentLevel[i] + currentLevel[i + 1]);
                        nextLevel.Add(combinedHash);
                        if (currentLevel[i] == HashFunc(targetTransaction) || (i + 1 < currentLevel.Count && currentLevel[i + 1] == HashFunc(targetTransaction)))
                        {
                            // Add the sibling hash to the path
                            merklePath.Add(i + 1 < currentLevel.Count ? currentLevel[i + 1] : currentLevel[i]);
                        }
                    }
                    else
                    {
                        nextLevel.Add(currentLevel[i]);
                    }
                }

                currentLevel = nextLevel;
            }

            merklePath.Add(merkleRoot);
            return merklePath;
        }
    }
}
