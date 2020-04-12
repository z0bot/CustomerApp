using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    public class PostNotificationsRequest : ServiceRequest
    {
         public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/notifications";
        public override HttpMethod Method => HttpMethod.Post;
        public override Dictionary<string, string> Headers => null;
        public NotificationRequestBody Body;

        //May need to include employee id
        public PostNotificationsRequest(string notificationType)
        {
            Body = new NotificationRequestBody
            {
                notificationType = notificationType,
            };
        }
        public static async Task<bool> SendNotificationRequest(string notificationType)
        {
            var sendNotificationRequest = new PostNotificationsRequest(notificationType);
            var response = await ServiceRequestHandler.MakeServiceCall<NotificationResponse>(sendNotificationRequest, sendNotificationRequest.Body);
            if(response.message == null)
            {
                return false;
            }
            else if(response.message == "Employee ID given does not match any ID" )
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public class NotificationRequestBody
        {
            public string notificationType { get; set; }
            public string sender { get; set; }
        }
    }
}
