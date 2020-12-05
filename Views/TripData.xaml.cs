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
using Structures;
using SpyglassApp.SQL;
using Windows.Storage;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpyglassApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TripData : Page
    {
        Trip trip = new Trip();
        int tripNum = 900;
        public TripData()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;            
        }

        private void setDate()
        {
            var shortDateFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shortdate");
            var dateTimeToFormat = DateTime.Now;
            var shortDate = shortDateFormatter.Format(dateTimeToFormat);
            this.DateText.Text = shortDate;
            trip.Date = shortDate;
        }

        private void getDepartureTime()
        {
            var shortTimeFormmater = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shorttime");
            var timeToFormat = DateTime.Now;
            var shortTime = shortTimeFormmater.Format(timeToFormat);
            this.DepartureTimeText.Text = shortTime;
        }

        private void StartTripButton_Click(object sender, RoutedEventArgs e)
        {
            setDate();
            getDepartureTime();
            trip.DepartureTime = this.DepartureTimeText.Text;
            this.TripNumberInput.Text = tripNum.ToString();
            trip.TripNumber = tripNum;
            tripNum += 1;
            this.Frame.Navigate(typeof(DropData));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                List<string> data = new List<string>();
                SpecialTrip specialTrip = new SpecialTrip();

                /* For navigationTo that do not need a list of species */
                if (e.Parameter.GetType() == typeof(SpecialTrip))
                {
                    specialTrip = (SpecialTrip)e.Parameter;
                    data = specialTrip.list;
                }
                
                // ending trip - save data into trip
                if(data[0].Equals("EndTrip"))
                {
                    this.ArrivalTimeText.Text = data[1];
                    trip.ArrivalTime = data[1];
                    trip.DropsList = specialTrip.drops;
                    Upload_Button.Visibility = Visibility.Visible;
                }
                else
                {
                    // do nothing
                }

                
            }
            base.OnNavigatedTo(e);
        }

        // saves all user input then upload
        private void UploadTrip_Click(object sender, RoutedEventArgs e)
        {
            trip.NumberOfObservers = this.ObserversInput.Text;
            trip.ObserverInitials = this.ObserversNamesInput.Text;
            trip.NumberOfAnglers = this.AnglersInput.Text;
            trip.NumberOfObserverAnglers = this.ObservedAnglersInput.Text;
            trip.CaptainName = this.CaptainInput.Text;
            trip.VesselName = this.VesselInput.Text;
            trip.PortName = this.PortInput.Text;
            trip.Condition_Sea = this.ConditionSeaInput.Text;
            trip.Condition_Sky = this.ConditionSkyInput.Text;
            trip.Condition_Wind = this.ConditionWindInput.Text;
            trip.Condition_Swell = this.ConditionSwellInput.Text;
            trip.Notes = this.NotesInput.Text;

            // upload data to a txt file
            //uploadToFile();

            // upload data to local database
            uploadToLocalDatabase();
            
        }
        private void uploadToLocalDatabase()
        {
            SQLHandler sQLHanlder = new SQLHandler(true);
            sQLHanlder.insertTrip(trip.ObserverInitials,trip.Date,"2020",trip.PortName,trip.VesselName,trip.NumberOfObservers,
                trip.NumberOfAnglers,trip.CaptainName,trip.Condition_Sea,trip.Condition_Wind,trip.Condition_Swell,trip.Notes,
                trip.DepartureTime,trip.ArrivalTime);

            foreach (Drop drop in trip.DropsList)
            {
                sQLHanlder.insertDrop(drop.ObserverFishers, drop.StartGPS, drop.StartGPS, drop.EndGPS, drop.EndGPS,
                    drop.Notes, drop.TimeDown, drop.TimeUp, drop.Depth);
                foreach (Species species in drop.SpeciesList)
                {
                    sQLHanlder.insertFish("0", species.FishLength, species.FishFate, "M", species.FishNotes);
                }
            }

        }

        //private async void uploadToFile()
        //{
        //    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        //    StorageFile file = await localFolder.GetFileAsync("Spyglass_Data.txt");
        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Nathan\Spyglass_Data.txt",true))
        //    {
        //        file.WriteLine("Trip Data\n");
        //        file.WriteLine("Trip Number: " + trip.TripNumber.ToString());
        //        file.WriteLine("Date: " + trip.Date);
        //        file.WriteLine("NumberOfObservers: " + trip.NumberOfObservers);
        //        file.WriteLine("ObserverInitials: " + trip.ObserverInitials);
        //        file.WriteLine("NumberOfAnglers: " + trip.NumberOfAnglers);
        //        file.WriteLine("NumberOfObserverAnglers: " + trip.NumberOfObserverAnglers);
        //        file.WriteLine("CaptainName: " + trip.CaptainName);
        //        file.WriteLine("VesselName: " + trip.VesselName);
        //        file.WriteLine("PortName: " + trip.PortName);
        //        file.WriteLine("Condition_Sea: " + trip.Condition_Sea);
        //        file.WriteLine("Condition_Sky: " + trip.Condition_Sky);
        //        file.WriteLine("Condition_Wind: " + trip.Condition_Wind);
        //        file.WriteLine("Condition_Swell: " + trip.Condition_Swell);
        //        file.WriteLine("DepartureTime: " + trip.DepartureTime);
        //        file.WriteLine("ArrivalTime: " + trip.ArrivalTime);
        //        file.WriteLine("Notes: " + trip.Notes + "\n");

        //        file.WriteLine("Drop Data\n");
        //        foreach (Drop drop in trip.DropsList)
        //        {
        //            file.WriteLine("DropNumber: " + drop.DropNumber.ToString());
        //            file.WriteLine("ObserverFishers: " + drop.ObserverFishers);
        //            file.WriteLine("EndGPS: " + drop.EndGPS);
        //            file.WriteLine("Depth: " + drop.Depth);
        //            file.WriteLine("Notes: " + drop.Notes);
        //            file.WriteLine("TimeDown: " + drop.TimeDown);
        //            file.WriteLine("TimeUp: " + drop.TimeUp);

        //            file.WriteLine("Species Data\n");
        //            foreach (Species species in drop.SpeciesList)
        //            {
        //                file.WriteLine("SpeciesName: " + species.SpeciesName);
        //                file.WriteLine("CommonName: " + species.CommonName);
        //                file.WriteLine("FishLength: " + species.FishLength);
        //                file.WriteLine("FishFate: " + species.FishFate);
        //                file.WriteLine("FishNotes: " + species.FishNotes);
        //            }
        //        }

        //        file.Write("End of Trip " + trip.TripNumber.ToString());

        //    }
        //}

        private async void uploadToFile()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
         
            StorageFile file = await localFolder.CreateFileAsync("Spyglass_Data.txt",CreationCollisionOption.OpenIfExists);
            
            await FileIO.WriteTextAsync(file,"Trip Data\n");
            await FileIO.WriteTextAsync(file, "Trip Number: " + trip.TripNumber.ToString());
            await FileIO.WriteTextAsync(file, "Date: " + trip.Date);
            await FileIO.WriteTextAsync(file, "NumberOfObservers: " + trip.NumberOfObservers);
            await FileIO.WriteTextAsync(file, "ObserverInitials: " + trip.ObserverInitials);
            await FileIO.WriteTextAsync(file, "NumberOfAnglers: " + trip.NumberOfAnglers);
            await FileIO.WriteTextAsync(file, "NumberOfObserverAnglers: " + trip.NumberOfObserverAnglers);
            await FileIO.WriteTextAsync(file, "CaptainName: " + trip.CaptainName);
            await FileIO.WriteTextAsync(file, "VesselName: " + trip.VesselName);
            await FileIO.WriteTextAsync(file, "PortName: " + trip.PortName);
            await FileIO.WriteTextAsync(file, "Condition_Sea: " + trip.Condition_Sea);
            await FileIO.WriteTextAsync(file, "Condition_Sky: " + trip.Condition_Sky);
            await FileIO.WriteTextAsync(file, "Condition_Wind: " + trip.Condition_Wind);
            await FileIO.WriteTextAsync(file, "Condition_Swell: " + trip.Condition_Swell);
            await FileIO.WriteTextAsync(file, "DepartureTime: " + trip.DepartureTime);
            await FileIO.WriteTextAsync(file, "ArrivalTime: " + trip.ArrivalTime);
            await FileIO.WriteTextAsync(file, "Notes: " + trip.Notes + "\n");

            await FileIO.WriteTextAsync(file, "Drop Data\n");
            foreach (Drop drop in trip.DropsList)
            {
                await FileIO.WriteTextAsync(file, "DropNumber: " + drop.DropNumber.ToString());
                await FileIO.WriteTextAsync(file, "ObserverFishers: " + drop.ObserverFishers);
                await FileIO.WriteTextAsync(file, "EndGPS: " + drop.EndGPS);
                await FileIO.WriteTextAsync(file, "Depth: " + drop.Depth);
                await FileIO.WriteTextAsync(file, "Notes: " + drop.Notes);
                await FileIO.WriteTextAsync(file, "TimeDown: " + drop.TimeDown);
                await FileIO.WriteTextAsync(file, "TimeUp: " + drop.TimeUp);

                await FileIO.WriteTextAsync(file, "Species Data\n");
                foreach (Species species in drop.SpeciesList)
                {
                    await FileIO.WriteTextAsync(file, "SpeciesName: " + species.SpeciesName);
                    await FileIO.WriteTextAsync(file, "CommonName: " + species.CommonName);
                    await FileIO.WriteTextAsync(file, "FishLength: " + species.FishLength);
                    await FileIO.WriteTextAsync(file, "FishFate: " + species.FishFate);
                    await FileIO.WriteTextAsync(file, "FishNotes: " + species.FishNotes);
                }
            }

            await FileIO.WriteTextAsync(file, "End of Trip " + trip.TripNumber.ToString());            
        }
    }
}
