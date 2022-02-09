using System.Net;

namespace Etch.OrchardCore.Greenhouse.Services.Dtos
{
    public class GreenhouseApplicationResponse
    {
        public string Error { get; set; }
        public int ResponseCode { get; set; }
        public bool Success
        {
            get { return ResponseCode == (int)HttpStatusCode.OK; }
        }
    }
}
