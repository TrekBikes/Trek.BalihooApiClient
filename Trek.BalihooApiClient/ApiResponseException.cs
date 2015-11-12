using System;
using System.Net;
using System.Runtime.Serialization;
using RestSharp;

namespace Trek.BalihooApiClient
{
    [Serializable]
    public class ApiResponseException : Exception
    {

        public ResponseData HttpResponseData { get; }

        public ApiResponseException()
        {
        }


        public ApiResponseException(IRestResponse response) : this("API call failed", response)
        {
        }

        public ApiResponseException(string message, IRestResponse response) : base(message)
        {
            HttpResponseData = new ResponseData
            {
                ErrorMessage = response.ErrorMessage,
                Content = response.Content,
                ResponseUri = response.ResponseUri,
                StatusCode = response.StatusCode,
                ErrorException = response.ErrorException
            };
        }

        public ApiResponseException(string message) : base(message)
        {
        }

        public ApiResponseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ApiResponseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        [Serializable]
        public class ResponseData
        {
            
            public string ErrorMessage { get; set; }
            public string Content { get; set; }
            public Uri ResponseUri { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public Exception ErrorException { get; set; }
        }
    }
}