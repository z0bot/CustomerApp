using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class PostReviewRequest : ServiceRequest
    {
        //the endpoint we are trying to hit
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/reviews/";
        //the type of request
        public override HttpMethod Method => HttpMethod.Post;
        //headers if we ever need them
        public override Dictionary<string, string> Headers => null;

        public ReviewObject Body;

        // Constructor containing the employee ID and tip amount to be sent
        PostReviewRequest(string OrderID, string EmployeeID, int rating, string reason)
        {
            ReviewObject r = new ReviewObject();

            r.order_id = OrderID;
            r.employee_id = EmployeeID;
            r.question01_rating = rating;
            r.question01_reason = reason;

            Body = r;
        }

        // Request body content object
        public class ReviewObject
        {
            public string order_id;
            public string employee_id;
            public int question01_rating;
            public string question01_reason;
        }


        // Posts a tip to the database
        public static async Task<bool> SendPostReviewRequest(string OrderID, string EmployeeID, int rating, string reason)
        {
            //make a new request object
            var serviceRequest = new PostReviewRequest(OrderID, EmployeeID, rating, reason);
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
