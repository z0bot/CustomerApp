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
    public partial class categoryPage : ContentPage
    {
        List<Models.MenuItem> members;
        List<Button> buttons;

        public categoryPage(string categoryName)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            categoryLabel.Text = categoryName;

            buttons = new List<Button>();

            // Get menu items within this category.

            // Generate fake members until database can be used
            members = new List<Models.MenuItem>();
            for (int i = 0; i < 10; ++i) 
            {
                members.Add(new Models.MenuItem() { Name = categoryName + i.ToString(), Price = 3.50, Description = "Ehhh", SpecialInstructions = null });
            }

            // Create buttons for each category member
            for(int i = 0; i < members.Count; ++i)
            {
                Button temp;
                menuItemList.Children.Add(temp = (new Button()
                {
                    Text = members[i].Name + " | $" + members[i].Price,
                    Margin = new Thickness(30, 0, 30, 20),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    WidthRequest = 140,
                    BackgroundColor = Color.FromHex("1fbd85"),
                    CornerRadius = 15
                }));
                buttons.Add(temp);
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