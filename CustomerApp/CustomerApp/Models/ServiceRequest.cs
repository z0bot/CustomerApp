using System.Collections.Generic;
using System.Net.Http;

namespace CustomerApp.Models
{
    public abstract class ServiceRequest
    {
        public abstract string Url { get; }
        public abstract HttpMethod Method { get; }
        public abstract Dictionary<string, string> Headers { get; }
    }
}
