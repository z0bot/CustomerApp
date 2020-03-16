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
    public partial class menuPage : ContentPage
    {
        public class categoryLink
        {
            public string name;

            public Button buttonInfo { get; set; }
        }

        public List<categoryLink> category;
        public menuPage()
        {
            InitializeComponent();

            List<string> categoryNames = new List<string>() { "Appetizers", "Entrees", "Sides", "Desserts", "Beverages" }; // *** Temporary categories

            for(int i = 0; i < categoryNames.Count(); ++i)
            {
                categoryLink newCat = new categoryLink();

                categoryList.Children.Add(newCat.buttonInfo = (new Button() 
                    { 
                        Text = categoryNames[i], 
                        Margin = new Thickness(30,0,30,20),
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.White,
                        WidthRequest = 140,
                        BackgroundColor = Color.FromHex("0x1fbd85")
                    }));

                newCat.name = categoryNames[i];

                newCat.buttonInfo.Clicked += async (sender, args) => DisplayAlert("Navigation", newCat.name, "OK");

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