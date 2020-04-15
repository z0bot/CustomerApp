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
    public partial class PayWithPointsPage : ContentPage
    {

        public PayWithPointsPage()
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
            await DisplayOrder();
        }

        /// <summary>
        /// Prompts user if they want to use their points to get this item for no charge, if they have sufficient points.
        /// Simply sets this item's 'paid' attribute to true, then reduces the points total by the requisite amount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnPointsPressed(object sender, EventArgs e)
        {
            OrderItem item = new OrderItem((OrderItem)(((ViewCell)(((ImageButton)sender).Parent.Parent.Parent)).BindingContext));
            if ((int)RealmManager.All<User>().FirstOrDefault().points >= (int)(item.price * PointsPerDollar.rate))
            {
                if (await DisplayAlert("Use points?", "You can use " + item.price * PointsPerDollar.rate + " of your " + RealmManager.All<User>().FirstOrDefault().points + " points to get this item for free!\n"
                    + "Would you like to redeem points in exchange for this item?\n"
                    + "NOTE: Points cannot be refunded after being redeemed.", "Yes, please!", "No thanks"))
                {
                    // Get most recent order status
                    await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                    item = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == item._id).FirstOrDefault();

                    if(item.paid)
                    {
                        await DisplayAlert("Item already paid for", "This item was paid for by someone else already.\n\nNo points have been deducted from your account", "OK");
                        await DisplayOrder();
                        return;
                    }

                    //Store item into local database
                    RealmManager.Write(() =>
                    {
                        item.paid = true;
                    });

                    // Update points total
                    await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);
                    await UpdatePointsRequest.SendUpdatePointsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().points - (item.price * PointsPerDollar.rate));

                    // Send updated order
                    await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
                }
            }
            else
                await DisplayAlert("Insufficient points", "Sorry, but you need " + item.price * PointsPerDollar.rate + " points to get this item for free.\n\n"
                    + "Your current points total is " + RealmManager.All<User>().FirstOrDefault().points + "\n\n"
                    + "Only " + ((item.price * PointsPerDollar.rate) - RealmManager.All<User>().FirstOrDefault().points) + " to go!", "OK");

            await DisplayOrder();
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

        async void OnReturnButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Pulls the most recent order status, then assigns that to the items list's itemsSource
        /// Also resets contribution to 0
        /// </summary>
        /// <returns></returns>
        public async Task DisplayOrder()
        {
            orderRefreshView.IsEnabled = false;
            menuFoodItemsView.IsEnabled = false;

            // Fetch most recent order status
            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);


            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid).ToList();

            await Task.Delay(500); // Try to fix crashes related to toggling rows before the source is finished updating

            orderRefreshView.IsEnabled = true;
            menuFoodItemsView.IsEnabled = true;
        }


        async void onRefresh()
        {
            // Pull newest order status
            await DisplayOrder();

            orderRefreshView.IsRefreshing = false;
        }
    }
}