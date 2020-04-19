using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomerApp.Models.ServiceRequests
{
    class UpdateIngredientRequest : ServiceRequest
    {
                public string id;
        public override string Url => "https://dijkstras-steakhouse-restapi.herokuapp.com/ingredients/" + id;
        public override HttpMethod Method => HttpMethod.Put;
        public override Dictionary<string, string> Headers => null;
        public IList<UpdateIngredientRequestObject> Body;

        public UpdateIngredientRequest(string id, string updateQuantity, string amount)
        {
            this.id = id;
            UpdateIngredientRequestObject updateIngredientRequestObject = new UpdateIngredientRequestObject
            {
                propName = updateQuantity,
                value = amount
            };
            Body = new List<UpdateIngredientRequestObject>();
            Body.Add(updateIngredientRequestObject);
        }
        public static async Task<bool> SendUpdateIngredientRequest(string id, string updateQuantity, string amount)
        {
            var sendUpdateIngredientRequest = new UpdateIngredientRequest(id, updateQuantity, amount);
            var response = await ServiceRequestHandler.MakeServiceCall<DeleteResponse>(sendUpdateIngredientRequest, sendUpdateIngredientRequest.Body);
            if(response == null)
            {
                return false;
            }
            else
            { 
                return true;
            }
        }
    }
    public class UpdateIngredientRequestObject
    {
        public string propName { get; set; }
        public string value { get; set; }
    }
}
