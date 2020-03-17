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
    public partial class menuItemPage : ContentPage
    {
        Models.MenuItem item;
        public menuItemPage(string itemName)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            // Get this item's details
            item = new Models.MenuItem() { Name = itemName, Picture = "goodFood", Description = "Description of " + itemName, Nutrition = "Calories: hella\nFat: hella\nIngredients: hellman's", Price = 3.50, SpecialInstructions = null };


            nameLabel.Text = item.Name;
            descLabel.Text = item.Description;
            itemPic.Source = item.Picture;
            priceLabel.Text = "$" + item.Price.ToString();
        }

        async void OnNutritionButtonClicked(object sender, EventArgs e)
        {
            // Display nutrition info
            // **** Maybe make a separate page? Look into this: https://github.com/rotorgames/Rg.Plugins.Popup

            await DisplayAlert("Nutrition info", item.Nutrition, "OK");
        }
        async void OnAddInstructionsClicked(object sender, EventArgs e)
        {
            // Prompt for special instructions

            item.SpecialInstructions = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.SpecialInstructions);
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            // Send item to order


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