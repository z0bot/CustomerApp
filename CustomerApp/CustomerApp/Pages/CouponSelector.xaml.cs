using CustomerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models.ServiceRequests;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CouponSelectorPage : ContentPage
    {
        double contribution = 0, tip = 0;

        public CouponSelectorPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Necessary for the refreshview to work
            System.Windows.Input.ICommand cmd = new Command(onRefresh);
            orderRefreshView.Command = cmd;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DisplayCoupons();
        }

        async void OnFinalizeButtonClicked(object sender, EventArgs e)
        {

            await Navigation.PopModalAsync();
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


        /// <summary>
        /// Pulls the most recent order status, then assigns that to the items list's itemsSource
        /// Also resets contribution to 0
        /// </summary>
        /// <returns></returns>
        public async Task DisplayCoupons()
        {
            orderRefreshView.IsEnabled = false;

            //await GetCouponsRequest.SendGetCouponsRequest();

            CouponsView.ItemsSource = RealmManager.All<User>().FirstOrDefault().coupons.ToList();

            orderRefreshView.IsEnabled = true;
        }

        /// <summary>
        /// Called upon toggling a switch.
        /// Note that the toggle itself is tied to an order item's paid attribute, so we do not need ot manually change it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnToggleSelected(object sender, ToggledEventArgs e)
        {
            // Don't do anything if Realm is writing
            if (RealmManager.Realm.IsInTransaction)
                return;

            Coupon toggledItem = (Coupon)(((ViewCell)(((Switch)sender).Parent.Parent.Parent)).BindingContext);

            // Error checking
            if (toggledItem._id == null)
            {
                await DisplayCoupons();
                return;
            }

        }

        async void onRefresh()
        {
            // Pull newest order status
            await DisplayCoupons();

            orderRefreshView.IsRefreshing = false;
        }
    }
}