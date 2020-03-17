using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    public class categoryLink
    {
        public string name;

        public Button buttonInfo { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class menuPage : ContentPage
    {
        

        public List<categoryLink> category;
        public menuPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            category = new List<categoryLink>();

            List<string> categoryNames = new List<string>() { "Appetizers", "Entrees", "Sides", "Desserts", "Beverages", "Appetizers", "Entrees", "Sides", "Desserts", "Beverages" }; // *** Temporary categories. Doubled up to demonstrate scrollview

            for (int i = 0; i < categoryNames.Count; ++i)
            {
                categoryLink newCat = new categoryLink();

                newCat.name = categoryNames[i];

                categoryList.Children.Add(newCat.buttonInfo = (new Button()
                {
                    Text = newCat.name,
                    Margin = new Thickness(30, 0, 30, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                }));

                newCat.buttonInfo.Clicked += async (sender, args) => await DisplayAlert("Navigation", newCat.name, "OK");

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