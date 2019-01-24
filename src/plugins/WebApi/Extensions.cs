using System.Net;
using System.Threading.Tasks;
using Netsphere.Common;
using Newtonsoft.Json;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace WebApi
{
    public static class WebApiControllerExtensions
    {
        public static Task<bool> JsonResponseAsync(this WebApiController This, object obj,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            This.Response.StatusCode = (int)statusCode;

            return This.StringResponseAsync(obj?.ToJson() ?? "{}");
        }

        public static T ParseJsonNet<T>(this WebApiController This)
        {
            return JsonConvert.DeserializeObject<T>(This.RequestBody());
        }
    }
}
