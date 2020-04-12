using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    public class UserAuthenticationRequest : ServiceRequest
    {
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/user/authentication";
        public override HttpMethod Method => HttpMethod.Post;
        public override Dictionary<string, string> Headers => null;
        public UserAuthenticationBody Body;

        public UserAuthenticationRequest(string email, string password)
        {
            Body = new UserAuthenticationBody
            {
                email = email,
                password = password,
            };
        }
        public static async Task<bool> SendUserAuthenticationRequest(string email, string password)
        {
            var sendUserAuthRequest = new UserAuthenticationRequest(email, password);
            var response = await ServiceRequestHandler.MakeServiceCall<UserPostResponse>(sendUserAuthRequest, sendUserAuthRequest.Body);

            if (response.message == "Email does not exist" || response.message == "Inncorrect Password")
            {
                return false;
            }
            else
            {
                response.user.password = password;
                RealmManager.AddOrUpdate<User>(response.user);
                return true;
            }
        }
        public class UserAuthenticationBody
        {
            public string email { get; set; }
            public string password { get; set; }
        }
    }
}
