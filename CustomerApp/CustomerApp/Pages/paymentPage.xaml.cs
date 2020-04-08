﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CustomerApp.Models;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class paymentPage : ContentPage
    {
        static double total;

        public paymentPage(double contribution, double tip)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            confirmPayButton.IsVisible = false;

            total = contribution + tip;
        }

        async void scanCard(object sender, EventArgs e)
        {
            DependencyService.Get<ICard>().StartRead();

            // Give the card reader time to come up
            await System.Threading.Tasks.Task.Delay(1000);

            scanCardButton.IsVisible = false;

            // Add button to confirm order if it was not already added
            confirmPayButton.Text = "Confirm Payment of " + total.ToString("C");
            confirmPayButton.IsVisible = true;

        }


        async void confirmButton(object sender, EventArgs e)
        {
            if (DependencyService.Get<ICard>().ReadSuccesful())
            {
                if (await DisplayAlert("Card Details", DependencyService.Get<ICard>().GetCardName() + "\n"
                    + DependencyService.Get<ICard>().GetCardNum() + "\n"
                    + DependencyService.Get<ICard>().GetCardCvv() + "\n"
                    + "Valid expiry? " + (DependencyService.Get<ICard>().IsExpiryValid() ? "Yes" : "No"), "Confirm", "Cancel"))
                {
                    await DisplayAlert("Confimed", "Payment confirmed", "OK");

                    await LeavePage();
                    return;
                }
                else
                {

                }
            }
            else
            {
                await DisplayAlert("Error", "Couldn't read card data, please try to scan your card again", "OK");
                scanCardButton.IsVisible = true;
                confirmPayButton.IsVisible = false;
            }
        }

        async void payWithCashClicked(object sender, EventArgs e)
        {
            // Send notification to waitstaff

            await DisplayAlert("Cash Payment", "Your server is on their way to collect your cash payment", "OK");

            await LeavePage();
        }

        async Task LeavePage()
        {
            // Remove previous page to prevent double payment
            if(Navigation.NavigationStack.Count() > 1)
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count() - 2]);


            // Add points


            // Unlock user from payment page
            RealmManager.Write(() =>
            {
                RealmManager.All<User>().FirstOrDefault().paymentInProgress = false;
                RealmManager.All<User>().FirstOrDefault().contribution = 0;
                RealmManager.All<User>().FirstOrDefault().tip = 0;
            });

            // Offer game
            if (await DisplayAlert("Game Opportunity", "Would you like to play a game for a chance at a free dessert?", "Yes, please!", "No thanks"))
            {
                Navigation.InsertPageBefore(new gamePage(), this);
            }
            else
            {
                Navigation.InsertPageBefore(new endPage(), this);
            }
            await Navigation.PopAsync();
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

        // Prevent going back to previous pages, as the order has already been sent. Must continue and pay
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}