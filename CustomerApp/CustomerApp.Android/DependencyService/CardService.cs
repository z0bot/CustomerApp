using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Card.IO;
using CustomerApp.Droid;
using Xamarin.Forms;
using Xamarin.Essentials;

[assembly: Dependency(typeof(CardService))]
namespace CustomerApp.Droid
{

    public class CardService : ICard
    {
        private Activity Act;
        Context context = Android.App.Application.Context;

        // Starts reading the credit card via Card.IO
        public void StartRead()
        {
            InitService();
            

            var intent = new Intent(Act, typeof(CardIOActivity));
            intent.PutExtra(CardIOActivity.ExtraRequireCvv, true);
            intent.PutExtra(CardIOActivity.ExtraRequireCardholderName, true);

            Act.StartActivityForResult(intent, 101);
        }

        public string GetCardNum()
        {
            if(InfoSharer.Instance.Card == null)
            {
                return null;
            }
            else
                return InfoSharer.Instance.Card.CardNumber;
        }

        public string GetCardName()
        {
            if (InfoSharer.Instance.Card == null)
            {
                return null;
            }
            else
                return InfoSharer.Instance.Card.CardholderName;
        }

        public string GetCardCvv()
        {
            if (InfoSharer.Instance.Card == null)
            {
                return null;
            }
            else
                return InfoSharer.Instance.Card.Cvv;
        }

        void InitService()
        {
            var c = Platform.CurrentActivity;
            Act = c as Activity;
        }
    }

    public class InfoSharer
    {
        private static InfoSharer instance = null;
        private static object lockObj = new object();
        public CreditCard Card { get; set; }

        public static InfoSharer Instance
        {
            get
            {
                lock (lockObj)
                {
                    if(instance == null)
                    {
                        instance = new InfoSharer();
                    }
                    return instance;
                }
            }
        }
    }
}