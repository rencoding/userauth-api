
namespace GenesisTest.WebAPI.DomainEntities
{
    public enum UserSessionStatus
    {
        /// <summary>
        /// Authorized
        /// </summary>
        Authorized,

        /// <summary>
        /// Unauthorized; Token doesnot exist, User Id or Token not matching
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Invalid Session; Token expired
        /// </summary>
        InvalidSession
    }
}
