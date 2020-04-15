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
            List<Coupon> selectedList = new List<Coupon>();

            foreach (Coupon c in RealmManager.All<User>().FirstOrDefault().coupons.Where((Coupon c) => c.selected).ToList())
                selectedList.Add(new Coupon(c));

            // Get most recent coupons list
            await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);

            //CouponsView.ItemsSource = null;

            RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().coupons.Clear());

            foreach (Coupon c in selectedList)
            {
                c.selected = true;
                RealmManager.AddOrUpdate<Coupon>(c);
            }


            await Navigation.PopAsync();
        }

        async void OnRefillButtonClicked(object sender, EventArgs e)
        {
            // Send refill request
            string notificationType = "Refill";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);

            await DisplayAlert("Refill", "Server Notified of Refill Request", "OK");
        }

        async void OnServerButtonClicked(object sender, EventArgs e)
        {
            // Send Help Request
            string notificationType = "Help requested";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);
            await DisplayAlert("Help Request", "Server Notified of Help Request", "OK");
        }

        async void AddCouponClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCouponPage());
        }

        /// <summary>
        /// Pulls the most recent coupons list, then assigns that to the coupon list's itemsSource
        /// Also resets contribution to 0
        /// </summary>
        /// <returns></returns>
        public async Task DisplayCoupons()
        {
            orderRefreshView.IsEnabled = false;

            // Get most recent user data (including coupons)
            await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);

            CouponsView.ItemsSource = RealmManager.All<Coupon>().Where((Coupon c) => c.couponType == "Customer" && !c.selected).ToList();

            orderRefreshView.IsEnabled = true;
        }

        async void OnApplyCouponPressed(object sender, EventArgs e)
        {
            Coupon selectedCoupon = new Coupon((Coupon)(((ViewCell)(((ImageButton)sender).Parent.Parent.Parent)).BindingContext));

            if (!selectedCoupon.selected)
            {
                selectedCoupon.selected = true;

                RealmManager.AddOrUpdate<Coupon>(selectedCoupon);

                RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().coupons.Remove(selectedCoupon));

                await UpdateCouponsRequest.SendUpdateCouponsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().coupons);

                await DisplayAlert("Coupon Succesfully Applied!", "You have succesfully applied this coupon to your order!", "OK");
            }
            else
            {
                await DisplayAlert("Coupon Already Applied", "Sorry, but this coupon has already been applied. Please select a different coupon", "OK");
            }

            await DisplayCoupons();
        }


        async void onRefresh()
        {
            // Pull newest order status
            await DisplayCoupons();

            orderRefreshView.IsRefreshing = false;
        }

    }
}