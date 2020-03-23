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
    public partial class endPage : ContentPage
    {
        static double unpaid;
        public endPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Pull unpaid balance from database
            // Faked for now

            unpaid = 100;

            updateLabel();
        }

        async void continueButtonPressed(object sender, EventArgs e)
        {
            if (unpaid > 0)
            {
                if (await DisplayAlert("Unpaid balance remaining", "Your balance has not yet been fully paid. Would you like to make an additional payment?", "Yes", "No"))
                {
                    await Navigation.PushAsync(new checkoutPage());
                }
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }

        // Need to call this regularly somehow
        void updateLabel()
        {
            // Pull unpaid from server


            if (unpaid > 0)
                continueButton.Text = "Make additional payment";
            else
                continueButton.Text = "Log out of table";
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

        // Prevent going back to previous pages, as the order has already been sent. Must start new order via button
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}