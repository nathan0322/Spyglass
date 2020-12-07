using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows;
using Windows.UI;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Structures;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpyglassApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DropData : Page
    {
        int DropRowIndex = 1;
        int timeUpIndex = 1;
        int rowNumberToDelete;
        int rowNumberForSpeciesEdit = 0;
        List<Drop> listOfDrops = new List<Drop>();
        List<Species> currentSpeciesToHold = new List<Species>();

        public DropData()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private String getArrivalTime()
        {
            var shortTimeFormmater = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shorttime");
            var timeToFormat = DateTime.Now;
            var shortTime = shortTimeFormmater.Format(timeToFormat);
            return shortTime;
        }
        private void Add_Drop_Click(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("AddDrop");
            data.Add(DropRowIndex.ToString());
            this.Frame.Navigate(typeof(BlankPage1), data); // blankPage1 is add-drop page
        }
        private async void End_Trip_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog endTripDialog = new ContentDialog
            {
                Title = "End Trip?",
                Content = "Are you sure you want to end the trip?",
                PrimaryButtonText = "End Trip",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await endTripDialog.ShowAsync();

            // End the trip if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                SpecialTrip specialTrip = new SpecialTrip();
                List<string> list = new List<string>();
                list.Add("EndTrip");
                list.Add(getArrivalTime());
                specialTrip.list = list;
                specialTrip.drops = listOfDrops;
                this.Frame.Navigate(typeof(TripData), specialTrip);
            }
            else
            {
                // The user clicked the CloseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }
        }

        private void AddDropToGrid(Drop dropToAdd, int row)
        {
            RowDefinition c = new RowDefinition();
            c.Height = new GridLength(90);
            this.DropTable.RowDefinitions.Add(c);

            Border b1 = new Border();
            Border b2 = new Border();
            Border b3 = new Border();
            Border b4 = new Border();
            Border b5 = new Border();

            if (DropRowIndex % 2 == 1)
            {
                b1.Background = new SolidColorBrush(Colors.LightGreen);
                b2.Background = new SolidColorBrush(Colors.LightGreen);
                b3.Background = new SolidColorBrush(Colors.LightGreen);
                b4.Background = new SolidColorBrush(Colors.LightGreen);
                b5.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                b1.Background = new SolidColorBrush(Colors.LightGray);
                b2.Background = new SolidColorBrush(Colors.LightGray);
                b3.Background = new SolidColorBrush(Colors.LightGray);
                b4.Background = new SolidColorBrush(Colors.LightGray);
                b5.Background = new SolidColorBrush(Colors.LightGray);
            }

            this.DropTable.Children.Add(b1);
            Grid.SetRow(b1, DropRowIndex);
            this.DropTable.Children.Add(b2);
            Grid.SetRow(b2, DropRowIndex);
            Grid.SetColumn(b2, 1);
            this.DropTable.Children.Add(b3);
            Grid.SetRow(b3, DropRowIndex);
            Grid.SetColumn(b3, 2);
            this.DropTable.Children.Add(b4);
            Grid.SetRow(b4, DropRowIndex);
            Grid.SetColumn(b4, 3);
            this.DropTable.Children.Add(b5);
            Grid.SetRow(b5, DropRowIndex);
            Grid.SetColumn(b5, 4);

            TextBlock block0 = new TextBlock();
            block0.Text = DropRowIndex.ToString();
            block0.VerticalAlignment = this.DropNumberText.VerticalAlignment;
            block0.HorizontalAlignment = this.DropNumberText.HorizontalAlignment;
            block0.Width = 40;
            block0.FontSize = 32;
            this.DropTable.Children.Add(block0);
            Grid.SetRow(block0, DropRowIndex);
            Grid.SetColumn(block0, 0);

            TextBlock block1 = new TextBlock();
            block1.Text = dropToAdd.TimeDown;
            block1.VerticalAlignment = this.TimeDownText.VerticalAlignment;
            block1.HorizontalAlignment = this.TimeDownText.HorizontalAlignment;
            block1.Width = 175;
            block1.FontSize = 32;
            this.DropTable.Children.Add(block1);
            Grid.SetRow(block1, DropRowIndex);
            Grid.SetColumn(block1, 1);

            Button EditDropButton = new Button();
            EditDropButton.Content = "Edit Drop";
            EditDropButton.Height = 90;
            EditDropButton.Width = 160;
            EditDropButton.FontSize = 30;
            EditDropButton.Margin = new Thickness(15);
            EditDropButton.Click += editDrop_Click;
            this.DropTable.Children.Add(EditDropButton);
            Grid.SetRow(EditDropButton, DropRowIndex);
            Grid.SetColumn(EditDropButton, 3);

            Button EditSpeciesButton = new Button();
            EditSpeciesButton.Content = "Species";
            EditSpeciesButton.Height = 90;
            EditSpeciesButton.Width = 150;
            EditSpeciesButton.FontSize = 30;
            EditSpeciesButton.Margin = new Thickness(15);
            EditSpeciesButton.Click += editSpecies_Click;
            this.DropTable.Children.Add(EditSpeciesButton);
            Grid.SetRow(EditSpeciesButton, DropRowIndex);
            Grid.SetColumn(EditSpeciesButton, 4);

            Button deleteRow_Button = new Button();
            deleteRow_Button.Content = "X";
            deleteRow_Button.Width = 30;
            deleteRow_Button.Height = 32;
            deleteRow_Button.Click += deleteDrop_Click;
            this.DropTable.Children.Add(deleteRow_Button);
            Grid.SetRow(deleteRow_Button, DropRowIndex);
            Grid.SetColumn(deleteRow_Button, 5);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                List<string> data = new List<string>();
                SpecialDrop specialDrop = new SpecialDrop();

                /* For navigationTo that do not need a list of species */
                if (e.Parameter.GetType() == typeof(List<string>))
                {
                    data = (List<string>)e.Parameter;
                }
                /* For navigationTo that need a list of species */
                else
                {
                    specialDrop = (SpecialDrop)e.Parameter;
                    data = specialDrop.list;
                }
                
                
                // coming from species - end drop
                if (data[0].Equals("FromSpecies_EndDrop") == true)
                {
                    if(timeUpIndex >= DropRowIndex)
                    {
                        // do nothing
                    }
                    else
                    {
                        TextBlock block2 = new TextBlock();
                        block2.Text = data[1];
                        block2.VerticalAlignment = this.TimeUpText.VerticalAlignment;
                        block2.HorizontalAlignment = this.TimeUpText.HorizontalAlignment;
                        block2.Width = 175;
                        block2.FontSize = 32;
                        this.DropTable.Children.Add(block2);
                        Grid.SetRow(block2, timeUpIndex);
                        Grid.SetColumn(block2, 2);

                        Drop drop = new Drop();
                        drop = listOfDrops[timeUpIndex - 1];
                        drop.TimeUp = data[1];
                        drop.SpeciesList = specialDrop.drop.SpeciesList; 

                        listOfDrops.Insert(timeUpIndex - 1,drop);
                        listOfDrops.RemoveAt(timeUpIndex);

                        timeUpIndex += 1;
                        //rowNumberForSpeciesEdit += 1;
                    }
                }

                // coming from add drop - saving edited drop
                else if (data[0].Equals("SaveEditDrop"))
                {
                    Drop drop = new Drop();
                    int rowToChange = System.Convert.ToInt32(data[1]);
                    drop.DropNumber = System.Convert.ToInt32(data[2]);
                    drop.ObserverFishers = data[3];
                    drop.StartGPS = data[4];
                    drop.EndGPS = data[5];
                    drop.Depth = data[6];
                    drop.Notes = data[7];
                    drop.TimeDown = data[8];
                    drop.TimeUp = data[9];
                    
                    drop.SpeciesList = currentSpeciesToHold;
                    
                    listOfDrops.Insert(rowToChange, drop);
                    listOfDrops.RemoveAt(rowToChange + 1);

                }
                // coming from add drop - add new drop
                else if(data[0].Equals("NewDrop"))
                {
                    // coming from add drop, but only 1 drop in progress allowed at time
                    if (DropRowIndex - 1 == timeUpIndex)
                    {
                        // do nothing
                    }
                    else
                    {
                        Drop drop = new Drop();
                        drop.DropNumber = System.Convert.ToInt32(data[1]);
                        drop.ObserverFishers = data[2];
                        drop.StartGPS = data[3];
                        drop.EndGPS = data[4];
                        drop.Depth = data[5];
                        drop.Notes = data[6];
                        drop.TimeDown = data[7];
                        drop.TimeUp = data[8];
                        listOfDrops.Add(drop);

                        AddDropToGrid(drop, DropRowIndex);

                        DropRowIndex += 1;
                    }   
                } 
                // coming from Species - saving current species list
                else if(data[0].Equals("SaveSpecies"))
                {
                    Drop drop = new Drop();
                    drop = listOfDrops[rowNumberForSpeciesEdit];
                    drop.SpeciesList = specialDrop.drop.SpeciesList;

                    listOfDrops.Insert(rowNumberForSpeciesEdit, drop);
                    listOfDrops.RemoveAt(rowNumberForSpeciesEdit + 1);

                    //foreach (Species species in drop.SpeciesList)
                    //{
                    //    Debug.WriteLine("Common: " + species.CommonName);
                    //}
                }
            }
            base.OnNavigatedTo(e);
        }
        private void editSpecies_Click(object sender, RoutedEventArgs e)
        {
            SpecialDrop data = new SpecialDrop();
            Drop dropToEdit = new Drop();
            List<string> list = new List<string>();

            Button callingDelete = sender as Button;
            int rowNumber = Grid.GetRow(callingDelete);
            rowNumberForSpeciesEdit = rowNumber - 1; // to hold current drop number for species edit on return

            dropToEdit = listOfDrops[rowNumber - 1];
            data.drop = dropToEdit;
            list.Add("EditDropSpecies");
            data.list = list;

            this.Frame.Navigate(typeof(SpeciesData),data);
        }
        private void editDrop_Click(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            Drop dropToEdit = new Drop();
            
            Button callingDelete = sender as Button;
            int rowNumber = Grid.GetRow(callingDelete);
            dropToEdit = listOfDrops[rowNumber - 1];

            data.Add("EditDrop");
            data.Add((rowNumber-1).ToString());
            data.Add((dropToEdit.DropNumber).ToString());
            data.Add(dropToEdit.ObserverFishers);
            data.Add(dropToEdit.StartGPS);
            data.Add(dropToEdit.EndGPS);
            data.Add(dropToEdit.Depth);
            data.Add(dropToEdit.Notes);
            data.Add(dropToEdit.TimeDown);
            data.Add(dropToEdit.TimeUp);

            currentSpeciesToHold = dropToEdit.SpeciesList;
            
            this.Frame.Navigate(typeof(BlankPage1),data); // BlankPage1 = AddDrop Page
        }

        private async void deleteDrop_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDropDialog = new ContentDialog
            {
                Title = "Delete drop permanently?",
                Content = "If you delete this drop, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDropDialog.ShowAsync();

            // Delete the drop if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                // Delete the drop
                Button callingDelete = sender as Button;
                rowNumberToDelete = Grid.GetRow(callingDelete);

                listOfDrops.RemoveAt(rowNumberToDelete - 1);

                if (DropRowIndex <= timeUpIndex)
                {
                    timeUpIndex -= 1;
                }

                DropRowIndex -= 1;


                int callingButtonIndex = DropTable.Children.IndexOf(callingDelete);
                foreach (var child in DropTable.Children.ToArray())
                {
                    var childRow = (int)child.GetValue(Grid.RowProperty);
                    
                    if (childRow == rowNumberToDelete)
                    {
                        DropTable.Children.Remove(child);
                    }
                    else if (childRow > rowNumberToDelete)
                    {
                        child.SetValue(Grid.RowProperty, childRow - 1);
                    }
                }
                DropTable.RowDefinitions.RemoveAt(rowNumberToDelete);
            }
            else
            {
                // The user clicked the CloseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }
        }

    }
}
