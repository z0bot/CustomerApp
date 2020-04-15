using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class GetOrderRequest : ServiceRequest
    {
        string orderID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/orders/" + orderID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Get;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        GetOrderRequest(string ID)
        {
            orderID = ID;
        }

        public static async Task<bool> SendGetOrderRequest(string ID)
        {
            //make a new request object
            var serviceRequest = new GetOrderRequest(ID);
            //get a response
            var response = await ServiceRequestHandler.MakeServiceCall<Order>(serviceRequest);

            if(response == null)
            {
                //call failed
                return false;
            }
            else
            {
                // Add the response into the local database

                // Remove current contents
                RealmManager.RemoveAll<Order>();
                RealmManager.RemoveAll<OrderItem>();

                RealmManager.AddOrUpdate<Order>(response);
                //call succeeded
                return true;
            }
        }
    }
}
