
using Newtonsoft.Json;

namespace GenesisTest.WebAPI.Helpers
{
    public static class StringToJsonFormatter
    {
        /// <summary>
        /// String Extension to convert to Json
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToJson(this string str)
        {
            return JsonConvert.SerializeObject(new { message = $"{str}" } );
        }
    }
}
