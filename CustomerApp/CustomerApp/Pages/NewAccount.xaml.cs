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
    public partial class NewAccount : ContentPage
    {
        public NewAccount()
        {
            InitializeComponent();
        }

        private async void RegisterAcctButton_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(uxFirstName.Text) || 
                string.IsNullOrEmpty(uxLastName.Text) ||
                string.IsNullOrEmpty(uxEmail.Text) ||
                string.IsNullOrEmpty(uxPassword.Text)||
                string.IsNullOrEmpty(uxPasswordReentry.Text)||
                uxBirthdate.Date == null)
            {
                await DisplayAlert("ERROR", "All entries must be filled!", "OK");
            }
            else if(UserPasswordCheck())
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("ERROR", "Passwords must match!", "Continue");
            }
        }
        /// <summary>
        /// A check that returns true if the user passwords match and then
        /// stores account info into realm 
        /// </summary>
        public bool UserPasswordCheck()
        {
            if (uxPassword.Text == uxPasswordReentry.Text)
            {
                StoreUserDetails();
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Method that will store the user account details into 
        /// Realm, a local data base that persist user data on device
        /// </summary>
        public void StoreUserDetails()
        {
            User user = new User();

            user.FirstName = uxFirstName.Text;
            user.LastName = uxLastName.Text;
            user.Birthday = uxBirthdate.ToString();
            user.Email = uxEmail.Text;
            user.Password = uxPassword.Text;

            RealmManager.AddOrUpdate<User>(user);
        }
    }
}