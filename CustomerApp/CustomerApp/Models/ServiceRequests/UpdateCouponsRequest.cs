using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class UpdateCouponsRequest : ServiceRequest
    {
        string userID;
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/user/" + userID;
        //the type of request
        public override HttpMethod Method => HttpMethod.Put;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        public IList<UpdaterObject> Body;

        // Constructor containing user ID and Coupons value that needs to be updated
        UpdateCouponsRequest(string ID, IList<Coupon> newCoupons)
        {
            userID = ID;

            UpdaterObject UO = new UpdaterObject(newCoupons);

            Body = new List<UpdaterObject>();
            Body.Add(UO);
        }

        // Request body content object
        public class UpdaterObject
        {
            public string propName = "Coupons";
            public IList<SerializableCoupon> value { get; set; }

            public UpdaterObject(IList<Coupon> Coupons)
            {
                value = new List<SerializableCoupon>();
                foreach (Coupon c in Coupons)
                    value.Add(new SerializableCoupon(c));
            }
        }

        public class SerializableCoupon
        {
            public string _id { get; set; }

            public string description { get; set; }

            public IList<string> requiredItems { get; }

            public IList<string> appliedItems { get; }

            public string couponType { get; set; }

            public double discount { get; set; }

            public bool repeatable { get; set; }

            public bool active { get; set; }

            public SerializableCoupon(Coupon c)
            {
                _id = c._id;

                description = c.description;

                requiredItems = c.requiredItems;

                appliedItems = c.appliedItems;

                couponType = c.couponType;

                discount = c.discount;

                repeatable = c.repeatable;

                active = c.active;
            }
        }

        public static async Task<bool> SendUpdateCouponsRequest(string ID, IList<Coupon> newCoupons)
        {
            //make a new request object
            var serviceRequest = new UpdateCouponsRequest(ID, newCoupons);
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
