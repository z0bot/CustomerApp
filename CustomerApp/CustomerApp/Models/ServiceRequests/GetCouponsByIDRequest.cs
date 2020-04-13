using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class GetCouponsByIDRequest : ServiceRequest
    {
        public string ID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/coupons" + ID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Get;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null; 

        GetCouponsByIDRequest(string couponID)
        {
            ID = couponID;
        }

        public static async Task<bool> SendGetCouponsByIDRequest(string ID)
        {
            //make a new request object
            var serviceRequest = new GetCouponsByIDRequest(ID);
            //get a response
            var response = await ServiceRequestHandler.MakeServiceCall<Coupon>(serviceRequest);

            if(response == null)
            {
                //call failed
                return false;
            }
            else
            {
                //add the response into the local database
                RealmManager.AddOrUpdate<Coupon>(response);
                //call succeeded
                return true;
            }
        }
    }
}
