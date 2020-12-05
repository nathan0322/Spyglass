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
using Structures;
using System.Diagnostics;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpyglassApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeciesData : Page
    {
        int rowToAdd = 1;
        List<Species> listOfSpecies = new List<Species>();
        int currentDropIndex;
        public SpeciesData()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private String getTime()
        {
            var shortTimeFormmater = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            var timeToFormat = DateTime.Now;
            var longTime = shortTimeFormmater.Format(timeToFormat);
            return longTime;
        }
        // delete fish and move all fish up one row
        private void deleteFish(int rowToDelete)
        {
            int rowNumber = rowToDelete;

            rowToAdd -= 1;

            foreach (var child in SpeciesTable.Children.ToArray())
            {
                var childRow = (int)child.GetValue(Grid.RowProperty);
                if (childRow == rowNumber)
                {
                    SpeciesTable.Children.Remove(child);
                }
                else if (childRow > rowNumber)
                {
                    child.SetValue(Grid.RowProperty, childRow - 1);
                }
            }
            SpeciesTable.RowDefinitions.RemoveAt(rowNumber);
        }

        // delete fish row
        private void deleteRow(int rowToDelete)
        {
            int rowNumber = rowToDelete;

            foreach (var child in SpeciesTable.Children.ToArray())
            {
                var childRow = (int)child.GetValue(Grid.RowProperty);
                if (childRow == rowNumber)
                {
                    SpeciesTable.Children.Remove(child);
                }
            }
            SpeciesTable.RowDefinitions.RemoveAt(rowNumber);
        }

        private void AddFishButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddFish));
        }

        private async void EndDropButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDropDialog = new ContentDialog
            {
                Title = "End current drop?",
                Content = "If you end this drop, the end time will be updated and can not be changed",
                PrimaryButtonText = "End Drop",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDropDialog.ShowAsync();

            // End the drop if the user clicked the primary button.
            // Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                SpecialDrop specialDrop = new SpecialDrop();
                List<string> data = new List<string>();
                Drop drop = new Drop();
                
                drop.SpeciesList = listOfSpecies;
                specialDrop.drop = drop;
                data.Add("FromSpecies_EndDrop");
                data.Add(getTime());
                specialDrop.list = data;
                this.Frame.Navigate(typeof(DropData), specialDrop);
            }
        }
        private void SaveSpecies_Click(object sender, RoutedEventArgs e)
        {
            SpecialDrop data = new SpecialDrop();
            Drop dropToSave = new Drop();
            List<string> list = new List<string>();
            list.Add("SaveSpecies");
            dropToSave.SpeciesList = listOfSpecies;
            data.list = list;
            data.drop = dropToSave;

            this.Frame.Navigate(typeof(DropData), data);
        }

        private void editFish_Click(object sender, RoutedEventArgs e)
        {
            Button callingEdit = sender as Button;
            int rowNumber = Grid.GetRow(callingEdit);

            List<string> data = new List<string>();
            data.Add("EditSpecies");
            data.Add(rowNumber.ToString());
            data.Add(listOfSpecies[rowNumber - 1].SpeciesName);
            data.Add(listOfSpecies[rowNumber - 1].CommonName);
            data.Add(listOfSpecies[rowNumber - 1].FishLength);
            data.Add(listOfSpecies[rowNumber - 1].FishFate);
            data.Add(listOfSpecies[rowNumber - 1].FishNotes);

            this.Frame.Navigate(typeof(AddFish),data);
        }

        private void addFishToGrid(Species fish,int row)
        {
            RowDefinition c = new RowDefinition();
            c.Height = new GridLength(90);
            this.SpeciesTable.RowDefinitions.Add(c);
            
            Border b1 = new Border();
            Border b2 = new Border();
            Border b3 = new Border();
            Border b4 = new Border();

            if (row % 2 == 1)
            {
                b1.Background = new SolidColorBrush(Colors.LightBlue);
                b2.Background = new SolidColorBrush(Colors.LightBlue);
                b3.Background = new SolidColorBrush(Colors.LightBlue);
                b4.Background = new SolidColorBrush(Colors.LightBlue);
            }
            else
            {
                b1.Background = new SolidColorBrush(Colors.LightGray);
                b2.Background = new SolidColorBrush(Colors.LightGray);
                b3.Background = new SolidColorBrush(Colors.LightGray);
                b4.Background = new SolidColorBrush(Colors.LightGray);
            }
            this.SpeciesTable.Children.Add(b1);
            Grid.SetRow(b1, row);
            this.SpeciesTable.Children.Add(b2);
            Grid.SetRow(b2, row);
            Grid.SetColumn(b2, 1);
            this.SpeciesTable.Children.Add(b3);
            Grid.SetRow(b3,row);
            Grid.SetColumn(b3, 2);
            this.SpeciesTable.Children.Add(b4);
            Grid.SetRow(b4, row);
            Grid.SetColumn(b4, 3);

            TextBlock newBlock = new TextBlock();
            newBlock.Text = fish.SpeciesName;
            newBlock.VerticalAlignment = this.SpeciesText.VerticalAlignment;
            newBlock.Width = this.SpeciesText.Width;
            newBlock.FontSize = 32;
            this.SpeciesTable.Children.Add(newBlock);
            Grid.SetRow(newBlock,row);
            Grid.SetColumn(newBlock, 0);

            TextBlock block2 = new TextBlock();
            block2.Text = fish.CommonName;
            block2.FontSize = 32;
            block2.VerticalAlignment = this.CommonNameText.VerticalAlignment;
            block2.TextWrapping = this.CommonNameText.TextWrapping;
            block2.Width = this.CommonNameText.Width;
            this.SpeciesTable.Children.Add(block2);
            Grid.SetRow(block2, row);
            Grid.SetColumn(block2, 1);

            TextBlock block3 = new TextBlock();
            block3.Text = fish.FishLength;
            block3.FontSize = 32;
            block3.VerticalAlignment = this.LengthText.VerticalAlignment;
            block3.Width = this.LengthText.Width;
            this.SpeciesTable.Children.Add(block3);
            Grid.SetRow(block3, row);
            Grid.SetColumn(block3, 2);

            TextBlock block4 = new TextBlock();
            block4.Text = fish.FishFate;
            block4.FontSize = 32;
            block4.VerticalAlignment = this.FateText.VerticalAlignment;
            block4.Width = this.FateText.Width;
            this.SpeciesTable.Children.Add(block4);
            Grid.SetRow(block4, row);
            Grid.SetColumn(block4, 3);

            Button editRow_Button = new Button();
            editRow_Button.Content = "Edit";
            editRow_Button.Width = 80;
            editRow_Button.Height = 32;
            editRow_Button.Click += editFish_Click;
            this.SpeciesTable.Children.Add(editRow_Button);
            Grid.SetRow(editRow_Button, row);
            Grid.SetColumn(editRow_Button, 4);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
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

                if (data[0].Equals("DeleteFish"))
                {
                    int rowToDelete = System.Convert.ToInt32(data[1]);
                    deleteFish(rowToDelete);
                    listOfSpecies.RemoveAt(rowToDelete - 1);
                }
                else if (data[0].Equals("SaveFish"))
                {
                    Species newFish;
                    newFish.SpeciesName = data[2];
                    newFish.CommonName = data[3];
                    newFish.FishLength = data[4];
                    newFish.FishFate = data[5];
                    newFish.FishNotes = data[6];
                    int rowToEdit = System.Convert.ToInt32(data[1]);

                    listOfSpecies.Insert(rowToEdit - 1, newFish);
                    listOfSpecies.RemoveAt(rowToEdit);
                    
                    deleteRow(rowToEdit);
                    addFishToGrid(newFish, rowToEdit);                    
                }
                else if (data[0].Equals("NewFish"))
                {
                    // create species struct to add into listOfSpecies
                    Species newFish;
                    newFish.SpeciesName = data[1];
                    newFish.CommonName = data[2];
                    newFish.FishLength = data[3];
                    newFish.FishFate = data[4];
                    newFish.FishNotes = data[5];
                    listOfSpecies.Add(newFish);

                    addFishToGrid(newFish, rowToAdd);

                    rowToAdd += 1;
                }
                else if (data[0].Equals("EditDropSpecies"))
                {
                    int newDrop; // new drop or species list is empty
                    if(specialDrop.drop.SpeciesList != null)
                    {
                       listOfSpecies = specialDrop.drop.SpeciesList;
                       newDrop = 0; // not empty
                    }
                    else
                    {
                        listOfSpecies = new List<Species>();
                        newDrop = 1;
                    }
                    
                    // delete grid
                    for(int i = rowToAdd - 1; i > 0; i--)
                    {
                        deleteRow(i);
                    }
                    rowToAdd = 1; // reset rowToAdd

                    // if it's not a new drop or empty list then add fish to grid
                    if(newDrop == 0)
                    {
                        foreach (Species species in listOfSpecies)
                        {
                            addFishToGrid(species, rowToAdd);
                            rowToAdd += 1;
                        }
                    }
                  
                }
                else
                {
                    // do nothing
                }
            }
            base.OnNavigatedTo(e);
        }

       
    }
}
