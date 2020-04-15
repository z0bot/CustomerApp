using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Linq;

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

                // Ugly code that maintains which coupons have been selected
                if (RealmManager.Find<User>(response.user.email) != null)
                {
                    response.user.tableNum = RealmManager.Find<User>(response.user.email).tableNum;

                    IList<Coupon> cList = RealmManager.Find<User>(response.user.email).coupons;
                    if (!cList.Count.Equals(0))
                    {
                        foreach(Coupon c in cList) // List of coupons we had before
                        {
                            Coupon kept = response.user.coupons.Where((Coupon d) => d._id == c._id).FirstOrDefault();
                            if (kept != null) // The account still has this coupon
                            {
                                response.user.coupons.Where((Coupon d) => d._id == c._id).FirstOrDefault().selected = c.selected;
                            }
                        }
                    }
                }

                RealmManager.AddOrUpdate<User>(response.user);
                await Task.Delay(100);
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
