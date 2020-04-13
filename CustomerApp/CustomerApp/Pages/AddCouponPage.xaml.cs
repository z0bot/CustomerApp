using GoogleVisionBarCodeScanner;
using CustomerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models.ServiceRequests;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCouponPage : ContentPage
    {
        double contribution = 0, tip = 0;

        public AddCouponPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
        }

        async void OnFinalizeButtonClicked(object sender, EventArgs e)
        {

            await Navigation.PopModalAsync();
        }

        async void OnRefillButtonClicked(object sender, EventArgs e)
        {
            // Send refill request


            await DisplayAlert("Refill", "Server Notified of Refill Request", "OK");
        }

        async void OnServerButtonClicked(object sender, EventArgs e)
        {
            // Send Help Request

            await DisplayAlert("Help Request", "Server Notified of Help Request", "OK");
        }

        /// <summary>
        /// initialize device flashlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlashlightButton_Clicked(object sender, EventArgs e)
        {
            GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
        }

        /// <summary>
        /// Once barcode detected, "OnDetected" event will be triggered, 
        /// do the stuff with the barcode, it will contain type and display value of QR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> obj = e.BarcodeResults;

            string result = obj[0].DisplayValue;

            if (!(await GetCouponsByIDRequest.SendGetCouponsByIDRequest(result)))
            {
                await DisplayAlert("Invalid Coupon", "Sorry, we couldn't find that coupon, please try again", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
                return;
            }

            // Add coupon

            await DisplayAlert("Coupon Succesfully Applied!", "You've succesfully applied this coupon!", "OK");
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            // Turn off flashlight if it is on
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

            await Navigation.PopModalAsync();
        }

        async void manualEntry(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Enter coupon ID", "Enter your coupon code here:\n\nWe suggest copying the code from your email", "OK", "Cancel", null, -1, keyboard: Keyboard.Numeric, null);

            if (!(await GetCouponsByIDRequest.SendGetCouponsByIDRequest(result)))
            {
                await DisplayAlert("Invalid Coupon", "Sorry, we couldn't find that coupon, please try again", "OK");
                return;
            }

            // Add coupon

            await DisplayAlert("Coupon Succesfully Applied!", "You've succesfully applied this coupon!", "OK");
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            // Turn off flashlight if it is on
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

            await Navigation.PopModalAsync();
        }
    }
}