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
                if(userPassword.Text==userPasswordReentry.Text)
                    await Navigation.PopModalAsync();
                else
                    await DisplayAlert("Error", "Passwords must match!", "Continue");
        }
    }
}