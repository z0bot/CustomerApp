using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Models;
using CustomerApp.Models.ServiceRequests;
using CustomerApp.Pages;
using Realms;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingsPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        int rating = 0;
        public RatingsPopupPage()
        {
            InitializeComponent();
        }

        private async void uxDeclineBtn_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PopModalAsync();
            await PopupNavigation.Instance.PopAllAsync();
        }

        private async void uxSubmitBtn_Clicked(object sender, EventArgs e)
        {
            string empID = RealmManager.All<Order>().FirstOrDefault().employee_id;
            string orderID = RealmManager.All<Order>().FirstOrDefault()._id;
            await PostReviewRequest.SendPostReviewRequest(orderID, empID, rating, uxReasonEntry.Text);


            await DisplayAlert("Sucess!","Thanks for your review", "OK");
            //await Navigation.PopModalAsync();
            await PopupNavigation.Instance.PopAllAsync();
        }

        private void uxRatingButton1_Clicked(object sender, EventArgs e)
        {
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "greyStar";
            uxRatingButton3.Source = "greyStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
            rating = 1;
        }

        private void uxRatingButton2_Clicked(object sender, EventArgs e)
        {
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "greyStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
            rating = 2;
        }

        //btn3
        private void uxRatingButton3_Clicked(object sender, EventArgs e)
        {
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
            rating = 3;
        }

        //btn4
        private void uxRatingButton4_Clicked(object sender, EventArgs e)
        {
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "goldStar";
            uxRatingButton5.Source = "greyStar";
            rating = 4;
        }
        //btn5
        private void uxRatingButton5_Clicked(object sender, EventArgs e)
        {
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "goldStar";
            uxRatingButton5.Source = "goldStar";
            rating = 5;
        }
    }
}