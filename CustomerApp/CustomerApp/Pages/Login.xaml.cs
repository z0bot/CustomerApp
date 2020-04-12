using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;
using GoogleVisionBarCodeScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        //page constructor
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method that dictates what happens when QR scan button is pressed
        /// </summary>
        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            //will eventually make webcall to check against server if valid user and valid password
            //for now just checks that the entries aren't null
            if(String.IsNullOrEmpty(uxEmailAdress.Text) || String.IsNullOrEmpty(uxPassword.Text))
            {
                await DisplayAlert("ERROR", "All entries must be filled!", "OK");
            }
            else if (await CameraPermissions())
            {
                /*will eventually be a post request to fill out the rest of the properties
                for a user that theoretically already exists
                For now, creates new user object and stores it in realm*/

                var response = await UserAuthenticationRequest.SendUserAuthenticationRequest(uxEmailAdress.Text, uxPassword.Text);
                if(response)
                {
                    User currentUser = new User
                    {
                        email = uxEmailAdress.Text,
                        password = uxPassword.Text,
                    };
                    RealmManager.AddOrUpdate<User>(currentUser);
                    await Navigation.PushAsync(new QRScannerPage());
                }
                else
                {
                    await DisplayAlert("Incorrect Login Credentials", "No account found with that email and/or password", "Try Again");
                }
            }
            else
                await DisplayAlert("Alert", "You have to provide Camera permission", "Ok");
        }
        public async Task<bool> CameraPermissions()
        {
            //captures users response to camera permission request
            bool allowed = false;
            //user camera request
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            return allowed;
        }

        private void NewAccountButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new NewAccount()));
        }

        // Since the app is now a large loop, we must prevent the user from going back to the endPage
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}