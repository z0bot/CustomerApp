using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class UpdatePointsRequest : ServiceRequest
    {
        string userID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/user/" + userID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Put;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        public IList<UpdaterObject> Body;

        // Constructor containing user ID and points value that needs to be updated
        UpdatePointsRequest(string ID, double newPoints)
        {
            userID = ID;

            UpdaterObject UO = new UpdaterObject(newPoints);

            Body = new List<UpdaterObject>();
            Body.Add(UO);
        }

        // Request body content object
        public class UpdaterObject
        {
            public string propName = "points";
            public double value { get; set; }

            public UpdaterObject(double points)
            {
                value = points;
            }
        }


        public static async Task<bool> SendUpdatePointsRequest(string ID, double newPoints)
        {
            //make a new request object
            var serviceRequest = new UpdatePointsRequest(ID, newPoints);
            //get a response
            var response = await ServiceRequestHandler.MakeServiceCall<DeleteResponse>(serviceRequest, serviceRequest.Body);

            if(response == null)
            {
                //call failed
                return false;
            }
            else
            {
                //call succeeded
                return true;
            }
        }
    }
}
