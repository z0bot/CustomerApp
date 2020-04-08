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
    public partial class YourOrderPage : ContentPage
    {
        public YourOrderPage()
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

        async void OnSendOrderClicked(object sender, EventArgs e)
        {
            //Navigate to order confirmation page
            if (await DisplayAlert("WARNING: Sending Order", "Are you sure you want to send the order? Current items cannot be changed by anyone at the table.", "Yes", "No"))
            {
                // Set order status to 'sent'
                //RealmManager.Write(() => RealmManager.All<Order>().FirstOrDefault().send_to_kitchen = true);
                await SendOrderRequest.SendSendOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                await Navigation.PushAsync(new checkoutPage());
            }
        }

        // Replaces send order button after order is sent
        async void OnPayClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new checkoutPage());
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            //Navigate to menu

            await Navigation.PushAsync(new menuPage());
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

        //// Disable back button for this page with a confirmation warning
        //protected override bool OnBackButtonPressed()
        //{
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        if (await DisplayAlert("Confirm Leaving page", "Are you sure you want to leave this page?", "Yes", "No"))
        //        {
        //            try
        //            {
        //                await Navigation.PopAsync();
        //            }
        //            catch
        //            {
        //                await DisplayAlert("CAUGHT", "You suck", "OK");
        //            }
        //        }
        //    });
        //    return true;
        //}

        async void OnEditItemInvoked(object sender, EventArgs e)
        {
            OrderItem item = new OrderItem((OrderItem)(((SwipeItem)sender).BindingContext));

            await DisplayOrder();

            // Don't allow edits for sent items. Might change this to check if the item is prepared later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");
                return;
            }
            
            string instructions = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.special_instruct);
            
            // Don't send request if nothing changed
            if (instructions == item.special_instruct)
                return;

            OrderItem preUpdateItem = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.special_instruct == item.special_instruct && o._id == item._id).FirstOrDefault();

            if (preUpdateItem != null)
            {
                RealmManager.Write(() => preUpdateItem.special_instruct = instructions);

                await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
            }
            else
            {
                await DisplayAlert("Something went wrong", "Sorry, but we were unable to edit that item's description. Most likely, it was removed by another user at your table. Please verify if the item is gone, then re-add it to your order", "OK");
            }
            await DisplayOrder();
        }

        async void OnRemoveItemInvoked(object sender, EventArgs e)
        {
            OrderItem item = new OrderItem((OrderItem)(((SwipeItem)sender).BindingContext));

            await DisplayOrder();

            // Don't allow removal of sent items. Might change this to check if the item is prepared later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");
                return;
            }

            OrderItem preUpdateItem = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.special_instruct == item.special_instruct && o._id == item._id).FirstOrDefault();
            
            // Remove item if it was not already removed
            if(preUpdateItem != null)
            {
                RealmManager.Write(() => RealmManager.All<Order>().FirstOrDefault().menuItems.Remove(preUpdateItem));

                await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
            }


            await DisplayOrder();
        }

        // This and enableRefresh are used in an attempt to make it easier to use the swipeview for each item
        async void disableRefresh(object sender, SwipeStartedEventArgs e)
        {
            orderRefreshView.IsEnabled = false;
        }

        async void enableRefresh(object sender, SwipeEndedEventArgs e)
        {
            orderRefreshView.IsEnabled = true;
        }

        async void onRefresh()
        {
            // Pull newest order status
            await DisplayOrder();

            orderRefreshView.IsRefreshing = false;
        }

        public async Task DisplayOrder()
        {
            // Fetch most recent version of the order
            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().menuItems.ToList();

            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                // Reset button
                sendOrderButton.Clicked -= OnSendOrderClicked;
                sendOrderButton.Clicked -= OnPayClicked;
                // Add new functionality
                sendOrderButton.Clicked += OnPayClicked;
                sendOrderButton.Text = "Payment";
            }
        }
    }

}