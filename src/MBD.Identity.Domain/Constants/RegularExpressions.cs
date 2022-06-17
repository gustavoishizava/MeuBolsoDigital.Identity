namespace MBD.Identity.Domain.Constants
{
    public static class RegularExpressions
    {
        public const string StrongPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$";
    }
}