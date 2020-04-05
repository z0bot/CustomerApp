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
    public partial class paymentPage : ContentPage
    {
        static double total;

        public paymentPage(double payment)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            total = payment;
        }

        async void scanCard(object sender, EventArgs e)
        {
            DependencyService.Get<ICard>().StartRead();

            // Give the card reader time to come up
            await System.Threading.Tasks.Task.Delay(1000);

            // Add button to confirm order if it was not already added
            if(mainStack.Children.Count != 5) {
                Button confirm = new Button()
                {
                    Text = "Confirm Payment of " + total.ToString("C"),
                    Margin = new Thickness(30, 0, 30, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                };

                confirm.Clicked += (Sender, args) => confirmButton();

                mainStack.Children.Insert(3, confirm);
            }
        }


        async void confirmButton()
        {
            if (DependencyService.Get<ICard>().ReadSuccesful())
            {
                if (await DisplayAlert("Card Details", DependencyService.Get<ICard>().GetCardName() + "\n" 
                    + DependencyService.Get<ICard>().GetCardNum() + "\n" 
                    + DependencyService.Get<ICard>().GetCardCvv() + "\n" 
                    + "Valid expiry? " + (DependencyService.Get<ICard>().IsExpiryValid() ? "Yes" : "No"), "Confirm", "Cancel"))
                {
                    await DisplayAlert("Confimed", "Payment confirmed", "OK");

                    // Remove previous page to prevent double payment
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count() - 2]);


                    // Add points


                    // Offer game
                    if (await DisplayAlert("Game Opportunity", "Would you like to play a game for a chance at a free dessert?", "Yes, please!", "No thanks"))
                    {
                        Navigation.InsertPageBefore(new gamePage(), this);
                    }
                    else
                    {
                        Navigation.InsertPageBefore(new endPage(), this);
                    }
                    await Navigation.PopAsync();
                        
                }
                else
                {

                }
            }
            else
                await DisplayAlert("Error", "Couldn't read card data, try again", "OK");
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
    }
}