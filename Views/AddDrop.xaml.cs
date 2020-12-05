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
using Windows.Devices.Geolocation;
using Structures;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpyglassApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        int currentDropIndex;
        public BlankPage1()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DropData));
        }

        private void Start_Drop_Click(object sender, RoutedEventArgs e)
        {              
            getTimeDown();
            
            List<string> data = new List<string>();
            data.Add("NewDrop");
            data.Add(DropNumberInput.Text);
            data.Add(ObserverFishersInput.Text);
            data.Add(StartGPSInput.Text);
            data.Add(EndGPSInput.Text);
            data.Add(DepthInput.Text);
            data.Add(DropDataNotesInput.Text);
            data.Add(TimeDownText.Text);
            data.Add(TimeUpText.Text);
            //System.Threading.Thread.Sleep(600);
            this.Frame.Navigate(typeof(DropData), data);
        }

        private void Save_Drop_Click(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("SaveEditDrop");
            data.Add(currentDropIndex.ToString());
            data.Add(DropNumberInput.Text);
            data.Add(ObserverFishersInput.Text);
            data.Add(StartGPSInput.Text);
            data.Add(EndGPSInput.Text);
            data.Add(DepthInput.Text);
            data.Add(DropDataNotesInput.Text);
            data.Add(TimeDownText.Text);
            data.Add(TimeUpText.Text);
            this.Frame.Navigate(typeof(DropData), data);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                List<string> data = (List<string>)e.Parameter;

                if (data[0].Equals("AddDrop"))
                {
                    SaveButton.Visibility = Visibility.Collapsed;
                    currentDropIndex = System.Convert.ToInt32(data[1]);
                    DropNumberInput.Text = data[1];
                    StartGPSInput.Text = "";
                    EndGPSInput.Text = "";
                    DepthInput.Text = "";
                    DropDataNotesInput.Text = "";
                    TimeDownText.Text = "";
                    TimeUpText.Text = "";
                }
                else if(data[0].Equals("EditDrop"))
                {
                    SaveButton.Visibility = Visibility.Visible;
                    currentDropIndex = System.Convert.ToInt32(data[1]);
                    DropNumberInput.Text = data[2];
                    ObserverFishersInput.Text = data[3];
                    StartGPSInput.Text = data[4];
                    if (data[5] != null)
                    {
                        EndGPSInput.Text = data[5];
                    }
                    DepthInput.Text = data[6];
                    DropDataNotesInput.Text = data[7];
                    TimeDownText.Text = data[8];
                    if (data[9] != null)
                    {
                        TimeUpText.Text = data[9];
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        private void getTimeDown()
        {
            var shortTimeFormmater = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            var timeToFormat = DateTime.Now;
            var longTime = shortTimeFormmater.Format(timeToFormat);
            this.TimeDownText.Text = longTime;
        }

        private async void Get_StartGPS_Button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog getGPSDialog = new ContentDialog
            {
                Title = "Get Start GPS?",
                Content = "This will override the previous Start GPS coordinates for this drop (if any).",
                PrimaryButtonText = "Get Start GPS",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await getGPSDialog.ShowAsync();

            // Get the start GPS coords if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        Geolocator geolocator = new Geolocator();
                        geolocator.DesiredAccuracyInMeters = 50;

                        Geoposition position = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));
                        String latitude = position.Coordinate.Latitude.ToString("F4");
                        String longitude = position.Coordinate.Longitude.ToString("F4");

                        this.StartGPSInput.Text = latitude + ", " + longitude;
                        break;
                }
            }
        }
        private async void Get_EndGPS_Button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog getGPSDialog = new ContentDialog
            {
                Title = "Get End GPS?",
                Content = "This will override the previous End GPS coordinates for this drop (if any).",
                PrimaryButtonText = "Get End GPS",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await getGPSDialog.ShowAsync();

            // Get the start GPS coords if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        Geolocator geolocator = new Geolocator();
                        geolocator.DesiredAccuracyInMeters = 50;

                        Geoposition position = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));
                        String latitude = position.Coordinate.Latitude.ToString("F4");
                        String longitude = position.Coordinate.Longitude.ToString("F4");

                        this.EndGPSInput.Text = latitude + ", " + longitude;
                        break;
                }
            }
        }
    }
}
