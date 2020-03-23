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

            System.Windows.Input.ICommand cmd = new Command(onRefresh);
            refresher.Command = cmd;
            updateLabel();
        }

        async void continueButtonPressed(object sender, EventArgs e)
        {
            updateLabel();

            if (unpaid > 0)
            {
                if (await DisplayAlert("Unpaid balance remaining", "Your balance has not yet been fully paid. Would you like to make an additional payment?", "Yes", "No"))
                {
                    await Navigation.PushAsync(new checkoutPage());
                }
            }
            else
            {
                if (await DisplayAlert("Log out confirmation", "Once logged out, you will not be able to request refills until a new order is started. Are you sure you want to leave the table?", "Yes", "No"))
                {
                    await Navigation.PopToRootAsync();
                }
            }
        }

        void onRefresh()
        {
            updateLabel();
            refresher.IsRefreshing = false;
        }

        // Need to call this regularly via onRefresh
        void updateLabel()
        {
            // Pull unpaid from server
            // Currently decrement every time triggered
            unpaid -= 40;

            if (unpaid > 0)
                continueButton.Text = "Make payment towards remaining " + unpaid.ToString("C");
            else
                continueButton.Text = "Log out of table";

            return ;
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