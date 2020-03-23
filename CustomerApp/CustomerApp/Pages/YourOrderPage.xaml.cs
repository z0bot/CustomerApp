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
    public partial class YourOrderPage : ContentPage
    {
        public YourOrderPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

        }

        async void OnSendOrderClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm selection", "Once sent, order cannot be changed. Continue?", "Yes", "No"))
            { 
                // Send order to kitchen

                //Navigate to order confirmation page
                await Navigation.PushAsync(new checkoutPage());
            }
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            //Navigate to menu

            await Navigation.PushAsync(new menuPage());
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

        // Disable back button for this page with a confirmation warning
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if(await DisplayAlert("WARNING: Changes will be lost", "Are you sure you want to leave this page?", "Yes", "No"))
                {
                    await Navigation.PopAsync();
                }
            });
            return true; 
        }
    }

}