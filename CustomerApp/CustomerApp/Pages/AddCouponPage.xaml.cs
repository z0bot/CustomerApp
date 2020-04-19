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

        public AddCouponPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);
        }

        async void OnFinalizeButtonClicked(object sender, EventArgs e)
        {

            await Navigation.PopAsync();
        }

        async void OnRefillButtonClicked(object sender, EventArgs e)
        {
            // Send refill request
            string notificationType = "Refill";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);

            await DisplayAlert("Refill", "Server Notified of Refill Request", "OK");
        }

        async void OnServerButtonClicked(object sender, EventArgs e)
        {
            // Send Help Request
            string notificationType = "Help requested";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);
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

            // Get most recent user data (including coupons)
            var success = await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);

            if (!(await GetCouponsByIDRequest.SendGetCouponsByIDRequest(result)))
            {
                await DisplayAlert("Invalid Coupon", "Sorry, we couldn't find that coupon, please try again", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
                return;
            }

            

            // Ensure coupon is unique and active
            if (success && RealmManager.All<User>().FirstOrDefault().coupons.Where((Coupon c) => c._id == result).ToList().Count().Equals(0) && (RealmManager.Find<Coupon>(result)).active)
            {
                // Add coupon
                RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().coupons.Add(RealmManager.Find<Coupon>(result)));

                await UpdateCouponsRequest.SendUpdateCouponsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().coupons);

                await DisplayAlert("Coupon Succesfully Applied!", "You've succesfully applied this coupon!", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

                // Turn off flashlight if it is on
                if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                    GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Inactive Coupon", "Sorry, but this coupon has already been redeemed\nVerify that you have the correct coupon, or call your server for assistance", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            }
        }

        async void manualEntry(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Enter coupon ID", "Enter your coupon code here:\n\nWe suggest copying the code from your email", "OK", "Cancel", null, -1, keyboard: Keyboard.Numeric, null);

            // Get most recent user data (including coupons)
            var success = await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);

            if (result == null || !(await GetCouponsByIDRequest.SendGetCouponsByIDRequest(result)))
            {
                await DisplayAlert("Invalid Coupon", "Sorry, we couldn't find that coupon, please try again", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
                return;
            }

            
            // Ensure coupon is unique and active
            if (success && RealmManager.All<User>().FirstOrDefault().coupons.Where((Coupon c) => c._id == result).Count().Equals(0) && (RealmManager.Find<Coupon>(result)).active)
            {
                // Add coupon
                RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().coupons.Add(RealmManager.Find<Coupon>(result)));

                await UpdateCouponsRequest.SendUpdateCouponsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().coupons);

                await DisplayAlert("Coupon Succesfully Applied!", "You've succesfully applied this coupon!", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

                // Turn off flashlight if it is on
                if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                    GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Inactive Coupon", "Sorry, but this coupon has already been redeemed\nVerify that you have the correct coupon, or call your server for assistance", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
            }
        }
    }
}