using TokenApi.Dtos.Enums;

namespace TokenApi.Models.TokenVersions.V1
{
    public class TokenDataV1
    {
        public decimal Balance { get; set; }

        public int BalanceHash { get; set; }

        public string Version { get; set; }

        public int VersionHash { get; set; }

        public DateTime Created { get; set; }

        public int CreatedHash { get; set; }

        public TokenTypeEnum TokenTypeId { get; set; }

        public int TokenTypeIdHash { get; set; }

        public Guid TransactionId { get; set; }

        public int TransactionIdHash { get; set; }

        public string TokenProof {  get; set; }
    }
}
