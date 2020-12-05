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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpyglassApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class AddFish : Page
    {
        private List<string> SpeciesNames = new List<string>() { "S. auriculatus","S. carnatus","S. caurinus","S. chlorostictus", "S.chrysomelas","S. constellatus","S. dalli","S. diaconus","S. entomelas","S. flavidus","S. melanops",
            "S. miniatus","S. nebulosus","S. paucispinis","S. pinniger","S. rosaceus","S. serriceps","Citharichthys sordidus","Ophiodon elongatus","S. atrovirens","S. hopkinsi","S. levis","S. maliger","S. mystinus","S. nigrocinctus","S. rastrelliger",
            "S. ruberrimus","S. rubrivinctus","S. semicinctus","S. serranoides","Anarrhichthys ocellatus","Caulolatilus princeps","Hexagrammos decagrammus","Lepidopsetta bilineata","Oncorhynchus tshawytscha","Paralichthys californicus",
            "Squalus acanthias","Scomber japonicas","Scorpaenichthys marmoratus","Semicossyphus pulcher"};
        private List<string> CommonNames = new List<string>() { "Brown rockfish","Gopher rockfish","Copper rockfish","Greenspotted rockfish","Black-and-yellow rockfish","Starry rockfish","Calico rockfish","Deacon rockfish","Widow rockfish","Yellowtail rockfish","Black rockfish",
            "Vermillion rockfish","China rockfish","Bocaccio","Canary rockfish","Rosy rockfish","Treefish","Pacific sanddab","Lingcod","Kelp rockfish","Squarespot rockfish","Cowcod","Quillback rockfish","Blotched rockfish","Tiger rockfish","Grass rockfish",
            "Yelloweye rockfish","Flag rockfish","Halfbanded rockfish","Olive rockfish","Wolf eel","Ocean whitefish","Kelp greenling","Rock sole","King salmon","California halibut",
            "Spiny dogfish","Chub mackerel","Cabezon","California sheephead"};

        private List<string> FishPictures = new List<string>() { "brown.png", "gopher.png","copper.png","greenspotted.png","bay.png","starry.png","calico.png","deacon.png","widow.png","yellowtail.png","black.png","vermilion.png",
            "china.png","bocaccio.png","canary.png","rosy.png","treefish.png","pacsanddab.png","lingcod.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png",
            "spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png",
            "spyglass_logo.png","spyglass_logo.png","spyglass_logo.png","spyglass_logo.png"};

        int rowCalled;

        public AddFish()
        {
            this.InitializeComponent();
            List<GridTextBlockDataObject> gridList = new List<GridTextBlockDataObject>();
            for(int i = 0; i < SpeciesNames.Count; i++)
            {
                gridList.Add(new GridTextBlockDataObject(SpeciesNames[i],CommonNames[i],FishPictures[i]));
            }
            SelectionGridView.ItemsSource = gridList;
        }

        private void SelectionGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SpeciesNameTextBox.Text = ((GridTextBlockDataObject)e.ClickedItem).SpeciesNameText;
            CommonNameTextBox.Text = ((GridTextBlockDataObject)e.ClickedItem).CommonNameText;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                List<string> data = (List<string>)e.Parameter;

                if (data[0].Equals("EditSpecies") == true)
                {
                    SaveButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                    rowCalled = System.Convert.ToInt32(data[1]);
                    SpeciesNameTextBox.Text = data[2];
                    CommonNameTextBox.Text = data[3];
                    FishLengthTextBox.Text = data[4];
                    FishFateTextBox.Text = data[5];
                    NotesInput.Text = data[6];
                }
            }
            base.OnNavigatedTo(e);
        }
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("NewFish");
            data.Add(SpeciesNameTextBox.Text);
            data.Add(CommonNameTextBox.Text);
            data.Add(FishLengthTextBox.Text);
            data.Add(FishFateTextBox.Text);
            data.Add(NotesInput.Text);
            this.Frame.Navigate(typeof(SpeciesData), data);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SpeciesData));
        }

        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteFishDialog = new ContentDialog
            {
                Title = "Delete fish permanently?",
                Content = "If you delete this fish, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteFishDialog.ShowAsync();

            // Delete the drop if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                List<string> data = new List<string>();
                data.Add("DeleteFish");
                data.Add(rowCalled.ToString());
                this.Frame.Navigate(typeof(SpeciesData), data);
                DeleteButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("SaveFish");
            data.Add(rowCalled.ToString());
            data.Add(SpeciesNameTextBox.Text);
            data.Add(CommonNameTextBox.Text);
            data.Add(FishLengthTextBox.Text);
            data.Add(FishFateTextBox.Text);
            data.Add(NotesInput.Text);
            this.Frame.Navigate(typeof(SpeciesData), data);
        }
    }

    public class GridTextBlockDataObject
    {
        public string SpeciesNameText { get; set; }
        public string CommonNameText { get; set; }

        public string FishPictures { get; set; }
        public GridTextBlockDataObject(string speciesnametext, string commonnametext, string fishpicture)
        {
            SpeciesNameText = speciesnametext;
            CommonNameText = commonnametext;
            FishPictures = "/Assets/Fish/" + fishpicture;
        }
    }
}
