using CustomerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models.ServiceRequests;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class orderHerePage : ContentPage
    {
        public static Notifications notifications;
        public orderHerePage()
        {
            InitializeComponent();

            //will get and store initial details about a table in Realm for notification use
            //such as employee id and table number
            MakeTableRequestOnStart();
            
        }
        public async Task MakeTableRequestOnStart()
        {
            await GetTableRequest.SendGetTableRequest(RealmManager.All<User>().FirstOrDefault().tableNum);
            GetNotificationParameters();
        }
        public void GetNotificationParameters()
        {
            RealmManager.Write(() =>
            {
                notifications.employee_id = RealmManager.All<Table>().FirstOrDefault().employee_id;
                notifications.sender = RealmManager.All<Table>().FirstOrDefault().tableNumberString;
            });
        }
        /// <summary>
        /// Gets the order associated with the current table, then move to YourOrder page
        /// </summary>
        async void OnOrderHereButtonClicked(object sender, EventArgs e)
        {
            // Clear existing data
            RealmManager.RemoveAll<Order>();
            RealmManager.RemoveAll<Table>();

            // Pull existing orders for this table
            await GetTableRequest.SendGetTableRequest(RealmManager.All<User>().FirstOrDefault().tableNum);

            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Table>().FirstOrDefault().order_id._id);

            await Navigation.PushAsync(new YourOrderPage());
        }
        async void OnRefillButtonClicked(object sender, EventArgs e)
        {
            string notificationType = "Refill";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);
            await DisplayAlert("Refill", "Server Notified of Refill Request", "OK");
        }

        async void OnServerButtonClicked(object sender, EventArgs e)
        {
            // Send Help Request
            string notificationType = "Help requested";
            await PostNotificationsRequest.SendNotificationRequest(notificationType, RealmManager.All<Table>().FirstOrDefault().employee_id, RealmManager.All<Table>().FirstOrDefault().tableNumberString);
            await DisplayAlert("Help Request", "Server Notified of Help Request", "OK");
        }

        /// <summary>
        /// Completely logs the user out of the app, theoretically resetting it to the same as the first boot
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await DisplayAlert("LOGOUT", "Are you sure you want to logout from this table?", "Yes", "No"))
                {
                    RealmManager.RemoveAll<MenuFoodItem>();
                    RealmManager.RemoveAll<Order>();
                    RealmManager.RemoveAll<Table>();
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