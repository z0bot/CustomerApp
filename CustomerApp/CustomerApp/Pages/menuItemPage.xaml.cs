﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class menuItemPage : ContentPage
    {
        MenuFoodItem item;
        public menuItemPage(string itemID)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Get this item's details
            item = new MenuFoodItem(RealmManager.Find<MenuFoodItem>(itemID));
            // Assign item new ID
            item._id += new Random().Next(0, 1000000);
            nameLabel.Text = item.name;
            descLabel.Text = item.description;
            itemPic.Source = item.picture;
            priceLabel.Text = item.StringPrice;
        }

        async void OnNutritionButtonClicked(object sender, EventArgs e)
        {
            // Display nutrition info
            // **** Maybe make a separate page? Look into this: https://github.com/rotorgames/Rg.Plugins.Popup

            await DisplayAlert("Nutrition info", item.nutrition, "OK");
        }
        async void OnAddInstructionsClicked(object sender, EventArgs e)
        {
            // Prompt for special instructions

            item.special_instruct = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.special_instruct);
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            //Store item into local database
            RealmManager.Write(() => 
            {
                RealmManager.Realm.All<Order>().FirstOrDefault().Contents.Add(item);
            });
            

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