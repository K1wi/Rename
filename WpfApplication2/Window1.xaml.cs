using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
    public partial class Window1 : Window
    {
        public string sDir_Settings = System.IO.Directory.GetCurrentDirectory() + "\\Settings";
        public string sFile_Settings = System.IO.Directory.GetCurrentDirectory() + "\\Settings\\set.ddl";
        public string[] arr_Settings = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public Window1()
        {
            InitializeComponent();            
        }

        private void LoadSettings()
        {
            if (System.IO.Directory.Exists(sDir_Settings) == false)
                System.IO.Directory.CreateDirectory(sDir_Settings);
            if (System.IO.File.Exists(sFile_Settings) == false)
            {
             //   System.IO.File.Create(sFile_Settings);
            }
            else
            {
                string[] lines = System.IO.File.ReadAllLines(sFile_Settings);
                LoadChecbox(chkbox_SerieID, lines[0]);
                txtbox_SeasonName.Text = lines[1];
                LoadChecbox(chkbox_Season1, lines[2]);
                LoadChecbox(chkbox_Episode1, lines[3]);
                txtbox_EpisodeS.Text = lines[4];
                LoadChecbox(chkbox_Episode2, lines[5]);
                txtbox_EpisodeE.Text = lines[6];
                lbl_SerieName.Content = lines[7];
                lbl_SeasonName.Content = lines[8];
                lbl_EpisodeName.Content = lines[9];
            }
        }

        private void LoadChecbox(CheckBox chkbox, string p)
        {
            if (p == "False")
                chkbox.IsChecked = false;
            else
                chkbox.IsChecked = true;
        }

        private void SaveSettingsToFile()
        {            
            arr_Settings[0] = chkbox_SerieID.IsChecked.ToString();
            arr_Settings[1] = txtbox_SeasonName.Text;
            arr_Settings[2] = chkbox_Season1.IsChecked.ToString();
            arr_Settings[3] = chkbox_Episode1.IsChecked.ToString();
            arr_Settings[4] = txtbox_EpisodeS.Text;
            arr_Settings[5] = chkbox_Episode2.IsChecked.ToString();
            arr_Settings[6] = txtbox_EpisodeE.Text;
            arr_Settings[7] = lbl_SerieName.Content.ToString();
            arr_Settings[8] = lbl_SeasonName.Content.ToString();
            arr_Settings[9] = lbl_EpisodeName.Content.ToString();
            System.IO.File.WriteAllLines(sFile_Settings, arr_Settings);
                       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtbox_SeasonName_TextChanged(object sender, TextChangedEventArgs e)
        {
              
        
            //if (chkbox_Season1.IsChecked == false)
                //lbl_SeasonName.Content = txtbox_SeasonName.Text + txtbox_SeasonS.Text;
            
           // else
            //    lbl_SeasonName.Content = txtbox_SeasonName.Text + " " + txtbox_SeasonS.Text;

        
        }

        private void txtbox_SeasonName_KeyUp(object sender, KeyEventArgs e)
        {
            if (chkbox_Season1.IsChecked == true)
                lbl_SeasonName.Content = txtbox_SeasonName.Text + txtbox_SeasonS.Text;
            else
               lbl_SeasonName.Content = txtbox_SeasonName.Text + " " + txtbox_SeasonS.Text;

        }

        private void chkbox_Season1_Click(object sender, RoutedEventArgs e)
        {
            if (chkbox_Season1.IsChecked == true)
                lbl_SeasonName.Content = txtbox_SeasonName.Text + txtbox_SeasonS.Text;
            else
                lbl_SeasonName.Content = txtbox_SeasonName.Text + " " + txtbox_SeasonS.Text;

        }
        private void ChangelblEpisode()
        {
            if (chkbox_Episode1.IsChecked == false && chkbox_Episode2.IsChecked == false)
                lbl_EpisodeName.Content = txtbox_EpisodeSerie.Text + " " + txtbox_EpisodeS.Text + txtbox_EpisodeSN.Text + " " + txtbox_EpisodeE.Text + txtbox_EpisodeEN.Text;
            else if (chkbox_Episode1.IsChecked == false && chkbox_Episode2.IsChecked == true)
                lbl_EpisodeName.Content = txtbox_EpisodeSerie.Text + " " + txtbox_EpisodeS.Text + txtbox_EpisodeSN.Text + txtbox_EpisodeE.Text + txtbox_EpisodeEN.Text;
            else if (chkbox_Episode1.IsChecked == true && chkbox_Episode2.IsChecked == false)
                lbl_EpisodeName.Content = txtbox_EpisodeSerie.Text + txtbox_EpisodeS.Text + txtbox_EpisodeSN.Text + " " + txtbox_EpisodeE.Text + txtbox_EpisodeEN.Text;
            else
                lbl_EpisodeName.Content = txtbox_EpisodeSerie.Text + txtbox_EpisodeS.Text + txtbox_EpisodeSN.Text + txtbox_EpisodeE.Text + txtbox_EpisodeEN.Text;
        }
        private void txtbox_EpisodeS_KeyUp(object sender, KeyEventArgs e)
        {
            ChangelblEpisode();

        }

        private void txtbox_EpisodeE_KeyUp(object sender, KeyEventArgs e)
        {
            ChangelblEpisode();
        }

        private void chkbox_Episode1_Click(object sender, RoutedEventArgs e)
        {
            ChangelblEpisode();
        }

        private void chkbox_Episode2_Click(object sender, RoutedEventArgs e)
        {
            ChangelblEpisode();
        }

        private void window_Settings_Activated(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void window_Settings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettingsToFile();
        }

      

    }
}
