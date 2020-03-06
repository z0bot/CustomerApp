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
    public partial class orderHerePage : ContentPage
    {
        public orderHerePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        async void OnOrderHereButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new YourOrderPage()));
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

        // Disable back button for this page
        protected override bool OnBackButtonPressed() { return true; }
    }
}