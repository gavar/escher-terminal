namespace Escher.App.Domain
{
    public static class PersonVerifyStatus
    {
        /// <summary>
        /// Person has invalid state.
        /// </summary>
        public const int Invalid = -1000;
        
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