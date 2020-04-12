using CustomerApp.Models;
using CustomerApp.Pages;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Page persistence based on if user is logged in
            if (!RealmManager.All<User>().Count().Equals(0))
            {
                // Make sure user has a table number
                if (RealmManager.All<User>().FirstOrDefault().tableNum != -1)
                {
                    if (RealmManager.All<User>().FirstOrDefault().paymentInProgress) // Lock user to payment page if they have not yet finished paying
                    {
                        MainPage = new NavigationPage(new paymentPage(RealmManager.All<User>().FirstOrDefault().contribution, RealmManager.All<User>().FirstOrDefault().tip));
                    }
                    else // Go to Order Here page
                        MainPage = new NavigationPage(new orderHerePage());
                }
                else
                    MainPage = new NavigationPage(new Login());
            }
            else
                MainPage = new NavigationPage(new Login());
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
        }
    }
}
