using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using CustomerApp.Pages;

namespace CustomerApp.Models.ServiceRequests
{
    /// Post request to add a new user to the server
    public class AddUserRequest : ServiceRequest
    {
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/user/signup";
        public override HttpMethod Method => HttpMethod.Post;
        public override Dictionary<string, string> Headers => null;
        public User Body;

        public AddUserRequest(string firstName, string lastName, string email, string password, string birthday)
        {
            Body = new User
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                password = password,
                birthday = birthday
            };
        }
        public static async Task<bool> SendAddUserRequest(string firstName, string lastName, string email, string password, string birthday)
        {
            var sendAddUserRequest = new AddUserRequest(firstName, lastName, email, password, birthday);
            var response = await ServiceRequestHandler.MakeServiceCall<UserPostResponse>(sendAddUserRequest, sendAddUserRequest.Body);
            string test = "test";

            if (response.message == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
