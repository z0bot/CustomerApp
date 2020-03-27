using CustomerApp.Models;
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
    public partial class orderHerePage : ContentPage
    {
        public orderHerePage()
        {
            InitializeComponent();
        }

        async void OnOrderHereButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new YourOrderPage());
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

        // Disable back button for this page
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("LOGOUT", "Are you sure you want to logout from this table?", "Yes", "No"))
                {
                    RealmManager.RemoveAll<MenuFoodItem>();
                    RealmManager.RemoveAll<User>();
                    await InsertPageBeneathAndPop();
                }
            });
            return true;
        }
        /// <summary>
        /// Due to dynamic MainPage states, this allows the user to logout from OrderHerePage
        /// safely and creates a new instance of the login page in case there wasn't one before
        /// </summary>
        /// <returns></returns>
        private async Task InsertPageBeneathAndPop()
        {
            //get current app navigation stack
            INavigation navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;
            //get current page
            Page currentpage = navigation.NavigationStack.ElementAt(navigation.NavigationStack.Count - 1);

            //remove anything beneath current page. Should always be no more than 3 things in stack at this point
            while(navigation.NavigationStack.ElementAt(0) != currentpage)
            {
               navigation.RemovePage(navigation.NavigationStack.ElementAt(0));
            }

            Page LoginPage = new Login();
            //insert loginPage beneath current page and pop to login page
            navigation.InsertPageBefore(LoginPage, currentpage);
            await System.Threading.Tasks.Task.Delay(100);
            await navigation.PopAsync();
        }

    }
}