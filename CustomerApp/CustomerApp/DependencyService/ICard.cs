using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;


namespace CustomerApp
{
    public interface ICard
    {
        void StartRead();

        string GetCardNum();

        string GetCardName();

        string GetCardCvv();
    }
}
