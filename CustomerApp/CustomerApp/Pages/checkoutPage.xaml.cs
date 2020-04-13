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
                await DisplayOrder();

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
                if ((contribution + tip) > 0)
                {
                    // Lock in user's payment status
                    RealmManager.Write(() =>
                    {
                        RealmManager.All<User>().FirstOrDefault().contribution = newContribution;
                        RealmManager.All<User>().FirstOrDefault().tip = tip;
                        RealmManager.All<User>().FirstOrDefault().paymentInProgress = true;
                    });

                    if (Math.Abs(newContribution - contribution) >= 0.01)
                        await DisplayAlert("Notice", "Due to other items already being paid for, your payment has been changed to " + newContribution.ToString("C") + " plus your tip of " + tip.ToString("C"), "OK");

                    await Navigation.PushAsync(new paymentPage(newContribution, tip));
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
            await Navigation.PushModalAsync(new CouponSelectorPage());
        }

        async void PointsClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PayWithPointsPage());
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

            await GetCouponsRequest.SendGetCouponsRequest();

            // Update active restaurant (sales, specials, etc.) coupons
            foreach (Coupon c in RealmManager.All<CouponsList>().FirstOrDefault().Coupons.Where((Coupon c) => c.couponType == "Restaurant" && c.active))
            {
                foreach(string reqID in c.requiredItems)
                {
                    List<OrderItem> requiredItemList = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == reqID && !o.couponApplied && !o.paid).ToList();
                    if (!requiredItemList.Count().Equals(0)) // If the order contains at least one of the required items not already used for a different coupon
                    {
                        foreach(string applID in c.appliedItems)
                        {
                            List<OrderItem> appliedItemList = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == applID && !o.couponApplied && !o.paid).ToList();
                            if (!appliedItemList.Count().Equals(0))
                            {
                                // For now just pick the first/default item from each list. Could be optimized later
                                OrderItem requiredItem = requiredItemList.FirstOrDefault();
                                OrderItem appliedItem = appliedItemList.FirstOrDefault();

                                RealmManager.Write(() =>
                                {
                                    RealmManager.Find<OrderItem>(appliedItem.newID).price *= (1 - (c.discount / 100));
                                    RealmManager.Find<OrderItem>(appliedItem.newID).couponApplied = true;
                                    RealmManager.Find<OrderItem>(requiredItem.newID).couponApplied = true;
                                });
                                await Task.Delay(5);
                            }
                            if (!c.repeatable)
                                break;
                        }
                        
                    }
                    if (!c.repeatable)
                        break;
                }
            }


            // Update Customer coupons
            foreach (Coupon c in RealmManager.All<User>().FirstOrDefault().coupons.Where((Coupon c) => c.selected))
            {
                foreach (string reqID in c.requiredItems)
                {
                    List<OrderItem> requiredItemList = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == reqID && !o.couponApplied && !o.paid).ToList();
                    if (!requiredItemList.Count().Equals(0)) // If the order contains at least one of the required items not already used for a different coupon
                    {
                        foreach (string applID in c.appliedItems)
                        {
                            List<OrderItem> appliedItemList = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o._id == applID && !o.couponApplied && !o.paid).ToList();
                            if (!appliedItemList.Count().Equals(0))
                            {
                                // For now just pick the first/default item from each list. Could be optimized later
                                OrderItem requiredItem = requiredItemList.FirstOrDefault();
                                OrderItem appliedItem = appliedItemList.FirstOrDefault();

                                RealmManager.Write(() =>
                                {
                                    RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.newID == appliedItem.newID).FirstOrDefault().price *= (1 - (c.discount / 100));
                                    RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.newID == appliedItem.newID).FirstOrDefault().couponApplied = true;
                                    RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.newID == requiredItem.newID).FirstOrDefault().couponApplied = true;
                                });
                                await Task.Delay(5);
                            }
                        }

                    }
                }
            }


            contribution = 0;

            OnContributionCompleted();

            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => !o.paid).ToList();

            await Task.Delay(500); // Try to fix crashes related to toggling rows before the source is finished updating

            orderRefreshView.IsEnabled = true;
            menuFoodItemsView.IsEnabled = true;
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
            if (toggledItem._id == null)
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