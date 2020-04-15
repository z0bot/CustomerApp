using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewAccount : ContentPage
    {
        public NewAccount()
        {
            InitializeComponent();
            MessagingCenterResponse();
        }

        private async void RegisterAcctButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uxFirstName.Text) ||
                string.IsNullOrEmpty(uxLastName.Text) ||
                string.IsNullOrEmpty(uxEmail.Text) ||
                string.IsNullOrEmpty(uxPassword.Text) ||
                string.IsNullOrEmpty(uxPasswordReentry.Text) ||
                uxBirthdate.Date == null)
            {
                await DisplayAlert("ERROR", "All entries must be filled!", "OK");
            }
            else if (!uxEmail.Text.Contains("@"))
            {
                await DisplayAlert("ERROR", "Invalid email entry!", "OK");
            }
            else if (UserPasswordCheck())
            {
                //space checking on email
                uxEmail.Text = uxEmail.Text.Replace(" ", "");
                var response = await AddUserRequest.SendAddUserRequest(uxFirstName.Text, uxLastName.Text, uxEmail.Text, uxPassword.Text, uxBirthdate.Date.ToString("MM,d,yyyy"));
                if (response)
                {
                    await DisplayAlert("Successful", "Account has been registered!", "OK");
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                await DisplayAlert("ERROR", "Passwords must match!", "Continue");
            }
        }
        //Used to catch errors thrown by the service request 
        //determined by entering preexisting email tied to an account
        public void MessagingCenterResponse()
        {
            MessagingCenter.Subscribe<HttpResponseMessage>(this, "Conflict", async (sender) =>
            {
                await DisplayAlert("Email already registered", "Please try again with a different email", "OK");
            });
        }
        /// <summary>
        /// A check that returns true if the user passwords match and then
        /// stores account info into realm 
        /// </summary>
        public bool UserPasswordCheck()
        {
            if (uxPassword.Text == uxPasswordReentry.Text)
            {
                StoreUserDetails();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Method that will store the user account details into 
        /// Realm, a local data base that persist user data on device
        /// </summary>
        public void StoreUserDetails()
        {
            User user = new User();

            user.first_name = uxFirstName.Text;
            user.last_name = uxLastName.Text;
            user.birthday = uxBirthdate.Date.ToString("MM,d,yyyy");
            user.email = uxEmail.Text;
            user.password = uxPassword.Text;

            RealmManager.RemoveAll<User>();
            RealmManager.AddOrUpdate<User>(user);
        }
    }
}