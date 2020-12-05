using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SpyglassApp.Views;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpyglassApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.MainFrame.Height = this.Height;
            this.MainFrame.Width = this.Width;
        }

        // Width with navView out = 1160 
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                // nothing for settings 
            }

            else
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();

                if (navItemTag == "TripData")
                {
                    this.MainFrame.Navigate(typeof(TripData));

                }
                else if (navItemTag == "DropData")
                {
                    this.MainFrame.Navigate(typeof(DropData));
                }
                else if (navItemTag == "SpeciesData")
                {
                    this.MainFrame.Navigate(typeof(SpeciesData));
                }
            }

        }

    }
}
