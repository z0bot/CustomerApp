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
    public partial class checkoutPage : ContentPage
    {
        double contribution = 0, tip = 0;

        public checkoutPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
             
            // Necessary for the refreshview to work
            System.Windows.Input.ICommand cmd = new Command(onRefresh);
            orderRefreshView.Command = cmd;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DisplayOrder();
        }


        /// <summary>
        /// Get the most recent order contents, then update them with what is paid for locally.
        /// If any balance needs to be paid, navigate to the payment page. Else, go to the endpage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnPayButtonClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirm", "You will not be able to change this contribution after leaving this screen. Continue to payment screen? You will be able to make another contribution later", "OK", "Cancel"))
            {


                // Establish which items are paid for locally
                List<string> paidForIDs = new List<string>();

                foreach (OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.paid))
                    paidForIDs.Add(o._id);

                // Get most recent order from remote database
                //await DisplayOrder();
                // Fetch most recent order status
                await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                List<Coupon> used = RealmManager.All<Coupon>().Where((Coupon c) => c.couponType == "Customer" && c.selected).ToList();

                foreach(Coupon c in used)
                {
                    await DeactivateCouponRequest.SendDeactivateCouponRequest(c._id); // Deactivate each coupon used so it cannot be used again
                }

                RealmManager.Remove<Coupon>(used);


                // Establish which items are paid for remotely
                List<string> alreadyPaidIDs = new List<string>();
                foreach (OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.paid))
                    alreadyPaidIDs.Add(o._id);

                // Find the difference between remote and local
                foreach (string ID in alreadyPaidIDs)
                {
                    var index = paidForIDs.IndexOf(ID);
                    if (index != -1)
                    {
                        paidForIDs.RemoveAt(index);
                    }
                }

                // Mark each newly paid for item as paid and find the new sum of their prices
                double newContribution = 0;
                foreach (string ID in paidForIDs)
                {
                    newContribution += (RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == ID && !o.paid).FirstOrDefault()).price;
                    RealmManager.Write(() => RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == ID && !o.paid).FirstOrDefault().paid = true);
                }

                // Update items paid for in database
                await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());



                // Navigate to payment page if a balance is due
                if ((newContribution + tip) > 0)
                {
                    // Lock in user's payment status
                    RealmManager.Write(() =>
                    {
                        RealmManager.All<User>().FirstOrDefault().contribution = newContribution;
                        RealmManager.All<User>().FirstOrDefault().tip = tip;
                        RealmManager.All<User>().FirstOrDefault().paymentInProgress = true;
                    });

                    await Navigation.PushAsync(new paymentPage());
                }
                else // No contribution
                    await Navigation.PushAsync(new endPage());
            }
        }


        // Clear the tip's entry every time it is selected
        void clearTip(object sender, EventArgs e)
        {
            ((Entry)sender).Text = "";
        }

        /// <summary>
        /// Called upon the tip entry box being deselected.
        /// Rounds the tip to two decimal places (currency) if it is a valid entry (non-negative numbers)
        /// Otherwise, sets tip to 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTipCompleted(object sender, EventArgs e)
        {
            // Sanity check inputs
            if (!double.TryParse(((Entry)sender).Text, out tip) || tip < 0)
                tip = 0;
            else
                tip = double.Parse(((Entry)sender).Text, System.Globalization.NumberStyles.Currency);

            // Round to two decimal places, then update the textboxes
            tip = Math.Round(tip, 2);

            tipEntry.Text = tip.ToString("C");

            OnContributionCompleted();
        }

        /// <summary>
        /// Updates labels according to contribution and tip sum
        /// Also uses placeholder text to suggest a tip
        /// </summary>
        void OnContributionCompleted()
        {
            // Suggest tip through placeholder text
            tipEntry.Placeholder = (contribution * 0.2).ToString("C");

            // Update buttons
            payButton.Text = "Pay " + (contribution + tip).ToString("C");
        }


        async void OnRefillButtonClicked(object sender, EventArgs e)
        {
            // Send refill request
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

        async void CouponClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CouponSelectorPage());
        }

        async void PointsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PayWithPointsPage());
        }
        


        /// <summary>
        /// Pulls the most recent order status, then assigns that to the items list's itemsSource
        /// Also resets contribution to 0
        /// </summary>
        /// <returns></returns>
        public async Task DisplayOrder()
        {
            orderRefreshView.IsEnabled = false;
            menuFoodItemsView.IsEnabled = false;

            // Fetch most recent order status
            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

            await UpdateCoupons();
            


            contribution = 0;

            OnContributionCompleted();

            if(RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid).ToList().Count.Equals(0)) // Navigate to endpage if no unpaid items remain
            {
                await Navigation.PushAsync(new endPage());
                return;
            }

            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid).ToList();

            await Task.Delay(250); // Try to fix crashes related to toggling rows before the source is finished updating

            orderRefreshView.IsEnabled = true;
            menuFoodItemsView.IsEnabled = true;
        }

        async Task UpdateCoupons()
        {
            await GetCouponsRequest.SendGetCouponsRequest();

            foreach(Coupon c in RealmManager.All<CouponsList>().FirstOrDefault().Coupons)
            {
                List<OrderItem> requiredItemList = new List<OrderItem>();
                foreach (string reqID in c.requiredItems)
                {
                    foreach(OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid && !o.couponApplied && o._id == reqID).ToList())
                    {
                        requiredItemList.Add(o);
                    }
                }

                if (requiredItemList.Count.Equals(0))
                    continue;

                List<OrderItem> appliedItemList = new List<OrderItem>();
                foreach (string reqID in c.appliedItems)
                {
                    foreach (OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid && !o.couponApplied && o._id == reqID).ToList())
                    {
                        appliedItemList.Add(o);
                    }
                }

                if (appliedItemList.Count.Equals(0))
                    continue;

                int reqIt = 0;
                int appIt = 0;
                do
                {
                    OrderItem currentReq = requiredItemList[reqIt];
                    
                    OrderItem currentApp = appliedItemList[appIt];

                    var newPrice = RealmManager.Find<OrderItem>(currentApp.newID).price * (1 - (c.discount / 100));
                    newPrice = Math.Round(newPrice, 2);

                    RealmManager.Write(() =>
                    {
                        RealmManager.Find<OrderItem>(currentApp.newID).price = newPrice;
                        RealmManager.Find<OrderItem>(currentApp.newID).couponApplied = true;
                        RealmManager.Find<OrderItem>(currentReq.newID).couponApplied = true;
                    });
                    await Task.Delay(5);

                    ++appIt;
                    ++reqIt;
                    if (reqIt >= requiredItemList.Count || appIt >= appliedItemList.Count)
                        break;
                }
                while (c.repeatable);
            }

            foreach (Coupon c in RealmManager.All<Coupon>().Where((Coupon c) => c.selected && c.couponType == "Customer"))
            {
                List<OrderItem> requiredItemList = new List<OrderItem>();
                foreach (string reqID in c.requiredItems)
                {
                    foreach (OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid && !o.couponApplied && o._id == reqID).ToList())
                    {
                        requiredItemList.Add(o);
                    }
                }

                if (requiredItemList.Count.Equals(0))
                    continue;

                List<OrderItem> appliedItemList = new List<OrderItem>();
                foreach (string reqID in c.appliedItems)
                {
                    foreach (OrderItem o in RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid && !o.couponApplied && o._id == reqID).ToList())
                    {
                        appliedItemList.Add(o);
                    }
                }

                if (appliedItemList.Count.Equals(0))
                    continue;

                int reqIt = 0;
                int appIt = 0;
                do
                {
                    OrderItem currentReq = requiredItemList[reqIt];

                    OrderItem currentApp = appliedItemList[appIt];

                    var newPrice = RealmManager.Find<OrderItem>(currentApp.newID).price * (1 - (c.discount / 100));
                    newPrice = Math.Round(newPrice, 2);

                    RealmManager.Write(() =>
                    {
                        RealmManager.Find<OrderItem>(currentApp.newID).price = newPrice;
                        RealmManager.Find<OrderItem>(currentApp.newID).couponApplied = true;
                        RealmManager.Find<OrderItem>(currentReq.newID).couponApplied = true;
                    });
                    await Task.Delay(5);

                    ++appIt;
                    ++reqIt;
                    if (reqIt >= requiredItemList.Count || appIt >= appliedItemList.Count)
                        break;
                }
                while (c.repeatable);
            }
        }

        /// <summary>
        /// Called upon toggling a switch.
        /// Note that the toggle itself is tied to an order item's paid attribute, so we do not need ot manually change it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnTogglePaid(object sender, ToggledEventArgs e)
        {
            ((Switch)sender).IsEnabled = false;

            // Don't do anything if Realm is writing
            if (RealmManager.Realm.IsInTransaction)
            {
                ((Switch)sender).IsEnabled = true;
                return;
            }

            OrderItem toggledItem = new OrderItem((OrderItem)(((ViewCell)(((Switch)sender).Parent.Parent.Parent)).BindingContext));

            // Error checking
            if (toggledItem == null)
            {
                ((Switch)sender).IsEnabled = true;
                //await DisplayOrder();
                return;
            }
                
               
            // Update contribution according to toggled/not toggled
            if (e.Value)
            {
                contribution += toggledItem.price;
                contribution = Math.Round(contribution, 2);
            }
            else
            {
                contribution -= toggledItem.price;
                contribution = Math.Round(contribution, 2);
                // Mainly need this because onAppearing seems to toggle off, even if the switch already was off
                if (contribution < 0)
                    contribution = 0;
            }

            OnContributionCompleted();
            ((Switch)sender).IsEnabled = true;
        }

        async void onRefresh()
        {
            // Pull newest order status
            await DisplayOrder();

            orderRefreshView.IsRefreshing = false;
        }
    }
}