using CustomerApp.Models;
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
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DisplayOrder();
        }

        async void OnSendOrderClicked(object sender, EventArgs e)
        {
            //Navigate to order confirmation page
            if (await DisplayAlert("WARNING: Sending Order", "Are you sure you want to send the order? No further edits may be made by anyone else at the table", "Yes", "No"))
            {
                // Set order status to 'sent'
                RealmManager.Write(() => RealmManager.All<Order>().FirstOrDefault().sent = true);

                // Update remote database

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
                if (await DisplayAlert("WARNING: Changes will be lost", "Are you sure you want to leave this page?", "Yes", "No"))
                {
                    RealmManager.RemoveAll<Order>();
                    try
                    {
                        await Navigation.PopAsync();
                    }
                    catch
                    {
                        await DisplayAlert("CAUGHT", "You suck", "OK");
                    }
                }
            });
            return true;
        }

        public void DisplayOrder()
        {
            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().Contents.ToList();
        }
    }

}