namespace TokenApi.Utils.Extensions
{
    public static class HasFractionalPart
    {
        public static bool HasFraction(this decimal value)
        {
            return value % 1 != 0;
        }
    }
}
