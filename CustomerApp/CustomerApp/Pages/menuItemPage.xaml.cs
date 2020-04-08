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
        OrderItem item;
        MenuFoodItem baseItem;
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
        async void OnAddInstructionsClicked(object sender, EventArgs e)
        {
            // Prompt for special instructions

            item.special_instruct = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.special_instruct);
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            // Remove ability to add items after an order is sent. Easy to change later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");

                //Navigate back to menu. Probably a more elegant method but is easy to do. Remove previous 2 pages, then pop
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                await Navigation.PopAsync();
                return;
            }
            //Store item into local database
            RealmManager.Write(() => 
            {
                RealmManager.Realm.All<Order>().FirstOrDefault().menuItems.Add(item);
            });

            await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());

            //Navigate back to menu. Probably a more elegant method but is easy to do. Remove previous 2 pages, then pop
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            await Navigation.PopAsync();
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