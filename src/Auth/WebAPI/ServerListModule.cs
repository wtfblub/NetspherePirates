using Auth.ServiceModel;
using Nancy;
using Nancy.Extensions;
using Netsphere.Network;
using Newtonsoft.Json;

namespace Netsphere.WebAPI
{
    public class ServerListModule : NancyModule
    {
        public ServerListModule()
            : base("/serverlist")
        {
            // Register a server
            Post["/register"] = _ =>
            {
                var serverInfo = JsonConvert.DeserializeObject<ServerInfoDto>(Request.Body.AsString());

                var result = AuthServer.Instance.ServerManager.Add(serverInfo)
                    ? RegisterResult.OK
                    : RegisterResult.AlreadyExists;
                return JsonConvert.SerializeObject(new RegisterResultDto(result));
            };

            // Update a server
            Post["/"] = _ =>
            {
                var serverInfo = JsonConvert.DeserializeObject<ServerInfoDto>(Request.Body.AsString());
                var result = AuthServer.Instance.ServerManager.Update(serverInfo);
                return result ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            };

            // Remove a server
            Delete["/"] = _ =>
            {
                var serverInfo = JsonConvert.DeserializeObject<RemoveServerDto>(Request.Body.AsString());
                AuthServer.Instance.ServerManager.Remove(serverInfo.Id);
                return HttpStatusCode.OK;
            };
        }
    }
}
