using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;
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
    public partial class gamePage : ContentPage
    {
        int winner;
        public gamePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Randomly select a number between 1 and 5, inclusively
            winner = new Random(System.DateTime.Now.Second).Next(1, 5);
        }

        /// <summary>
        /// Uses clicked button's text (A number between 1 and 5) to determine if choice was correct
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void onClicked(object sender, EventArgs e)
        {
            if(((Button)sender).Text == winner.ToString())
            {
                await DisplayAlert("Congratulations!", "Correct! The number was " + winner.ToString() + ".\n"
                    + "A coupon for your free dessert has been added to your account", "Yay!");

                // Add new coupon to account
                // Get most recent user data (including coupons)
                await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);

                // Post coupon, add it to the local account, then update remote database
                string ID = await PostDessertCouponRequest.SendPostDessertCouponRequest();
                if (ID != null)
                {
                    await GetCouponsByIDRequest.SendGetCouponsByIDRequest(ID);
                    RealmManager.Write(() => RealmManager.All<User>().FirstOrDefault().coupons.Add(RealmManager.Find<Coupon>(ID)));
                    await UpdateCouponsRequest.SendUpdateCouponsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().coupons);
                }
                else
                {
                    await DisplayAlert("Something went wrong", "Sorry, but something has gone wrong on our end. Please contact your waitstaff and show them this message so we can make this right", "OK");
                }

            }
            else
            {
                await DisplayAlert("Sorry!", "You guessed wrong. The number was " + winner.ToString() + ".", "Aww!");
            }

            Navigation.InsertPageBefore(new endPage(), this);
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

        // Prevent going back to previous pages, must complete game to continue
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}