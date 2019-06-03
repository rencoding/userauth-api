
using Newtonsoft.Json;

namespace GenesisTest.WebAPI.DomainEntities
{
    /// <summary>
    /// Error details for exception handling
    /// </summary>
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
