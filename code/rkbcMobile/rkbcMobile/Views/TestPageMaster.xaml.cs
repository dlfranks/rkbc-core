using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace rkbcMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPageMaster : ContentPage
    {
        public ListView ListView;

        public TestPageMaster()
        {
            InitializeComponent();

            BindingContext = new TestPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class TestPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<TestPageMasterMenuItem> MenuItems { get; set; }

            public TestPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<TestPageMasterMenuItem>(new[]
                {
                    new TestPageMasterMenuItem { Id = 0, Title = "Page 1" },
                    new TestPageMasterMenuItem { Id = 1, Title = "Page 2" },
                    new TestPageMasterMenuItem { Id = 2, Title = "Page 3" },
                    new TestPageMasterMenuItem { Id = 3, Title = "Page 4" },
                    new TestPageMasterMenuItem { Id = 4, Title = "Page 5" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}