using System;
using System.Net;
using Auth.ServiceModel;
using Newtonsoft.Json;
using RestSharp;

namespace Netsphere
{
    internal class AuthWebAPIClient
    {
        private readonly RestClient _client;

        public AuthWebAPIClient()
        {
            _client = new RestClient(new Uri(Config.Instance.AuthWebAPI.EndPoint));
        }

        public RegisterResult RegisterServer(ServerInfoDto serverInfo)
        {
            var request = new RestRequest("serverlist/register", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(serverInfo), ParameterType.RequestBody);
            return Execute<RegisterResultDto>(request).Result;
        }

        public bool UpdateServer(ServerInfoDto serverInfo)
        {
            var request = new RestRequest("serverlist/", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(serverInfo), ParameterType.RequestBody);

            HttpStatusCode statusCode;
            Execute(request, out statusCode);

            return statusCode == HttpStatusCode.OK;
        }

        public void RemoveServer(ushort id)
        {
            var request = new RestRequest("serverlist/", Method.DELETE);
            request.AddParameter("application/json", JsonConvert.SerializeObject(new RemoveServerDto(id)), ParameterType.RequestBody);
            Execute(request);
        }

        private void Execute(IRestRequest request)
        {
            var response = _client.Execute(request);
            if (response.ErrorException != null)
                throw response.ErrorException;

            if(response.StatusCode != HttpStatusCode.OK)
                throw new WebException(response.Content);
        }

        private void Execute(IRestRequest request, out HttpStatusCode statusCode)
        {
            var response = _client.Execute(request);
            if (response.ErrorException != null)
                throw response.ErrorException;

            statusCode = response.StatusCode;
        }

        private T Execute<T>(IRestRequest request)
            where T : new()
        {
            var response = _client.Execute(request);
            if (response.ErrorException != null)
                throw response.ErrorException;

            if (response.StatusCode != HttpStatusCode.OK)
                throw new WebException(response.Content);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
