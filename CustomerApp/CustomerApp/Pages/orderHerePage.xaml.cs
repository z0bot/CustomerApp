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

            // Pull existing orders for this table
            // GetOrderRequest.SendGetOrderRequest();

            Order currentOrder;

            if (RealmManager.All<Order>().Count() != 0)
                currentOrder = RealmManager.All<Order>().FirstOrDefault();
            else
            {
                currentOrder = new Order();
                // Get which table we are at
                int table;
                // table = RealmManager.All<User>().FirstOrDefault().table_num;
                table = 2; // TEMPORARILY set table to 2. Only used for testing

                currentOrder.tableNum = table;

                // Find which waitstaff is in charge of this table
                // GetTablesRequest.SendGetTablesRequest();
                string employeeID;
                // employeeID = RealmManager.Find<Table>().Where((Table t) => t.table_number == table).FirstOrDefault().employee_id;
                employeeID = "5e850b90c849ed00047b4ec9"; // TEMPORARILY assign this for testing. This should be Zach's employee ID (because he was first in the DB)

                currentOrder.waitstaff_id = employeeID;
                currentOrder.sent = false;

                RealmManager.AddOrUpdate<Order>(currentOrder);
            }

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