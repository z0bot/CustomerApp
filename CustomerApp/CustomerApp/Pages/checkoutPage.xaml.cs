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
        bool tipChanged = false;
        
        public checkoutPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Necessary for the refreshview to work
            System.Windows.Input.ICommand cmd = new Command(onRefresh);
            orderRefreshView.Command = cmd;

            // Set view to unpaid food items in this order
            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().Contents.Where((MenuFoodItem m) => m.paid == false).ToList();
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

        // Clear the tip's entry every time it is selected
        void clearTip(object sender, EventArgs e)
        {
            ((Entry)sender).Text = "";
        }

        // Called when entry is completed on the tip field. Sets tipChanged to true, preventing suggestions going forward
        void OnTipCompleted(object sender, EventArgs e)
        {
            // Sanity check inputs
            if (!double.TryParse(((Entry)sender).Text, out tip))
                tip = 0;
            else
                tip = double.Parse(((Entry)sender).Text, System.Globalization.NumberStyles.Currency);

            if (tip < 0) // No negative tips (lol)
                tip = 0;

            tipChanged = true;
            // Round to two decimal places, then update the textboxes
            tip = Math.Round(tip, 2);

            tipEntry.Text = tip.ToString("C");

            OnContributionCompleted();
        }


        void OnContributionCompleted()
        {
            // Suggest tip if not specified
            if (!tipChanged)
            {
                tip = 0;
                tipEntry.Placeholder = (contribution * 0.2).ToString("C");
            }

            // Update buttons
            if ((contribution + tip) > 0)
                payButton.Text = "Pay " + (contribution + tip).ToString("C");
            else
                payButton.Text = "No Contribution";
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
            // Reset all items to unpaid
            foreach (MenuFoodItem m in RealmManager.All<Order>().FirstOrDefault().Contents)
                RealmManager.Write(() =>
                {
                    m.paid = false;
                });

            // Fetch most recent order status
            

            contribution = 0;

            OnContributionCompleted();

            //menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().Contents.Where((MenuFoodItem m) => m.paid == false).ToList();
        }

        void OnTogglePaid(object sender, ToggledEventArgs e)
        {
            // Don't do anything if Realm is writing
            if (RealmManager.Realm.IsInTransaction)
                return;

            string toggledID = ((MenuFoodItem)((ViewCell)(((Switch)sender).Parent.Parent.Parent)).BindingContext)._id;
            

            // Update food item and contribution according to toggled/not toggled
            if (e.Value)
            {
                RealmManager.Write(() => RealmManager.Find<MenuFoodItem>(toggledID).paid = true);

                contribution += RealmManager.Find<MenuFoodItem>(toggledID).price;
                contribution = Math.Round(contribution, 2);
            }
            else
            {
                RealmManager.Write(() => RealmManager.Find<MenuFoodItem>(toggledID).paid = false);

                contribution -= RealmManager.Find<MenuFoodItem>(toggledID).price;
                contribution = Math.Round(contribution, 2);
                if (contribution < 0)
                    contribution = 0;
            }
            OnContributionCompleted();
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