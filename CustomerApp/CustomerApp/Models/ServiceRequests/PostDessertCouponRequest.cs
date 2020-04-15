using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class PostDessertCouponRequest : ServiceRequest
    {
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/coupon/";
        //the type of request
        public override HttpMethod Method => HttpMethod.Post;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        public SerializableCoupon Body;

        // Constructor containing the employee ID and tip amount to be sent
        PostDessertCouponRequest()
        {
            SerializableCoupon c = new SerializableCoupon();

            Body = c;
        }

        // Request body content object
        public class SerializableCoupon
        {
            public string couponType = "Customer";
            public IList<string> requiredItems = new List<string>();
            public IList<string> appliedItems = new List<string>();
            public double discount = 100;
            public bool active = true;
            public bool repeatable = false;
            public string description = "Claim 100% off one of our desserts";

            public SerializableCoupon()
            {
                string ID = "*****COOKIE ID HERE *******";
                requiredItems.Add(ID);
                appliedItems.Add(ID);
            }
        }


        // Posts a tip to the database
        public static async Task<bool> SendPostDessertCouponRequest()
        {
            //make a new request object
            var serviceRequest = new PostDessertCouponRequest();
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
