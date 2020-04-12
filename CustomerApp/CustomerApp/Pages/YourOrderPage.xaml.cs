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
    public partial class YourOrderPage : ContentPage
    {
        public YourOrderPage()
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

            if (DateTime.Now.DayOfYear == DateTime.Parse(RealmManager.All<User>().FirstOrDefault().birthday).DayOfYear)
            {
                if (!RealmManager.Realm.All<Order>().FirstOrDefault().send_to_kitchen && !RealmManager.All<BirthdayBool>().FirstOrDefault().birthdayGiftClaimed)
                {
                    if (await DisplayAlert("Happy Birthday!", "We're so glad you decided to join us on your birthday!\n\n"
                        + "To celebrate, we would like to offer you a complimentary dessert!\n\n"
                        + "Would you like to claim your dessert?\n\n"
                        + "You will not be asked again", "Yes", "No"))
                    {
                        // Get most recent order status
                        await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                        // Create complimentary dessert item
                        OrderItem item = new OrderItem();

                        //Assign item new ID
                        var rand = new Random();
                        item.newID = (rand.Next(0, 1000000000)).ToString();
                        while (RealmManager.Find<OrderItem>(item.newID) != null) // If the ID already exists, try again until you get a unique ID.
                        {
                            item.newID = (rand.Next(0, 1000000000)).ToString();
                        }

                        item.name = "Complimentary Dessert";
                        item.price = 0;
                        item.paid = true;

                        //Store item into local database
                        RealmManager.Write(() =>
                        {
                            RealmManager.Realm.All<Order>().FirstOrDefault().menuItems.Add(item);
                        });

                        // Send updated order
                        await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
                    }
                    RealmManager.Write(() =>
                    {
                        RealmManager.All<BirthdayBool>().FirstOrDefault().birthdayGiftClaimed = true;
                    });
                }
            }
            else // Set birthdayClaimed to false every day of the year but the user's birthday
                RealmManager.Write(() =>
                {
                    RealmManager.All<BirthdayBool>().FirstOrDefault().birthdayGiftClaimed = false;
                });

            await DisplayOrder();
        }

        /// <summary>
        /// Marks the order as 'sent' in the remote database after a confirmation message
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnSendOrderClicked(object sender, EventArgs e)
        {
            // Prompt if the user wants to send the order
            if (await DisplayAlert("WARNING: Sending Order", "Are you sure you want to send the order? Current items cannot be changed by anyone at the table.", "Yes", "No"))
            {
                // Fetch most recent version of the order
                await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                // Update IngredientTotals sum
                await RealmManager.All<Order>().FirstOrDefault().UpdateIngredientTotal();

                // Fetch most recent ingredients list
                await GetIngredientsRequest.SendGetIngredientsRequest();

                // Remove any items we can't make due to lack of ingredients
                // NOTE: Not extensively tested at the moment. Could break
                bool itemsRemoved = false;
                int iterator;
                for(iterator = 0; iterator < RealmManager.All<Order>().FirstOrDefault().IngredientTotals.Count; ++iterator)
                {
                    IngredientCount i = RealmManager.All<Order>().FirstOrDefault().IngredientTotals[iterator];

                    Ingredient DBIngredient = RealmManager.All<IngredientList>().FirstOrDefault().doc.Where((Ingredient ing) => ing._id == i._id).FirstOrDefault();
                    if (DBIngredient == null) // Ingredient not present in database
                    {
                        List<OrderItem> allItems = RealmManager.All<Order>().FirstOrDefault().menuItems.ToList();

                        List<OrderItem> containingItems = new List<OrderItem>();
                        foreach (OrderItem o in allItems)
                            if (o.ingredients.Contains(i._id))
                                containingItems.Add(o);

                        RealmManager.Remove<OrderItem>(containingItems);
                        itemsRemoved = true;

                        await RealmManager.All<Order>().FirstOrDefault().UpdateIngredientTotal();
                        --iterator; // Go back one iteration, since we may have entirely eliminated an ingredient from the count
                    }
                    else
                    {
                        if (DBIngredient.quantity < i.quantity)
                        {
                            List<OrderItem> allItems = RealmManager.All<Order>().FirstOrDefault().menuItems.ToList();

                            List<OrderItem> containingItems = new List<OrderItem>();
                            foreach (OrderItem o in allItems)
                                if (o.ingredients.Contains(i._id))
                                    containingItems.Add(o);

                            int difference = i.quantity - DBIngredient.quantity; // Number of items containing these ingredients we should remove
                            
                            List<OrderItem> toRemove = containingItems.GetRange(0, difference); // Remove the items from the beginning of the list

                            RealmManager.Remove<OrderItem>(toRemove);
                            itemsRemoved = true;

                            await RealmManager.All<Order>().FirstOrDefault().UpdateIngredientTotal();
                            --iterator; // Go back one iteration, since we may have entirely eliminated an ingredient from the count
                        }
                    }
                }


                // Inform user about any removed items
                if (itemsRemoved)
                {
                    await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());

                    // Option to cancel send order if items had to be removed
                    if (!await DisplayAlert("WARNING: Items Removed", "Some of the items you have selected could not be ordered due to a lack of ingredients\n"
                        + "The items have been automatically removed from your order\n"
                        + "Would you like to send the order now, or go back and review your order?", "Send now", "Review Order"))
                    {
                        await DisplayOrder();
                        return;
                    }
                    // Else we move on
                }

                // Set order status to 'sent'
                await SendOrderRequest.SendSendOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

                await Navigation.PushAsync(new checkoutPage());
            }
        }

        // Replaces send order button after order is sent
        async void OnPayClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new checkoutPage());
        }

        async void OnAddItemClicked(object sender, EventArgs e)
        {
            //Navigate to menu

            await Navigation.PushAsync(new menuPage());
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

        /// <summary>
        /// Only called by the swipeView Edit button. Allows user to change the special instructions for the selected item if the order is not yet sent
        /// This function will first pull the most recent order status, then, if the instructions have been changed, update the remote database with the new instructions
        /// If the item has been removed from the order before the instructions were input, an error message will be displayed and the remote order will not be updated
        /// Lastly, pulls the most recent order from the server again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnEditItemInvoked(object sender, EventArgs e)
        {
            OrderItem item = new OrderItem((OrderItem)(((SwipeItem)sender).BindingContext));

            await DisplayOrder();

            // Don't allow edits for sent items. Might change this to check if the item is prepared later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");
                return;
            }

            string instructions = await DisplayPromptAsync("Special Instructions", "Enter special instructions, such as allergen information", "OK", "Cancel", null, -1, keyboard: Keyboard.Plain, item.special_instruct);
            
            // Don't send request if nothing changed
            if (instructions == item.special_instruct)
                return;

            OrderItem preUpdateItem = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.special_instruct == item.special_instruct && o._id == item._id).FirstOrDefault();

            if (preUpdateItem != null)
            {
                RealmManager.Write(() => preUpdateItem.special_instruct = instructions);

                await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
            }
            else
            {
                await DisplayAlert("Something went wrong", "Sorry, but we were unable to edit that item's description. Most likely, it was removed by another user at your table. Please verify if the item is gone, then re-add it to your order", "OK");
            }
            await DisplayOrder();
        }

        /// <summary>
        /// Only called by the Delete swipeView item. Removes the selected item from the order if the order is not yet sent
        /// Pulls the most recent update for the order, removes the item if it still exists, then updates the remote order.
        /// Lastly, pulls the most recent order from the server again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnRemoveItemInvoked(object sender, EventArgs e)
        {
            OrderItem item = new OrderItem((OrderItem)(((SwipeItem)sender).BindingContext));

            await DisplayOrder();

            // Don't allow removal of sent items. Might change this to check if the item is prepared later
            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since the order has already been sent", "OK");
                return;
            }

            // Don't allow removing paid items. Might become more relevant later, but for now prevents removing items paid for with points
            if (RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.special_instruct == item.special_instruct && o._id == item._id).FirstOrDefault().paid)
            {
                await DisplayAlert("Option Unavailable", "Sorry, but this option is not available since this item has already been paid for", "OK");
                return;
            }

            OrderItem preUpdateItem = RealmManager.All<Order>().FirstOrDefault().menuItems.Where((OrderItem o) => o.special_instruct == item.special_instruct && o._id == item._id).FirstOrDefault();
            
            // Remove item if it was not already removed
            if(preUpdateItem != null)
            {
                RealmManager.Write(() => RealmManager.All<Order>().FirstOrDefault().menuItems.Remove(preUpdateItem));

                await UpdateOrderMenuItemsRequest.SendUpdateOrderMenuItemsRequest(RealmManager.All<Order>().FirstOrDefault()._id, RealmManager.All<Order>().FirstOrDefault().menuItems.ToList());
            }


            await DisplayOrder();
        }


        /// <summary>
        /// Called every time the list of order items (orderRefreshView) is refreshed by pulling down
        /// </summary>
        async void onRefresh()
        {
            // Pull newest order status
            await DisplayOrder();

            orderRefreshView.IsRefreshing = false;
        }

        /// <summary>
        /// Gets the most recent version of the order, sets the menuFoodItemsView's ItemsSource property to that order, and turns the Send Order button into a Payment button if the order has been sent
        /// </summary>
        /// <returns></returns>
        public async Task DisplayOrder()
        {
            // Fetch most recent version of the order
            await GetOrderRequest.SendGetOrderRequest(RealmManager.All<Order>().FirstOrDefault()._id);

            menuFoodItemsView.ItemsSource = RealmManager.All<Order>().FirstOrDefault().menuItems.ToList();

            if (RealmManager.All<Order>().FirstOrDefault().send_to_kitchen)
            {
                // Reset button
                sendOrderButton.Clicked -= OnSendOrderClicked;
                sendOrderButton.Clicked -= OnPayClicked;
                // Add new functionality
                sendOrderButton.Clicked += OnPayClicked;
                sendOrderButton.Text = "Payment";
            }
        }
    }

}