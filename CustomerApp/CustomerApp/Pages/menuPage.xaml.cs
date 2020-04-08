using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    public class categoryLink
    {
        public string name;

        public Button ButtonInfo { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class menuPage : ContentPage
    {
        
        public List<categoryLink> category;
        public menuPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateMenuItems();
            PopulateCategories();
        }

        async Task UpdateMenuItems()
        {
            RealmManager.RemoveAll<MenuItemsList>();
            var success = await GetMenuItemsRequest.SendGetMenuItemsRequest();
            
        }

        void PopulateCategories()
        {
            // Clear existing categories
            while(categoryList.Children.Count != 0)
                categoryList.Children.RemoveAt(0);

            category = new List<categoryLink>();

            List<string> categoryNames = new List<string>();

            foreach(MenuFoodItem m in RealmManager.All<MenuItemsList>().FirstOrDefault().menuItems)
            {
                categoryNames.Add(m.category);
            }

            categoryNames = categoryNames.Distinct().ToList();

            for (int i = 0; i < categoryNames.Count; ++i)
            {
                categoryLink newCat = new categoryLink
                {
                    name = categoryNames[i]
                };

                categoryList.Children.Add(newCat.ButtonInfo = (new Button()
                {
                    Text = newCat.name,
                    Margin = new Thickness(30, 0, 30, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                }));

                newCat.ButtonInfo.Clicked += async (sender, args) => await Navigation.PushAsync(new categoryPage(newCat.name));
                category.Add(newCat);
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