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

        private async void StartTripButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog startTripDialog = new ContentDialog
            {
                Title = "Start Trip?",
                Content = "Are you sure you want to start the trip?",
                PrimaryButtonText = "Start Trip",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await startTripDialog.ShowAsync();

            // Start the trip if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                setDate();
                getDepartureTime();
                trip.DepartureTime = this.DepartureTimeText.Text;
                this.TripNumberInput.Text = tripNum.ToString();
                trip.TripNumber = tripNum;
                tripNum += 1;
                this.Frame.Navigate(typeof(DropData));
            }
            
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
        private async void UploadTrip_Click(object sender, RoutedEventArgs e)
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

            ContentDialog uploadData = new ContentDialog
            {
                Title = "Upload data?",
                Content = "Save data to a file and upload to database.",
                PrimaryButtonText = "Upload",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await uploadData.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // upload data to a txt file
                uploadToFile();
                displayFileSaved();

                // upload data to local database
                //uploadToLocalDatabase();
            }
        }
        private async void displayFileSaved()
        {
            ContentDialog dataSaved = new ContentDialog
            {
                Title = "Data Saved",
                Content = "Data successfully saved to a file in AppData Local Packages 327c9e8c-c626-4d03-835c-e41abc648bd9_31cjdsjrwwpkj LocalState",
                PrimaryButtonText = "Close"
            };

            ContentDialogResult result = await dataSaved.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                // do nothing
            }
        }
        private void uploadToLocalDatabase()
        {
            SQLHandler sQLHanlder = new SQLHandler(false);
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
        private async void uploadToFile()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = await localFolder.CreateFileAsync("Spyglass_Data.txt", CreationCollisionOption.GenerateUniqueName);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString("Trip Data\n\n");
                    dataWriter.WriteString("Trip Number: " + trip.TripNumber.ToString());
                    dataWriter.WriteString("\nDate: " + trip.Date);
                    dataWriter.WriteString("\nNumberOfObservers: " + trip.NumberOfObservers);
                    dataWriter.WriteString("\nObserverInitials: " + trip.ObserverInitials);
                    dataWriter.WriteString("\nNumberOfAnglers: " + trip.NumberOfAnglers);
                    dataWriter.WriteString("\nNumberOfObserverAnglers: " + trip.NumberOfObserverAnglers);
                    dataWriter.WriteString("\nCaptainName: " + trip.CaptainName);
                    dataWriter.WriteString("\nVesselName: " + trip.VesselName);
                    dataWriter.WriteString("\nPortName: " + trip.PortName);
                    dataWriter.WriteString("\nCondition_Sea: " + trip.Condition_Sea);
                    dataWriter.WriteString("\nCondition_Sky: " + trip.Condition_Sky);
                    dataWriter.WriteString("\nCondition_Wind: " + trip.Condition_Wind);
                    dataWriter.WriteString("\nCondition_Swell: " + trip.Condition_Swell);
                    dataWriter.WriteString("\nDepartureTime: " + trip.DepartureTime);
                    dataWriter.WriteString("\nArrivalTime: " + trip.ArrivalTime);
                    dataWriter.WriteString("\nNotes: " + trip.Notes + "\n");

                    dataWriter.WriteString("\nDrop Data\n");
                    foreach (Drop drop in trip.DropsList)
                    {
                        dataWriter.WriteString("DropNumber: " + drop.DropNumber.ToString());
                        dataWriter.WriteString("\nObserverFishers: " + drop.ObserverFishers);
                        dataWriter.WriteString("\nStart GPS: " + drop.StartGPS);
                        dataWriter.WriteString("\nEndGPS: " + drop.EndGPS);
                        dataWriter.WriteString("\nDepth: " + drop.Depth);
                        dataWriter.WriteString("\nNotes: " + drop.Notes);
                        dataWriter.WriteString("\nTimeDown: " + drop.TimeDown);
                        dataWriter.WriteString("\nTimeUp: " + drop.TimeUp);

                        dataWriter.WriteString("\nSpecies Data\n");
                        foreach (Species species in drop.SpeciesList)
                        {
                            dataWriter.WriteString("\nSpeciesName: " + species.SpeciesName);
                            dataWriter.WriteString("\nCommonName: " + species.CommonName);
                            dataWriter.WriteString("\nFishLength: " + species.FishLength);
                            dataWriter.WriteString("\nFishFate: " + species.FishFate);
                            dataWriter.WriteString("\nFishNotes: " + species.FishNotes);
                        }
                    }
                    dataWriter.WriteString("\nEnd of Trip " + trip.TripNumber.ToString() + "\n");

                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
                stream.Dispose();
            }
        }
        
    }
}
