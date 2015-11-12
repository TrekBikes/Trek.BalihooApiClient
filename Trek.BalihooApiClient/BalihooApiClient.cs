using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using EnsureThat;
using RestSharp;

namespace Trek.BalihooApiClient
{
    /// <summary>
    /// An API client library partially implementing the API as it is documented
    /// at https://github.com/balihoo/local-connect-client
    /// </summary>
    public class BalihooApiClient
    {
        public string ApiVersion { get; }
        public string BaseUrl { get; }
        public string ClientId { get; set; }
        public string ClientApiKey { get; set; }

        private readonly RestClient _restClient;

        public BalihooApiClient() : this(string.Empty, string.Empty)
        { }

        public BalihooApiClient(string clientId, string clientApiKey)
        {
            ApiVersion = "v1.0";
            BaseUrl = "https://bac.balihoo-cloud.com/localdata/";
            ClientId = clientId;
            ClientApiKey = clientApiKey;

            var assembly = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            _restClient = new RestClient(new Uri($"{BaseUrl}{ApiVersion}"))
            {
                UserAgent = "TrekBalihooApiClient/" + assembly.Version + " (.NET " + Environment.Version + ")"
            };
        }

        #region GenerateClientApiKey

        public ClientApiKeyInfo GenerateClientApiKey(string apiKey, string brandKey)
        {
            return GenerateClientApiKey(apiKey, brandKey, string.Empty, string.Empty);
        }

        public ClientApiKeyInfo GenerateClientApiKey(string apiKey, string brandKey, string groupId, string userId)
        {
            Ensure.That(apiKey, nameof(apiKey)).IsNotNullOrWhiteSpace();
            Ensure.That(brandKey, nameof(brandKey)).IsNotNullOrWhiteSpace();

            var request = new RestRequest("genClientAPIKey");
            request.AddParameter("apiKey", apiKey);
            request.AddParameter("brandKey", brandKey);
            request.AddParameter("groupId", string.IsNullOrWhiteSpace(groupId) ? "NA" : groupId);
            request.AddParameter("userId", string.IsNullOrWhiteSpace(groupId) ? "TrekBalihooApiClient" : userId);

            var response = _restClient.Post<ClientApiKeyInfo>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = response.Data;
                ClientId = data.ClientId;
                ClientApiKey = data.ClientApiKey;

                return data;
            }

            throw new InvalidOperationException("genClientAPIKey API call failed" + Environment.NewLine + "Status Code: " + response.StatusCode + Environment.NewLine + response.ErrorMessage);
        }

        #endregion

        #region Campaigns

        public Dictionary<int, List<Campaign>> GetCampaigns(IList<int> locationIds)
        {
            return GetCampaigns(locationIds, null, null);
        }

        public Dictionary<int, List<Campaign>> GetCampaigns(IList<int> locationIds, DateTime? from, DateTime? to)
        {
            Ensure.That(locationIds, nameof(locationIds)).IsNotNull();
            Ensure.That(locationIds.Count, nameof(locationIds)).IsNot(0);

            return MakeRequest<Campaign>(CreateAuthedRequest("campaignswithtactics", locationIds, from, to));
        }

        #endregion

        #region Tactics

        public Dictionary<int, List<CampaignTacticMetric>> GetTacticMetrics(IList<int> locationIds, int tacticId)
        {
            return GetTacticMetrics(locationIds, tacticId, null, null);
        }

        public Dictionary<int, List<CampaignTacticMetric>> GetTacticMetrics(IList<int> locationIds, int tacticId, DateTime? from, DateTime? to)
        {
            Ensure.That(locationIds, nameof(locationIds)).IsNotNull();
            Ensure.That(locationIds.Count, nameof(locationIds)).IsNot(0);

            return MakeRequest<CampaignTacticMetric>(CreateAuthedRequest("tactic/" + tacticId + "/metrics", locationIds, from, to));
        }

        #endregion

        #region Private Helpers

        private static void AddCommonParameters(IRestRequest request, IEnumerable<int> locationIds, DateTime? from, DateTime? to)
        {
            if (locationIds != null)
            {
                request.AddParameter(LocationsToParameter(locationIds));
            }
            
            if (from.HasValue)
            {
                request.AddParameter(DateToParameter("from", from.Value));
            }

            if (to.HasValue)
            {
                request.AddParameter(DateToParameter("to", to.Value));
            }
        }
        private static Parameter LocationsToParameter(IEnumerable<int> locations)
        {
            Ensure.That(locations, nameof(locations)).IsNotNull();

            return new Parameter { Name = "locations", Value = string.Join(",", locations), Type = ParameterType.QueryString };
        }

        private static Parameter DateToParameter(string name, DateTime value)
        {
            return new Parameter { Name = name, Value = $"{value:yyyy-MM-dd}", Type = ParameterType.QueryString };
        }

        private RestRequest CreateAuthedRequest(string resource, IEnumerable<int> locationIds, DateTime? from, DateTime? to)
        {
            var request = CreateAuthedRequest(resource);
            AddCommonParameters(request, locationIds, from, to);
            return request;
        }

        private RestRequest CreateAuthedRequest(string resource)
        {
            Ensure.That(ClientId, nameof(ClientId)).IsNotNullOrWhiteSpace();
            Ensure.That(ClientApiKey, nameof(ClientApiKey)).IsNotNullOrWhiteSpace();

            var request = new RestRequest(resource);
            request.AddHeader("X-ClientId", ClientId);
            request.AddHeader("X-ClientApiKey", ClientApiKey);

            return request;
        }

        private Dictionary<int, List<T>> MakeRequest<T>(IRestRequest request)
        {
            // The API has two different results based on the number of locations being requested.
            // Attempting to handle that here.  Not elegant, but should get the job done.
            var locationParam = (string)request.Parameters.First(p => p.Name.Equals("locations")).Value;
            
            if (!locationParam.Contains(","))
            {
                var response = _restClient.Get<List<T>>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new Dictionary<int, List<T>> { { int.Parse(locationParam), response.Data } };
                }

                throw new ApiResponseException(response);
            }
            else
            {
                var response = _restClient.Get<Dictionary<int, List<T>>>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Data;
                }

                throw new ApiResponseException(response);
            }
        }

        #endregion
    }
}
