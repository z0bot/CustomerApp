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
        public PostNotificationsRequest(string notificationType, string employeeID, string sender)
        {
            Body = new NotificationRequestBody
            {
                notificationType = notificationType,
                employee_id = employeeID,
                sender = sender
            };
        }
        public static async Task<bool> SendNotificationRequest(string notificationType, string employeeID, string sender)
        {
            var sendNotificationRequest = new PostNotificationsRequest(notificationType, employeeID, sender);
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
            public string employee_id;
            public string notificationType { get; set; }
            public string sender { get; set; }
        }
    }
}
