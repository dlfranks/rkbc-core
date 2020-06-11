using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace rkbcMobile.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            Bootstrap.Initialize();
        }
        public ItemsViewModel Items
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ItemsViewModel>();
            }
        }
        public ItemDetailViewModel ItemDetail
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ItemDetailViewModel>();
            }
        }
    }
}
