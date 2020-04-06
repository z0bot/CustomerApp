using GoogleVisionBarCodeScanner;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScannerPage : ContentPage, INotifyPropertyChanged
    {
        public QRScannerPage()
        {
            InitializeComponent();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.QRCode);
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

        public async void ManualTableEntry_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Table Number", "Enter the table number shown at the center of your table", "OK");
            await Navigation.PushAsync(new orderHerePage());
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

            string result = string.Empty;
            for(int i = 0; i < obj.Count; i++)
            {
                result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
            }

            await DisplayAlert("You're signed in!", result, "OK");
            GoogleVisionBarCodeScanner.Methods.SetIsScanning(false);

            // Turn off flashlight if it is on
            if (GoogleVisionBarCodeScanner.Methods.IsTorchOn())
                GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();

            await Navigation.PushAsync(new orderHerePage());
        }
    }
}