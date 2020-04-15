using CustomerApp.Models;
using CustomerApp.Pages;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// Define a constant rate of points per dollar of 1 dollar = 100 points. Conversely, 1 cent = 1 point
// Could also define a separate ratio of dollars per point, i.e. 10000 points have the value of 1 dollar for redeeming purposes
class PointsPerDollar 
{
    public const int rate = 100;
}

namespace CustomerApp
{
    
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if(RealmManager.All<BirthdayBool>().Count().Equals(0))
                RealmManager.AddOrUpdate<BirthdayBool>(new BirthdayBool()); // Used to keep track of if birthday gift has been claimed before

            // Page persistence based on if user is logged in
            if (!RealmManager.All<User>().Count().Equals(0))
            {
                // Make sure user has a table number
                if (RealmManager.All<User>().FirstOrDefault().tableNum != -1)
                {
                    if (RealmManager.All<User>().FirstOrDefault().paymentInProgress) // Lock user to payment page if they have not yet finished paying
                    {
                        MainPage = new NavigationPage(new paymentPage());
                    }
                    else // Go to Order Here page
                    {
                        MainPage = new NavigationPage(new orderHerePage());
                    }
                }
                else
                {
                    MainPage = new NavigationPage(new Login());
                }
            }
            else
            {
                MainPage = new NavigationPage(new Login());
            }
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
