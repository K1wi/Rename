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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;


namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string sFile_Settings = System.IO.Directory.GetCurrentDirectory() + "\\Settings\\set.ddl";
        private string s_Source_Directory;
        private string set_SerieID, set_SeasonName, set_Season1, set_Episode1, set_Episode2, set_EpisodeS, set_EpisodeE;

        private List<SERIES> L_SERIES = new List<SERIES>();
        private List<SEASONS> L_SEASONS = new List<SEASONS>();
        private List<EPISODES> L_EPISODES = new List<EPISODES>();
        //  private List<VIDEO_FILE> L_VIDEO_FILE = new List<VIDEO_FILE>();
        private List<string> L_OLD_FILE_PATH = new List<string>();


        private struct SERIES
        {
            public int ID;
            public string Name;
            public string Path;
            public SERIES(int id, string name, string path)
            {
                ID = id;
                Name = name;
                Path = path;
            }
        }
        private struct SEASONS
        {
            public int ID;
            public int FK;
            public string Name;
            public string Path;
            public int Number;

            public SEASONS(int id, int fk, string name, string path, int number)
            {
                ID = id;
                Name = name;
                FK = fk;
                Path = path;
                Number = number;
            }
        }
        private struct EPISODES
        {
            public int ID;
            public int FK;
            public string Name;
            public int Number;
            public string Path;
            public EPISODES(int id, int fk, string name, string path, int number)
            {
                ID = id;
                Name = name;
                FK = fk;
                Number = number;
                Path = path;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void btn_SelectSource_Click(object sender, RoutedEventArgs e)
        {
            Personal_Library.FUNCTIONS.tstFUNC();

            var source_dlg = new System.Windows.Forms.FolderBrowserDialog();
            source_dlg.ShowNewFolderButton = false;
            source_dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            System.Windows.Forms.DialogResult result = source_dlg.ShowDialog();
            s_Source_Directory = source_dlg.SelectedPath;
            /*    if (s_Source_Directory.Length <= 30)
                    txtbox_CurrentSourcePath.Width = 100;
                else if (s_Source_Directory.Length <= 80)
                    txtbox_CurrentSourcePath.Width = 250;
                else if (s_Source_Directory.Length <= 130)
                    txtbox_CurrentSourcePath.Width = 450;
                else if (s_Source_Directory.Length <= 150)
                    txtbox_CurrentSourcePath.Width = 800;
         */
            txtbox_CurrentSourcePath.Text = s_Source_Directory;

            Initiate_Program(s_Source_Directory);


        }

        private void Initiate_Program(string path)
        {
            lstbox_Seasons.Items.Clear();
            lstbox_Episodes.Items.Clear();
            lstbox_NewName.Items.Clear();
            lstbox_Series.Items.Clear();
            L_SERIES.Clear();
            L_SEASONS.Clear();
            L_EPISODES.Clear();
            //L_VIDEO_FILE.Clear();
            if (Directory.Exists(path))
            {
                ProcessSource(path);
            }

        }

        private void ProcessSource(string source)
        {
            int E_Index = -1;
            int SS_Index = -1;
            int SN_Index = -1;
            int F_Index = -1;
            int SN_Number = -1;
            int E_Number = -1;

            string[] lst_Series = Directory.GetDirectories(source);   /* Get list of all directories(series) in source directory */

            /* Every serie in the source directory */
            foreach (string serie in lst_Series)
            {
                SS_Index++;
                L_SERIES.Add(new SERIES(SS_Index, System.IO.Path.GetFileName(serie), serie));  /* Add the current serie to the list */

                string[] lst_Season_Files = Directory.GetFiles(serie);  /* Get list of all files(episodes) in current serie directory */
                if (lst_Season_Files.Length > 0)    /* If files exist move them to season 0 directory */
                    ProcessWrongFiles();

                string[] lst_Seasons = Directory.GetDirectories(serie); /* Get list of all directories(seasons) in current serie directory */
                SN_Number = 0;
                /* Every season in the serie directory */
                foreach (string season in lst_Seasons)
                {
                    SN_Index++;
                    SN_Number++;
                    L_SEASONS.Add(new SEASONS(SN_Index, SS_Index, System.IO.Path.GetFileName(season), season, SN_Number));  /* Add the current season to the list */

                    string[] lst_Episodes_Dirs = Directory.GetDirectories(season);  /* Get list of all directories(episodes_dirs) in current season directory */
                    if (lst_Episodes_Dirs.Length > 0)    /* If directories exist move all files to the current season directory */
                        ProcessWrongDirectories(lst_Episodes_Dirs);

                    string[] lst_Episodes = Directory.GetFiles(season); /* Get list of all files(episodes) in current season directory */
                    //ADD THIS TO A LOG LIST
                    //     if (lst_Episodes.Length < 1)    /* If no files found in current season directory display in listbox */
                    //         lstbox_Episodes.Items.Add("No episodes in " + System.IO.Path.GetFileName(season) + " of " + System.IO.Path.GetFileName(serie));
                    //     else
                    //         lstbox_Episodes.Items.Add(lst_Episodes.Length.ToString() + " episode(s) was found for " + System.IO.Path.GetFileName(season) + " of " + System.IO.Path.GetFileName(serie));

                    /* Every episode in the season directory */
                    E_Number = 0;
                    foreach (string episode in lst_Episodes)
                    {
                        E_Index++;
                        E_Number++;
                        int[] fk = { SS_Index, SN_Index, E_Index };
                        L_EPISODES.Add(new EPISODES(E_Index, SN_Index, System.IO.Path.GetFileName(episode), episode, E_Number)); /* Add the current episode to the list */
                        // L_VIDEO_FILE.Add(new VIDEO_FILE(F_Index, episode, fk, SN_Number, E_Number)); /* Add the current episode path to the list */
                    }

                }
            }
            DisplaySeries();
        }

        private void ProcessWrongDirectories(string[] sDIrs)
        {
            // lstbox_Episodes.Items.Add("WRONG DIR ERROR");
        }

        private void ProcessWrongFiles()
        {
            // lstbox_Episodes.Items.Add("WRONG FILE ERROR");
        }

        private void DisplaySeries()
        {
            foreach (SERIES serie in L_SERIES)
            {
                lstbox_Series.Items.Add(serie.Name);
            }
        }
        /*
         * Populate the Seasons listbox with all seasons in L_Seasons list with matching serie ID
         * */
        private void DisplaySeasons(string serie)
        {
            int fk = getSerieID(serie);
            foreach (SEASONS season in L_SEASONS)
            {
                if (season.FK == fk)
                    lstbox_Seasons.Items.Add(season.Name);

            }
        }
        private void DisplayEpisodes(string serie, string season = "?default")
        {
            int serie_ID = getSerieID(serie);
            int season_ID = getSeasonID(season, serie_ID);

            if (season == "?default")
            {
                foreach (SEASONS s in L_SEASONS)
                {
                    if (s.FK == serie_ID)
                    {
                        foreach (EPISODES e in L_EPISODES)
                        {
                            if (e.FK == s.ID)
                            {
                                lstbox_Episodes.Items.Add(e.Name);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (EPISODES e in L_EPISODES)
                {
                    if (e.FK == season_ID)
                    {
                        lstbox_Episodes.Items.Add(e.Name);
                    }
                }
            }


        }

        private void DisplayNewNames(string serie, string season = "?default", string episode = "?default")
        {
            L_OLD_FILE_PATH.Clear();
            int[] fk;
            int serie_ID = getSerieID(serie);
            int season_ID = getSeasonID(season, serie_ID);
            int episod_ID = getEpisodeID(episode, season_ID);

            if (season == "?default")
            {
                foreach (SEASONS sn in L_SEASONS)
                {
                    if (sn.FK == serie_ID)
                    {
                        foreach (EPISODES ep in L_EPISODES)
                        {
                            if (ep.FK == )
                            {
                                //   string sNewName = GenerateNewName(ep.Path, serie, ep., v_file.eNumber);
                                //   lstbox_NewName.Items.Add(sNewName);
                                //   L_OLD_FILE_PATH.Add(v_file.Path);
                            }
                        }
                    }
                }
            }

            else if (episode == "?default")
            {
                fk = findFK(serie, season);
                foreach (VIDEO_FILE v_file in L_VIDEO_FILE)
                {
                    if (v_file.FK[0] == fk[0] && v_file.FK[1] == fk[1])
                    {
                        string sNewName = GenerateNewName(v_file.Path, serie, v_file.sNumber, v_file.eNumber);
                        lstbox_NewName.Items.Add(sNewName);
                        L_OLD_FILE_PATH.Add(v_file.Path);
                    }
                }
            }
            else
            {
                fk = findFK(serie, season, episode);
                foreach (VIDEO_FILE v_file in L_VIDEO_FILE)
                {
                    if (v_file.FK[0] == fk[0] && v_file.FK[1] == fk[1] && v_file.FK[2] == fk[2])
                    {
                        string sNewName = GenerateNewName(v_file.Path, serie, v_file.sNumber, v_file.eNumber);
                        lstbox_NewName.Items.Add(sNewName);
                        L_OLD_FILE_PATH.Add(v_file.Path);
                    }
                }

            }

        }
        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : GenerateNewName
         * Purpose      : Create a new name based on the Episode's Serie, Season number and Episode number
         *                the path of the file is used for the location of the file (Does not change the location
         *                of the file)
         * Parameters   : v_Path - The current path of the video file
         *                serie - The name of the serie
         *                sNum - The season number of the video file
         *                eNum - The episode number of the video file
         * Returns      : string value of the new name for the video file
         * ======================================================================================================*/
        private string GenerateNewName(string v_Path, string serie, int sNum, int eNum)
        {
            string result = v_Path;
            string season;

            result = result.Remove(0, result.IndexOf(serie) + serie.Length + 1);
            season = result.Remove(result.IndexOf("\\"));

            // result = v_Path;
            // result = result.Remove(result.LastIndexOf("\\") + 1);
            result = serie + CheckSpace(set_Episode1) + set_EpisodeS + ConvertNumber(sNum) + CheckSpace(set_Episode2) + set_EpisodeE + ConvertNumber(eNum) + System.IO.Path.GetExtension(v_Path);


            return result;
        }

        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : ConvertNumber
         * Purpose      : Add a leading zero if num < 10
         * Parameters   : num - int of the number to check
         * Returns      : string value of the number to use
         * ======================================================================================================*/
        private string ConvertNumber(int num)
        {
            if (num < 10)
                return "0" + num.ToString();
            else
                return num.ToString();
        }

        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : CheckSpace
         * Purpose      : Determines if a space should be removed
         * Parameters   : s_Check - string value containing False or True 
         * Returns      : string either blank or with space
         * ======================================================================================================*/
        private string CheckSpace(string s_Check)
        {
            if (s_Check == "False")
                return " ";
            else
                return "";
        }
        private int getSerieID(string serie_name)
        {
            int result = -1;
            foreach (SERIES s in L_SERIES)
                if (s.Name == serie_name)
                {
                    result = s.ID;
                    break;
                }
            return result;
        }

        private List<int> getSeasonID(string season_name, int serie_ID)
        {
            List<int> result = new List<int>();

            foreach (SEASONS s in L_SEASONS)
                if (s.Name == season_name && s.FK == serie_ID)
                {
                    result.Add(s.ID);
                    break;
                }
            return result;
        }

        private int getEpisodeID(string episode_name, int season_ID)
        {
            int result = -1;
            foreach (EPISODES e in L_EPISODES)
                if (e.Name == episode_name && e.FK == season_ID)
                {
                    result = e.ID;
                    break;
                }
            return result;
        }

        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : findFK
         * Purpose      : This function lookup the FK(foreign key) of a specific season or episode
         * Parameters   : serie - string value of the serie name to look for
         *                season - string value of the season name to look for
         * Returns      : int array of size two. First element is serie ID, second element is season ID 
         * ======================================================================================================*/
        private int[] findFK(string serie, string season = "?default", string episode = "?default")
        {
            int[] result = { -1, -2, -3 };

            if (season == "?default")
            {
                foreach (SERIES s in L_SERIES)
                    if (s.Name == serie)
                    {
                        result[0] = s.ID;
                        break;
                    }
            }
            else if (episode == "?default")
            {
                foreach (SERIES s in L_SERIES)
                    if (s.Name == serie)
                    {
                        result[0] = s.ID;
                        break;
                    }
                foreach (SEASONS sn in L_SEASONS)
                    if (sn.Name == season && sn.FK == result[0])
                    {
                        result[1] = sn.ID;
                        break;
                    }
            }
            else
            {
                foreach (SERIES s in L_SERIES)
                    if (s.Name == serie)
                    {
                        result[0] = s.ID;
                        break;
                    }
                foreach (SEASONS sn in L_SEASONS)
                    if (sn.Name == season && sn.FK == result[0])
                    {
                        result[1] = sn.ID;
                        break;
                    }
                foreach (EPISODES ep in L_EPISODES)
                    if (ep.Name == episode && ep.FK[0] == result[0] && ep.FK[1] == result[1])
                    {
                        result[2] = ep.ID;
                        break;
                    }

            }
            return result;

        }
        /* ======================================================================================================
         * COMMENT  lstbox_Seasons_SelectionChanged                                                                                             
         * ======================================================================================================
         * Event handler for when the content of lstbox_Seasons should change
         * Displays the relating Episodes as well as the New Names
         * 
         * 
         * ======================================================================================================*/
        private void lstbox_Seasons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstbox_Episodes.Items.Clear();
            lstbox_NewName.Items.Clear();
            if (lstbox_Series.SelectedIndex >= 0 && lstbox_Seasons.SelectedIndex >= 0)
            {
                DisplayEpisodes(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
                DisplayNewNames(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
            }

            CheckIfRenameEnable();

        }

        private void UpdateSelectedView(string type, string serie, string season = "?default", string episode = "?default")
        {
            lstbox_Selected.Items.Clear();
            if (type == "serie")
            {

                lbl_SeasonNumberSelected.IsEnabled = false;
                txtbox_NumberSelected.IsEnabled = false;
                btnNumberSelected.IsEnabled = false;

                lbl_NewNameSelected.IsEnabled = true;
                txtbox_NewSelected.IsEnabled = true;
                btnNewSelected.IsEnabled = true;

                lstbox_Selected.Items.Add("SERIE");
                lstbox_Selected.Items.Add("Name :\t" + serie);

            }
            if (type == "season")
            {

                lbl_SeasonNumberSelected.IsEnabled = true;
                txtbox_NumberSelected.IsEnabled = true;
                btnNumberSelected.IsEnabled = true;

                lbl_NewNameSelected.IsEnabled = true;
                txtbox_NewSelected.IsEnabled = true;
                btnNewSelected.IsEnabled = true;
                SEASONS tmp_SEASONS = findSeason(serie, season);


                lstbox_Selected.Items.Add("SEASON");
                lstbox_Selected.Items.Add("Name :\t" + tmp_SEASONS.Name);
                lstbox_Selected.Items.Add("Num :\t" + tmp_SEASONS.Number.ToString());
                txtbox_NumberSelected.Text = tmp_SEASONS.Number.ToString();
                txtbox_RenameSelected.Text = tmp_SEASONS.Name;
                txtbox_NewSelected.Text = tmp_SEASONS.Name;

            }
            if (type == "episode")
            {

                lbl_SeasonNumberSelected.IsEnabled = true;
                txtbox_NumberSelected.IsEnabled = true;
                btnNumberSelected.IsEnabled = true;

                lbl_NewNameSelected.IsEnabled = false;
                txtbox_NewSelected.IsEnabled = false;
                btnNewSelected.IsEnabled = false;

                SEASONS tmp_SEASONS = findSeason(serie, season);
                EPISODES tmp_EPISODE = findEpisode(serie, season, episode);
                VIDEO_FILE tmp_VIDEO = findVideo(serie, season, episode);
                string stmp_NewName = GenerateNewName(tmp_VIDEO.Path, serie, tmp_SEASONS.Number, tmp_EPISODE.Number);

                lstbox_Selected.Items.Add("EPISODE");
                lstbox_Selected.Items.Add("Old Name :\t" + episode);
                lstbox_Selected.Items.Add("New Name :\t" + stmp_NewName);
                lstbox_Selected.Items.Add("Season Num :\t" + tmp_SEASONS.Number.ToString());

            }
        }
        /*
        private VIDEO_FILE findVideo(string serie, string season, string episode)
        {
            SERIES tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            SEASONS tmp_SEASON = L_SEASONS.Find(x => x.Name == season && x.FK == tmp_SERIE.ID);
            EPISODES tmp_EPISODE = L_EPISODES.Find(x => x.Name == episode && x.FK[0] == tmp_SERIE.ID && x.FK[1] == tmp_SEASON.ID);
            VIDEO_FILE tmp_VIDEO = L_VIDEO_FILE.Find(x => x.ID == tmp_EPISODE.ID);
            return tmp_VIDEO;
        }
        */
        private EPISODES findEpisode(string serie, string season, string episode)
        {
            SERIES tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            SEASONS tmp_SEASON = L_SEASONS.Find(x => x.Name == season && x.FK == tmp_SERIE.ID);
            EPISODES tmp_EPISODE = L_EPISODES.Find(x => x.Name == episode && x.FK[0] == tmp_SERIE.ID && x.FK[1] == tmp_SEASON.ID);
            return tmp_EPISODE;
        }

        private SEASONS findSeason(string serie, string season)
        {
            SERIES tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            SEASONS tmp_SEASON = L_SEASONS.Find(x => x.Name == season && x.FK == tmp_SERIE.ID);
            return tmp_SEASON;
        }

        private SERIES findSeries(string serie)
        {
            SERIES tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            return tmp_SERIE;
        }


        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingWindow();
        }

        /* ======================================================================================================
         * COMMENT  ShowSettingWindow                                                                                             
         * ======================================================================================================
         * Display the window for edditing program settings
         * 
         * 
         * ======================================================================================================*/
        private void ShowSettingWindow()
        {
            var windw_Settings = new Window1();
            windw_Settings.Show();
        }

        /* ======================================================================================================
         * COMMENT  Window_Activated                                                                                             
         * ======================================================================================================
         * Check if a valid settings file exist if not open the settings window which will generate one
         * Use the set.dll setting file to populate specific setting parameters
         * 
         * 
         * ======================================================================================================*/
        private void Window_Activated(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(sFile_Settings) == false)
            {
                ShowSettingWindow();
            }
            else
            {
                string[] lines = System.IO.File.ReadAllLines(sFile_Settings);
                set_SerieID = lines[0];
                set_SeasonName = lines[1];
                set_Season1 = lines[2];
                set_Episode1 = lines[3];
                set_EpisodeS = lines[4];
                set_Episode2 = lines[5];
                set_EpisodeE = lines[6];
            }
        }


        /* ======================================================================================================
         * COMMENT  CheckIfRenameEnable                                                                                             
         * ======================================================================================================
         * Check if rename button should be enabled (Only when there are a valid number of items in lstbox_NewNames 
         * 
         * 
         * ======================================================================================================*/
        private void CheckIfRenameEnable()
        {
            if (lstbox_NewName.Items.IsEmpty == false)
                btn_Rename.IsEnabled = true;
            else
                btn_Rename.IsEnabled = false;
        }

        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : btn_Rename_Click(??Create a function)
         * Purpose      : ??????????????Rename the items in lstbox_NewName to the new name dispayed below old name
         * Parameters   : ?
         * Returns      : ?
         * ======================================================================================================*/
        private void btn_Rename_Click(object sender, RoutedEventArgs e)
        {
            string[] lst_items;
            string sPath;
            lst_items = getNewNamesFromListbox();
            for (int k = 0; k < L_OLD_FILE_PATH.Count; k++)
            {
                sPath = L_OLD_FILE_PATH[k].Remove(L_OLD_FILE_PATH[k].LastIndexOf("\\") + 1) + lst_items[k];
                //lstbox_Selected.Items.Add(L_OLD_FILE_PATH[k]);
                //lstbox_Selected.Items.Add(sPath );
                if (System.IO.File.Exists(sPath) == false)
                    System.IO.File.Move(L_OLD_FILE_PATH[k], sPath);
                else if (sPath != L_OLD_FILE_PATH[k])
                {
                    System.Windows.MessageBox.Show(sPath + " already exists file not renamed");

                }
            }

        }
        /* ======================================================================================================
         * COMMENT                                                                                               
         * ======================================================================================================
         * Function     : getNewNamesFromListbox
         * Purpose      : The functions creates a list of all names in lstbox_NewNames and return the string list
         * Parameters   : None
         * Returns      : string array of the list of items in lstbox_NewNames
         * ======================================================================================================*/
        private string[] getNewNamesFromListbox()
        {
            string[] result = new string[lstbox_NewName.Items.Count];
            int i = 0;
            foreach (string item in lstbox_NewName.Items)
            {
                result[i] = item;
                i++;
            }
            return result;
        }

        private void lstbox_Episodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstbox_NewName.Items.Clear();
            if (lstbox_Series.SelectedIndex >= 0 && lstbox_Seasons.SelectedIndex >= 0 && lstbox_Episodes.SelectedIndex >= 0)
            {
                DisplayNewNames(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString(), lstbox_Episodes.SelectedItem.ToString());
            }

            CheckIfRenameEnable();

        }

        /* ======================================================================================================
         * COMMENT  lstbox_Series_MouseUp                                                                                             
         * ======================================================================================================
         * Click event handler for lstbox_Series 
         * Displays the relating Seasons, Episodes as well as the New Names
         * 
         * 
         * ======================================================================================================*/
        private void lstbox_Series_MouseUp(object sender, MouseButtonEventArgs e)
        {
            lstbox_Seasons.Items.Clear();
            lstbox_Episodes.Items.Clear();
            lstbox_NewName.Items.Clear();
            if (lstbox_Series.SelectedIndex >= 0)
            {
                DisplaySeasons(lstbox_Series.SelectedItem.ToString());
                DisplayNewNames(lstbox_Series.SelectedItem.ToString());
                DisplayEpisodes(lstbox_Series.SelectedItem.ToString());
            }

            CheckIfRenameEnable();
            UpdateSelectedView("serie", lstbox_Series.SelectedItem.ToString());

        }

        private void lstbox_Seasons_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (lstbox_Series.SelectedIndex > -1)
                UpdateSelectedView("season", lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
        }

        private void lstbox_Episodes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstbox_Series.SelectedIndex > -1 && lstbox_Seasons.SelectedIndex > -1)
                UpdateSelectedView("episode", lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString(), lstbox_Episodes.SelectedItem.ToString());

        }

        private void btnRenameSelected_Click(object sender, RoutedEventArgs e)
        {
            ChangeSeasonName(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
        }

        private void ChangeSeasonName(string serie, string season)
        {
            SEASONS tmp_SEASONS = findSeason(serie, season);
            int pos = L_SEASONS.IndexOf(tmp_SEASONS);
            L_SEASONS.RemoveAt(pos);
            tmp_SEASONS.Name = txtbox_RenameSelected.Text.ToString();

            L_SEASONS.Insert(pos, tmp_SEASONS);
        }






    }
}
