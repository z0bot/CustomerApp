using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models.ServiceRequests;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class menuItemPage : ContentPage
    {
        
        OrderItem item; // The item as it will appear in the order
        MenuFoodItem baseItem; // The base item that will be copied into item
        
        /// <summary>
        /// Constructor for the menuItemPage, taking in a menuFoodItem's ID. Assumes that the ID exists
        /// Creates an orderItem copy of the associated menuItem, assigning it a unique key
        /// </summary>
        /// <param name="itemID"></param>
        public menuItemPage(string itemID)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Get this item's details
            baseItem = new MenuFoodItem(RealmManager.Find<MenuFoodItem>(itemID));

            item = new OrderItem(baseItem);
            //Assign item new ID
            var rand = new Random();
            item.newID = (rand.Next(0, 1000000000)).ToString();
            while (RealmManager.Find<OrderItem>(item.newID) != null) // If the ID already exists, try again until you get a unique ID.
            {
                item.newID = (rand.Next(0, 1000000000)).ToString();
            }

            // Update labels
            nameLabel.Text = baseItem.name;
            descLabel.Text = baseItem.description;
            itemPic.Source = baseItem.picture;
            priceLabel.Text = baseItem.StringPrice;
            item.special_instruct = null;
        }

        async void OnNutritionButtonClicked(object sender, EventArgs e)
        {
            // Display nutrition info
            // **** Maybe make a separate page? Look into this: https://github.com/rotorgames/Rg.Plugins.Popup

            await DisplayAlert("Nutrition info", baseItem.nutrition, "OK");
        }

        /// <summary>
        /// Add special instructions to the item (NOT the base item)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnAddInstructionsClicked(object sender, EventArgs e)
        {
            // Prompt for special instructions

            item.special_instruct = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.special_instruct);
        }

        /// <summary>
        /// If the order has not been sent, add this item to it. Then return to the yourOrder page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnAddItemClicked(object sender, EventArgs e)
        {
            // Remove ability to add items after an order is sent. Easy to change later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");

                LeavePage();
                return;
            }

            // Get most recent order status
            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

            //Store item into local database
            await AddToRealm(item);

            // Send update order
            await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());

            LeavePage();
        }

        async Task AddToRealm(OrderItem item)
        {
            RealmManager.Write(() =>
            {
                RealmManager.Realm.All<Order>().FirstOrDefault().menuItems.Add(item);
            });
        }

        /// <summary>
        /// Used to formally leave this page upon adding an item (Or failing to add one, if the order has been sent)
        /// </summary>
        async void LeavePage()
        {
            //Navigate back to menu. Probably a more elegant method but is easy to do. Remove previous 2 pages, then pop
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Prompts user if they want to use their points to get this item for no charge, if they have sufficient points.
        /// Simply adds this item to the order with the 'paid' property already true, then deducts the requisite number of points from the user's account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnPointsClicked(object sender, EventArgs e)
        {

            if ((int)RealmManager.All<User>().FirstOrDefault().points >= (int)(baseItem.price * PointsPerDollar.rate))
            {
                if(await DisplayAlert("Use points?", "You can use " + baseItem.price * PointsPerDollar.rate + " of your " + RealmManager.All<User>().FirstOrDefault().points + " points to get this item for free!\n"
                    + "Would you like to redeem points in exchange for this item?\n"
                    + "NOTE: Points cannot be refunded after being redeemed.", "Yes, please!", "No thanks"))
                {
                    // Get most recent order status
                    await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                    //Store item into local database
                    RealmManager.Write(() =>
                    {
                        item.paid = true;
                        RealmManager.Realm.All<Order>().FirstOrDefault().menuItems.Add(item);
                    });

                    // Send updated order
                    await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());


                    // Update points total
                    await UserAuthenticationRequest.SendUserAuthenticationRequest(RealmManager.All<User>().FirstOrDefault().email, RealmManager.All<User>().FirstOrDefault().password);
                    await UpdatePointsRequest.SendUpdatePointsRequest(RealmManager.All<User>().FirstOrDefault()._id, RealmManager.All<User>().FirstOrDefault().points - (baseItem.price * PointsPerDollar.rate));

                    LeavePage();
                }
            }
            else
                await DisplayAlert("Insufficient points", "Sorry, but you need " + baseItem.price * PointsPerDollar.rate + " points to get this item for free.\n\n"
                    + "Your current points total is " + RealmManager.All<User>().FirstOrDefault().points + "\n\n"
                    + "Only " + ((baseItem.price * PointsPerDollar.rate) - RealmManager.All<User>().FirstOrDefault().points) + " to go!", "OK");
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