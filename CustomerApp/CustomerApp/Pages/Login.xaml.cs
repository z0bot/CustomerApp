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
        public Login()
        {
            InitializeComponent();
        }
        //click to QR for now
        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            bool allowed = false;
            allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
                await Navigation.PushModalAsync(new NavigationPage(new QRScannerPage()));
            else
                await DisplayAlert("Alert", "You have to provide Camera permission", "Ok");

        }
    }
}