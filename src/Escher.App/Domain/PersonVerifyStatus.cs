namespace Escher.App.Domain
{
    public static class PersonVerifyStatus
    {
        /// <summary>
        /// Person is to young for registration in a system.
        /// </summary>
        public const int Young = -2;

        /// <summary>
        /// Person requires parents authorization which he don't have.
        /// </summary>
        public const int Unauthorized = -1;
    }
}