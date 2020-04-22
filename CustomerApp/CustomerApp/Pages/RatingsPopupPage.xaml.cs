using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerApp.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingsPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        //class variable to represnet given rating
        public int _rating;
        public RatingsPopupPage()
        {
            InitializeComponent();
        }

        //inidicates no review was submitted so just pop back 
        private async void uxDeclineBtn_Clicked(object sender, EventArgs e)
        {
            //special type of pop out of page due to plugin
            await PopupNavigation.Instance.PopAllAsync();
        }

        //review was written. send review service request here
        private async void uxSubmitBtn_Clicked(object sender, EventArgs e)
        {
            string userReview = uxUserReview.Text;

            //service request HERE send with  _rating and userReview variables

            await DisplayAlert("Sucess!","Thanks for your review", "OK");
            //special type of pop out of page due to plugin
            await PopupNavigation.Instance.PopAllAsync();
        }

        //btn1 star1=gold rest are grey
        private void uxRatingButton1_Clicked(object sender, EventArgs e)
        {
            _rating = 1;

            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "greyStar";
            uxRatingButton3.Source = "greyStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
        }
        //btn2 star 1&2 gold rest are grey
        private void uxRatingButton2_Clicked(object sender, EventArgs e)
        {
            _rating = 2;

            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "greyStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
        }

        //btn3
        private void uxRatingButton3_Clicked(object sender, EventArgs e)
        {
            _rating = 3;
            
            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "greyStar";
            uxRatingButton5.Source = "greyStar";
        }

        //btn4
        private void uxRatingButton4_Clicked(object sender, EventArgs e)
        {
            _rating = 4;

            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "goldStar";
            uxRatingButton5.Source = "greyStar";
        }
        //btn5
        private void uxRatingButton5_Clicked(object sender, EventArgs e)
        {
            _rating = 5;

            uxRatingButton1.Source = "goldStar";
            uxRatingButton2.Source = "goldStar";
            uxRatingButton3.Source = "goldStar";
            uxRatingButton4.Source = "goldStar";
            uxRatingButton5.Source = "goldStar";
        }
    }
}