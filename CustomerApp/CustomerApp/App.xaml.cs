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

            if (!RealmManager.All<User>().Count().Equals(0))
            {
                // Make sure user has a table number
                if (RealmManager.All<User>().FirstOrDefault().tableNum != -1)
                {
                    if (RealmManager.All<User>().FirstOrDefault().paymentInProgress)
                    {
                        MainPage = new NavigationPage(new paymentPage(RealmManager.All<User>().FirstOrDefault().contribution, RealmManager.All<User>().FirstOrDefault().tip));
                    }
                    else
                        MainPage = new NavigationPage(new orderHerePage());
                }
                else
                    MainPage = new NavigationPage(new Login());
            }
            else
                MainPage = new NavigationPage(new Login());

            //if an order exists go to YourOrderPage
            //else no order but user logged on, OrderHerePage
            //else LoginPage
            //if(!RealmManager.All<Order>().Count().Equals(0))
            //{
            //    // If an order exists and has already been sent, default to the checkout page
            //    if(RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            //    {
            //        MainPage = new NavigationPage(new checkoutPage());
            //    }
            //    else
            //    {
            //        MainPage = new NavigationPage(new YourOrderPage());

            //        //gets access to navigation stack
            //        INavigation navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;
            //        //gets current page (i.e. MainPage)
            //        Page currentpage = navigation.NavigationStack.ElementAt(navigation.NavigationStack.Count - 1);
            //        //gets new orderHerePage
            //        Page orderHerePage = new orderHerePage();
            //        //now have the ability to back from YourOrder to orderHerePage
            //        navigation.InsertPageBefore(orderHerePage, currentpage);
            //    }

            //}
            //if(!RealmManager.All<User>().Count().Equals(0))
            //{
            //    // Make sure user has a table number
            //    if(RealmManager.All<User>().FirstOrDefault().tableNum != -1)
            //    {
            //        if (RealmManager.All<User>().FirstOrDefault().paymentInProgress)
            //        {
            //            MainPage = new NavigationPage(new paymentPage(RealmManager.All<User>().FirstOrDefault().contribution, RealmManager.All<User>().FirstOrDefault().tip));
            //        }
            //        else if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen) // Order sent to kitchen
            //        {
            //            MainPage = new NavigationPage(new checkoutPage());

            //            //gets access to navigation stack
            //            INavigation navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;

            //            //gets current page (i.e. MainPage)
            //            Page currentpage = navigation.NavigationStack.ElementAt(navigation.NavigationStack.Count - 1);
            //            //gets new orderHerePage
            //            Page YourOrderPage = new YourOrderPage();

            //            //now have the ability to back from checkout to yourOrder
            //            navigation.InsertPageBefore(YourOrderPage, currentpage);

            //            //gets new orderHerePage
            //            Page orderHerePage = new orderHerePage();

            //            //now have the ability to back from yourOrder to orderhere
            //            navigation.InsertPageBefore(orderHerePage, YourOrderPage);
            //        }
            //        else if (!RealmManager.All<Order>().FirstOrDefault().menuItems.Count().Equals(0)) // Not sent to kitchen, but items exist
            //        {
            //            MainPage = new NavigationPage(new YourOrderPage());

            //            //gets access to navigation stack
            //            INavigation navigation = Xamarin.Forms.Application.Current.MainPage.Navigation;
            //            //gets current page (i.e. MainPage)
            //            Page currentpage = navigation.NavigationStack.ElementAt(navigation.NavigationStack.Count - 1);
            //            //gets new orderHerePage
            //            Page orderHerePage = new orderHerePage();
            //            //now have the ability to back from YourOrder to orderHerePage
            //            navigation.InsertPageBefore(orderHerePage, currentpage);
            //        }
            //    }
            //    else
            //        MainPage = new NavigationPage(new Login());
            //}
            //else
            //    MainPage = new NavigationPage(new Login());
        }

        public async void updateOrder()
        {
            await CustomerApp.Models.ServiceRequests.GetTableRequest.SendGetTableRequest(RealmManager.All<User>().FirstOrDefault().tableNum);
            await CustomerApp.Models.ServiceRequests.GetMenuItemsRequest.SendGetMenuItemsRequest();
        }

        protected override async void OnStart()
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
