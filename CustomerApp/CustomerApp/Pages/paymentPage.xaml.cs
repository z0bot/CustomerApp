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
        public paymentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        async void scanCard(object sender, EventArgs e)
        {
            DependencyService.Get<ICard>().StartRead();

            // There is definitely a better way to wait for the card to get read
            await System.Threading.Tasks.Task.Delay(1000);

            // Add button to confirm order if it was not already added
            if(mainStack.Children.Count != 4) { 
                Button confirm = new Button()
                {
                    Text = "Confirm Payment",
                    Margin = new Thickness(30, 0, 30, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                };

                confirm.Clicked += (Sender, args) => confirmButton();

                mainStack.Children.Add(confirm);
            }
        }

        async void confirmButton()
        {
            if(await DisplayAlert("Card Details", DependencyService.Get<ICard>().GetCardName() + "\n" + DependencyService.Get<ICard>().GetCardNum() + "\n" + DependencyService.Get<ICard>().GetCardCvv(), "Confirm", "Cancel"))
            {
                await DisplayAlert("Confimed", "Payment confirmed", "OK");

                // Whatever comes next
            }
            else
            {

            }
        }
    }
}