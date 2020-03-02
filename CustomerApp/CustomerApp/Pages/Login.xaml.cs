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
            //captures users response to camera permission request
            bool allowed = false;

            //user camera request
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();

            //if T then push to QRScanner page
            //else display alert
            if (allowed)
                await Navigation.PushModalAsync(new NavigationPage(new QRScannerPage()));
            else
                await DisplayAlert("Alert", "You have to provide Camera permission", "Ok");

        }

        private void NewAccountButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}