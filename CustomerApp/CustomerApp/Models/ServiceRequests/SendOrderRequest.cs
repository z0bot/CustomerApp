using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class SendOrderRequest : ServiceRequest
    {
        string orderID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/orders/" + orderID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Put;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        public IList<UpdaterObject> Body;

        // Constructor containing the order ID to be sent
        SendOrderRequest(string ID)
        {
            orderID = ID;

            UpdaterObject UO = new UpdaterObject();

            Body = new List<UpdaterObject>();
            Body.Add(UO);
        }

        // Request body content object
        public class UpdaterObject
        {
            public string propName = "send_to_kitchen";
            public bool value = true;
        }


        // Named strangely, but this updates the order at ID to be 'sent'
        public static async Task<bool> SendSendOrderRequest(string ID)
        {
            //make a new request object
            var serviceRequest = new SendOrderRequest(ID);
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
