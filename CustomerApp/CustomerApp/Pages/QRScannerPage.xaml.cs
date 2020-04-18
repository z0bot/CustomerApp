using GoogleVisionBarCodeScanner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScannerPage : ContentPage, INotifyPropertyChanged
    {
        public QRScannerPage()
        {
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
            /*for(int i = 0; i < obj.Count; i++)
            {
                //result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
                result = obj[i].DisplayValue;
            }*/

            int tablenum = -1;
            if (!int.TryParse(result, out tablenum) || tablenum < 1 || tablenum > 20)
            {
                await DisplayAlert("Invalid Table", "Sorry, we couldn't find table " + result + ", please try again", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
                return;
            }

            RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().tableNum = tablenum);

            await DisplayAlert("You're signed in!", "You've succesfully signed in to table " + tablenum + "!", "OK");
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            // Turn off flashlight if it is on
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

            await Navigation.PushAsync(new orderHerePage());
        }

        async void manualEntry(object sender, EventArgs e)
        {
            
            string result = await DisplayPromptAsync("Enter table number", "Enter the table number you are sitting at", "OK", "Cancel", "1, 2, 3, ... etc.", -1, keyboard: Keyboard.Numeric, null);

            int tablenum = -1;
            if(!int.TryParse(result, out tablenum) || tablenum < 1 || tablenum > 20)
            {
                await DisplayAlert("Invalid Table", "Sorry, we couldn't find table " + result + ", please try again", "OK");
                GoogleVisionBarCodeScanner.Methods.SetIsScanning(true);
                return;
            }

            RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().tableNum = tablenum);

            await DisplayAlert("You're signed in!", "You've succesfully signed in to table " + tablenum + "!", "OK");
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            // Turn off flashlight if it is on
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

            await Navigation.PushAsync(new orderHerePage());
        }
    }
}