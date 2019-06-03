
namespace GenesisTest.WebAPI.Helpers
{
    public interface ITokenManager
    {
        /// <summary>
        /// Generate token for user authentication
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        string GenerateTokenForUser(string Id);
    }
}
