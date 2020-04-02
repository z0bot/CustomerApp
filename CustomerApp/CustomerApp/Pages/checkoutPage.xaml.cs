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
    public partial class checkoutPage : ContentPage
    {
        static double contribution = 0, tip = 0;
        static bool tipChanged = false;
        
        public checkoutPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            System.Windows.Input.ICommand cmd = new Command(onRefresh);
            orderRefreshView.Command = cmd;

            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DisplayOrder();
        }

        async void OnPayButtonClicked(object sender, EventArgs e)
        {
            if ((contribution + tip) > 0)
            {
                // Update items paid for in database


                await Navigation.PushAsync(new paymentPage(contribution + tip));
            }
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

            tipChanged = true;
            // Round to two decimal places, then update the textboxes
            tip = Math.Round(tip, 2);

            tipEntry.Text = tip.ToString("C");

            payButton.Text = "Pay " + (contribution + tip).ToString("C");
        }

        void OnContributionCompleted()
        {
            if((contribution + tip) > 0)
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

        public void DisplayOrder()
        {
            // Fetch most recent order status

            

            menuFoodItemsView.ItemsSource = RealmManager.All<MenuFoodItem>().Where((MenuFoodItem m) => m.paid == false).ToList();
        }

        void OnTogglePaid(object sender, ToggledEventArgs e)
        {
            // Don't do anything if Realm is writing
            if (RealmManager.Realm.IsInTransaction)
                return;

            MenuFoodItem temp = new MenuFoodItem(((MenuFoodItem)((ViewCell)(((Switch)sender).Parent.Parent.Parent)).BindingContext));
            if (e.Value)
            {
                temp.paid = true;

                contribution += temp.price;
                contribution = Math.Round(contribution, 2);
            }
            else
            {
                temp.paid = false;
                contribution -= temp.price;
                contribution = Math.Round(contribution, 2);
            }
            OnContributionCompleted();

            RealmManager.AddOrUpdate<MenuFoodItem>(temp);

        }

        void onRefresh()
        {
            // Pull newest order status
            DisplayOrder();

            orderRefreshView.IsRefreshing = false;
        }


        // Prevent going back to previous pages, as the order has already been sent. Must continue and pay
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}