using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class FinishOrderRequest : ServiceRequest
    {
        public string tableID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/tables/finishorder/" + tableID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Post;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        // Constructor containing the table ID
        FinishOrderRequest(string ID)
        {
            tableID = ID;
        }

        // Posts a tip to the database
        public static async Task<bool> SendFinishOrderRequest(string tableID)
        {
            //make a new request object
            var serviceRequest = new FinishOrderRequest(tableID);
            //get a response
            var response = await ServiceRequestHandler.MakeServiceCall<DeleteResponse>(serviceRequest);


            //No response is set so just assume it worked
            return true;

        }
    }
}
