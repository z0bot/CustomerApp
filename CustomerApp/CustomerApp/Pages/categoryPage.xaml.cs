using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;

namespace CustomerApp.Pages
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class categoryPage : ContentPage
    {
        string category;
        List<MenuFoodItem> members;
        List<Ingredient> ingredients; // Used to filter items which we don't have enough ingredients for
        List<Button> buttons;

        public categoryPage(string categoryName)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            categoryLabel.Text = categoryName;

            category = categoryName;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetIngredientsRequest.SendGetIngredientsRequest();
            ingredients = RealmManager.All<IngredientList>().FirstOrDefault().doc.ToList();
            PopulateMenu();
        }

        void PopulateMenu()
        {
            // Clear existing categories
            while (categoryItemList.Children.Count != 0)
                categoryItemList.Children.RemoveAt(0);

            buttons = new List<Button>();

            // Get menu items within this category.
            members = RealmManager.All<MenuItemsList>().FirstOrDefault().menuItems.Where((MenuFoodItem m) => m.category == category).ToList();

            foreach(MenuFoodItem m in members)
            {
                foreach(Ingredient i in m.ingredients)
                {
                    // Remove a member if we don't have the necessary ingredients
                    var index = ingredients.FindIndex((Ingredient j) => j._id == i._id && j.quantity > 0);
                    if(index == -1)
                    {
                        members.Remove(m);
                        break;
                    }
                }
            }

            // Create buttons for each category member
            for (int i = 0; i < members.Count; ++i)
            {
                Button temp;
                string itemName = members[i].name;
                string itemID = members[i]._id;
                categoryItemList.Children.Add(temp = (new Button()
                {
                    Text = members[i].name + " | " + members[i].StringPrice,
                    Margin = new Thickness(20, 0, 20, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                }));

                temp.Clicked += async (sender, args) => await Navigation.PushAsync(new menuItemPage(itemID));

                buttons.Add(temp);
            }


            if (members.Count() == 0)
            {
                categoryItemList.Children.Add(new Label()
                {
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Header, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(15, 15, 15, 15),
                    Text = "We've come up empty!"
                });

                categoryItemList.Children.Add(new Label()
                {
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    Margin = new Thickness(15, 15, 15, 15),
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "We're all out of the ingredients for the items in this category. Please use the back button and look at a different category"
                });
            }
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