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
    public partial class checkoutPage : ContentPage
    {
        static double unpaid, contribution = 0, tip = 0;
        static bool tipChanged = false;

        
        public checkoutPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Pull unpaid total from database
            // Faked for now
            unpaid = 100;

            unpaidEntry.Text = unpaid.ToString("C");
        }

        async void OnPayButtonClicked(object sender, EventArgs e)
        {
            if ((contribution + tip) > 0)
                await Navigation.PushAsync(new paymentPage(contribution + tip));
            else // No contribution
                await Navigation.PushAsync(new endPage());
        }

        void OnTipCompleted(object sender, EventArgs e)
        {
            // Sanity check inputs
            if (((Entry)sender).Text == "$" || ((Entry)sender).Text == "$." || ((Entry)sender).Text == "." || ((Entry)sender).Text == "" || ((Entry)sender).Text == null)
                tip = 0;
            else
                tip = double.Parse(((Entry)sender).Text, System.Globalization.NumberStyles.Currency);

            if (tip < 0)
                tip = 0;

            // Round to two decimal places, then update the textboxes
            tip = Math.Round(tip, 2);

            tipEntry.Text = tip.ToString("C");

            payButton.Text = "Pay " + (contribution + tip).ToString("C");
        }

        void OnContributionCompleted(object sender, EventArgs e)
        {
            // Sanity check inputs
            if (((Entry)sender).Text == "$" || ((Entry)sender).Text == "$." || ((Entry)sender).Text == "." || ((Entry)sender).Text == "" || ((Entry)sender).Text == null)
                contribution = 0;
            else
                contribution = double.Parse(((Entry)sender).Text, System.Globalization.NumberStyles.Currency);

            if (contribution < 0)
                contribution = 0;
            else if (contribution > unpaid)
                contribution = unpaid;

            // Round to two decimal places, then update the textboxes
            contribution = Math.Round(contribution, 2);

            ((Entry)sender).Text = contribution.ToString("C");

            unpaidEntry.Text = (unpaid - contribution).ToString("C");
            if((contribution + tip) != 0)
                payButton.Text = "Pay " + (contribution + tip).ToString("C");
            else
                payButton.Text = "No Contribution";

            // Suggest tip if not specified
            if (!tipChanged)
            {
                tipEntry.Placeholder = (contribution * 0.2).ToString("C");
            }
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

        // Prevent going back to previous pages, as the order has already been sent. Must continue and pay
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}