using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RefreshableListBoxSample.Resources;
using System.Collections.ObjectModel;

namespace RefreshableListBoxSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>();
        private string _URL = "http://img.evolife.cn/2012-08/00169b08e5b19ad6.jpg";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.AddItems(50);
            this.refreshableListBox.ItemsSource = this._items;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void AddItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this._items.Add(_URL);
            }
        }

        private async void refreshableListBox_RefreshTriggered(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("refreshableListBox_RefreshTriggered");

            await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(2000);
                });
            this.AddItems(100);
            this.refreshableListBox.HideRefreshPanel();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}