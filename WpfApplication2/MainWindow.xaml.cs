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
using System.Security.AccessControl;


namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        const string S_DEFAULT = "?default";

        public string sFile_Settings = System.IO.Directory.GetCurrentDirectory() + "\\Settings\\set.ddl";
        private string s_Source_Directory;
        private string set_SerieID, set_SeasonName, set_Season1, set_Episode1, set_Episode2, set_EpisodeS, set_EpisodeE;

        private List<SERIE> L_SERIES = new List<SERIE>();
        private List<SEASON> L_SEASONS = new List<SEASON>();
        private List<EPISODE> L_EPISODES = new List<EPISODE>();

        private SERIE currSerie = new SERIE();
        private SEASON currSeason = new SEASON();
        private EPISODE currEpisode = new EPISODE();

        //  private List<VIDEO_FILE> L_VIDEO_FILE = new List<VIDEO_FILE>();
        private List<string> L_OLD_FILE_PATH = new List<string>();


        private struct SERIE
        {
            public int ID;
            public string Name;
            public string Path;
            public SERIE(int id, string name, string path)
            {
                ID = id;
                Name = name;
                Path = path;
            }
        }
        private struct SEASON
        {
            public int ID;
            public int FK;
            public string Name;
            public string Path;
            public int Number;

            public SEASON(int id, int fk, string name, string path, int number)
            {
                ID = id;
                Name = name;
                FK = fk;
                Path = path;
                Number = number;
            }
        }
        private struct EPISODE
        {
            public int ID;
            public int FK;
            public string Name;
            public int Number;
            public string Path;
            public EPISODE(int id, int fk, string name, string path, int number)
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
            //source_dlg.
            source_dlg.RootFolder = Environment.SpecialFolder.Desktop;
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
            btn_AutoRename.IsEnabled = false;
            btn_AutoRename2.IsEnabled = false;
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
            int SN_Number = -1;
            int E_Number = -1;

            string[] lst_Series = Directory.GetDirectories(source);   /* Get list of all directories(series) in source directory */

            /* Every serie in the source directory */
            foreach (string serie in lst_Series)
            {
                SS_Index++;
                L_SERIES.Add(new SERIE(SS_Index, System.IO.Path.GetFileName(serie), serie));  /* Add the current serie to the list */

                string[] lst_Season_Files = Directory.GetFiles(serie);  /* Get list of all files(episodes) in current serie directory */
                if (lst_Season_Files.Length > 0)    /* If files exist move them to season 0 directory */
                    ProcessWrongFiles(lst_Season_Files);

                string[] lst_Seasons = Directory.GetDirectories(serie); /* Get list of all directories(seasons) in current serie directory */
                SN_Number = 0;
                /* Every season in the serie directory */
                foreach (string season in lst_Seasons)
                {
                    SN_Index++;
                    SN_Number++;
                    L_SEASONS.Add(new SEASON(SN_Index, SS_Index, System.IO.Path.GetFileName(season), season, getSeasonNumFromName(season)));  /* Add the current season to the list */

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
                        if (rdbtn_Default.IsChecked == true)
                            L_EPISODES.Add(new EPISODE(E_Index, SN_Index, System.IO.Path.GetFileName(episode), episode, getEpisodeNumFromName(episode))); /* Add the current episode to the list */
                        else
                            L_EPISODES.Add(new EPISODE(E_Index, SN_Index, System.IO.Path.GetFileName(episode), episode, E_Number)); /* Add the current episode to the list */


                        // L_VIDEO_FILE.Add(new VIDEO_FILE(F_Index, episode, fk, SN_Number, E_Number)); /* Add the current episode path to the list */
                    }

                }
            }
            //  DisplaySeries();
            UpdateListBoxes();

        }

        private void ProcessWrongDirectories(string[] sDIrs)
        {
            // lstbox_Episodes.Items.Add("WRONG DIR ERROR");
        }

        private void ProcessWrongFiles(string[] path)
        {
            string sName, sNewPath;
            foreach (string s in path)
            {
                sName = System.IO.Path.GetFileName(s);
                sNewPath = basePath(s) + "\\" + set_SeasonName + CheckSpace(set_Season1) + "0\\" + sName;
                //  // Console.WriteLine("NAME :\t" + sName);
                //  // Console.WriteLine("PATH :\t" + s);
                //  // Console.WriteLine("NEW PATH :\t" + sNewPath);
                if (!System.IO.Directory.Exists(basePath(sNewPath)))
                {
                    LOG("Create :\t" + basePath(sNewPath));
                    System.IO.Directory.CreateDirectory(basePath(sNewPath));
                }
                if (!System.IO.File.Exists(sNewPath))
                {
                    System.IO.File.Move(s, sNewPath);
                    LOG("Moved :\t" + s + "\nto :\t" + sNewPath);
                    //// Console.WriteLine("Moved :\t" + s + "\nto :\t" + sNewPath);

                }

            }
            // Console.WriteLine("====================================================");
        }

        private void DisplaySeries()
        {
            foreach (SERIE serie in L_SERIES)
            {
                lstbox_Series.Items.Add(serie.Name);
            }
        }
        /*
         * Populate the Seasons listbox with all seasons in L_Seasons list with matching serie ID
         * */
        private void DisplaySeasons(string serie)
        {
            /*int serieID = getSerieID(serie);
            foreach (SEASONS season in L_SEASONS)
            {
                if (season.FK == serieID)
                    lstbox_Seasons.Items.Add(season.Name);

            }*/
        }
        private void UpdateListBoxes(string serie = S_DEFAULT, string season = S_DEFAULT, string episode = S_DEFAULT)
        {
            List<SERIE> selected_series = new List<SERIE>();
            List<SEASON> selected_seasons = new List<SEASON>();
            List<EPISODE> selected_episodes = new List<EPISODE>();
            if (serie == S_DEFAULT)
                lstbox_Series.Items.Clear();
            if (season == S_DEFAULT)
                lstbox_Seasons.Items.Clear();
            if (episode == S_DEFAULT)
                lstbox_Episodes.Items.Clear();
            lstbox_NewName.Items.Clear();
            int counter = 0;

            selected_series = getSelectedSerie(serie);

            foreach (SERIE s in selected_series)
            {
                counter++;
                if (serie == S_DEFAULT)
                {
                    lstbox_Series.Items.Add(s.Name);
                    //// Console.WriteLine("SERIE\tName :\t" + s.Name);
                }
                selected_seasons.Clear();
                selected_seasons.AddRange(getSelectedSeason(s.ID, season));
                selected_seasons.Sort(delegate(SEASON a, SEASON b)
                {
                    if (a.Name == null && b.Name == null) return 0;
                    else if (a.Name == null) return -1;
                    else if (b.Name == null) return 1;
                    else return a.Name.CompareTo(b.Name);
                });
                foreach (SEASON sn in selected_seasons)
                {
                    if (season == S_DEFAULT)
                    {
                        lstbox_Seasons.Items.Add(sn.Name);
                        // Console.WriteLine("  SEASON\tName :\t" + sn.Name + "\tS_Number :\t" + sn.Number.ToString());
                    }
                    selected_episodes.Clear();
                    selected_episodes.AddRange(getSelectedEpisode(sn.ID, episode));
                    foreach (EPISODE ep in selected_episodes)
                    {
                        if (episode == S_DEFAULT)
                        {
                            lstbox_Episodes.Items.Add(ep.Name);
                            //// Console.WriteLine("    EPISODE\tName :\t" + ep.Name + "\tE_Number :\t" + ep.Number.ToString() + "\tS_Number :\t" + sn.Number.ToString());
                        }
                        lstbox_NewName.Items.Add(GenerateNewName(ep.Path, s.Name, sn.Number, ep.Number));

                    }
                }
                //  prgrssbar_MainProgress.Visibility = System.Windows.Visibility.Hidden;
            }


        }

        private List<int> GetList(int[] ID, string type = "serie")
        {
            List<int> result = new List<int>();
            for (int k = 0; k < ID.Length; k++)
            {
                if (type == "serie")
                {
                    foreach (SEASON sn in L_SEASONS)
                    {
                        if (sn.FK == ID[k])
                            result.Add(sn.ID);
                    }
                }
                else if (type == "season")
                {
                    foreach (EPISODE ep in L_EPISODES)
                    {
                        if (ep.FK == ID[k])
                            result.Add(ep.ID);
                    }

                }
            }


            return result;
        }

        private void DisplayNewNames(string serie, string season = S_DEFAULT, string episode = S_DEFAULT)
        {
            /*     L_OLD_FILE_PATH.Clear();
                 int[] fk;
                 int serie_ID = getSerieID(serie);
                 int season_ID = getSeasonID(season, serie_ID);
                 int episod_ID = getEpisodeID(episode, season_ID);

                 if (season == S_DEFAULT)
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

                 else if (episode == S_DEFAULT)
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
     */
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
        private string GenerateNewName(string e_Path, string serie, int sNum, int eNum)
        {
            string result;// = e_Path;
            //string season;

            // result = result.Remove(0, result.IndexOf(serie) + serie.Length + 1);
            //season = result.Remove(result.IndexOf("\\"));

            // result = v_Path;
            // result = result.Remove(result.LastIndexOf("\\") + 1);
            result = serie + CheckSpace(set_Episode1) + set_EpisodeS + ConvertNumber(sNum) + CheckSpace(set_Episode2) + set_EpisodeE + ConvertNumber(eNum) + System.IO.Path.GetExtension(e_Path);


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


        private List<SERIE> getSelectedSerie(string serie_name)
        {
            if (serie_name == S_DEFAULT)
                return L_SERIES;
            else
                return L_SERIES.FindAll(x => x.Name == serie_name);
        }

        private List<SEASON> getSelectedSeason(int serie_ID, string season_name = S_DEFAULT)
        {
            if (season_name == S_DEFAULT)
                return L_SEASONS.FindAll(x => x.FK == serie_ID);
            else
                return L_SEASONS.FindAll(x => x.Name == season_name && x.FK == serie_ID);
        }

        private List<EPISODE> getSelectedEpisode(int season_ID, string episode_name = S_DEFAULT)
        {
            if (episode_name == S_DEFAULT)
                return L_EPISODES.FindAll(x => x.FK == season_ID);
            else
                return L_EPISODES.FindAll(x => x.Name == episode_name && x.FK == season_ID);
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
        private int[] findFK(string serie, string season = S_DEFAULT, string episode = S_DEFAULT)
        {
            int[] result = { -1, -2, -3 };
            /*
                        if (season == S_DEFAULT)
                        {
                            foreach (SERIES s in L_SERIES)
                                if (s.Name == serie)
                                {
                                    result[0] = s.ID;
                                    break;
                                }
                        }
                        else if (episode == S_DEFAULT)
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
                     */
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
                UpdateListBoxes(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
                // DisplayNewNames(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
            }

            CheckIfRenameEnable();

        }

        private void UpdateSelectedView(string type, string serie, string season = S_DEFAULT, string episode = S_DEFAULT)
        {
            lstbox_Selected.Items.Clear();
            currSerie = L_SERIES.Find(x => x.Name == serie);
            currSeason = L_SEASONS.Find(x => x.Name == season && x.FK == currSerie.ID);
            currEpisode = L_EPISODES.Find(x => x.Name == episode && x.FK == currSeason.ID);
            LOG("Serie data: \n" + "Name:\t\t" + currSerie.Name + "\t\tID:\t" + currSerie.ID + "\tPath:\t" + currSerie.Path +
                 "\nSeason data: \n" + "Name:\t\t" + currSeason.Name + "\t\tID:\t" + currSeason.ID + "\tFK:\t" + currSeason.FK + "\tNumber: \t" + currSeason.Number +
                 "\nEpisode data: \n" + "Name:\t\t" + currEpisode.Name + "\t\tID:\t" + currEpisode.ID + "\tFK:\t" + currEpisode.FK + "\tNumber: \t" + currEpisode.Number);

            btnDeleteSelected.IsEnabled = true;
            if (type == "serie")
            {

                lbl_SeasonNumberSelected.IsEnabled = false;
                txtbox_NumberSelected.IsEnabled = false;
                btnNumberSelected.IsEnabled = false;
                btnMoveSelected.IsEnabled = false;
                cmbobox_MoveTo.IsEnabled = false;

                lbl_NewNameSelected.IsEnabled = true;
                txtbox_NewSelected.IsEnabled = true;
                btnNewSelected.IsEnabled = true;
                btnRenameSelected.IsEnabled = true;
                txtbox_RenameSelected.IsEnabled = true;

                lstbox_Selected.Items.Add("SERIE");
                lstbox_Selected.Items.Add("Name :\t" + serie);
                btn_AutoRename.IsEnabled = true;
                btn_AutoRename2.IsEnabled = false;
                txtbox_RenameSelected.Text = serie;
                txtbox_NewSelected.Text = "Enter the new Serie name";

            }
            if (type == "season")
            {
                SEASON tmp_SEASONS = findSeason(serie, season);

                btnRenameSelected.IsEnabled = false;
                txtbox_RenameSelected.IsEnabled = false;

                btnMoveSelected.IsEnabled = true;
                cmbobox_MoveTo.IsEnabled = true;

                lbl_NewNameSelected.IsEnabled = true;
                txtbox_NewSelected.IsEnabled = true;
                btnNewSelected.IsEnabled = true;
                txtbox_NewSelected.Text = tmp_SEASONS.Name;

                lbl_SeasonNumberSelected.IsEnabled = true;
                txtbox_NumberSelected.IsEnabled = true;
                btnNumberSelected.IsEnabled = true;

                lstbox_Selected.Items.Add("SEASON");
                lstbox_Selected.Items.Add("Name :\t" + tmp_SEASONS.Name);
                lstbox_Selected.Items.Add("Num :\t" + tmp_SEASONS.Number.ToString());
                txtbox_NumberSelected.Text = tmp_SEASONS.Number.ToString();

                loadComboBox(tmp_SEASONS);
                btn_AutoRename2.IsEnabled = true;

            }
            if (type == "episode")
            {
                SEASON tmp_SEASONS = findSeason(serie, season);
                EPISODE tmp_EPISODE = findEpisode(serie, season, episode);

                btnRenameSelected.IsEnabled = false;
                txtbox_RenameSelected.IsEnabled = false;

                btnMoveSelected.IsEnabled = true;
                cmbobox_MoveTo.IsEnabled = true;

                lbl_NewNameSelected.IsEnabled = false;
                txtbox_NewSelected.IsEnabled = false;
                btnNewSelected.IsEnabled = false;

                lbl_SeasonNumberSelected.IsEnabled = true;
                txtbox_NumberSelected.IsEnabled = true;
                btnNumberSelected.IsEnabled = true;




                //    string stmp_NewName = GenerateNewName(tmp_VIDEO.Path, serie, tmp_SEASONS.Number, tmp_EPISODE.Number);

                lstbox_Selected.Items.Add("EPISODE");
                lstbox_Selected.Items.Add("Old Name :\t" + episode);
                //   lstbox_Selected.Items.Add("New Name :\t" + stmp_NewName);
                lstbox_Selected.Items.Add("Num :\t" + tmp_EPISODE.Number.ToString());
                loadComboBox(tmp_EPISODE);

            }
        }

        private void loadComboBox(SEASON tmp_SEASONS)
        {
            cmbobox_MoveTo.Items.Clear();
            foreach (SERIE s in L_SERIES)
            {
                cmbobox_MoveTo.Items.Add(s.Name);
            }
        }

        private void loadComboBox(EPISODE e)
        {
            cmbobox_MoveTo.Items.Clear();
            foreach (SEASON sn in getSelectedSeason(currSerie.ID))
            {
                cmbobox_MoveTo.Items.Add(sn.Name);
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
        private EPISODE findEpisode(string serie, string season, string episode)
        {
            SERIE tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            SEASON tmp_SEASON = L_SEASONS.Find(x => x.Name == season && x.FK == tmp_SERIE.ID);
            EPISODE tmp_EPISODE = L_EPISODES.Find(x => x.Name == episode && x.FK == tmp_SEASON.ID);// && x.FK[0] == tmp_SERIE.ID && x.FK[1] == tmp_SEASON.ID);
            return tmp_EPISODE;
        }

        private SEASON findSeason(string serie, string season)
        {
            SERIE tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
            SEASON tmp_SEASON = L_SEASONS.Find(x => x.Name == season && x.FK == tmp_SERIE.ID);
            return tmp_SEASON;
        }

        private SERIE findSerie(string serie)
        {
            SERIE tmp_SERIE = L_SERIES.Find(x => x.Name == serie);
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
            //string sPath;
            string serie = getSelecteFromListbox(lstbox_Series);
            string season = getSelecteFromListbox(lstbox_Seasons);
            string episode = getSelecteFromListbox(lstbox_Episodes);

            lst_items = getNewNamesFromListbox();

            List<SERIE> selected_series = new List<SERIE>();
            List<SEASON> selected_seasons = new List<SEASON>();
            List<EPISODE> selected_episodes = new List<EPISODE>();

            selected_series = getSelectedSerie(serie);
            foreach (SERIE s in selected_series)
            {
                selected_seasons.AddRange(getSelectedSeason(s.ID, season));
                foreach (SEASON sn in selected_seasons)
                {
                    selected_episodes.AddRange(getSelectedEpisode(sn.ID, episode));
                }
            }
            renameEpisodes(selected_episodes, lst_items);
            Initiate_Program(s_Source_Directory);
            /*   for (int k = 0; k < L_OLD_FILE_PATH.Count; k++)
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
               */

        }

        private string getSelecteFromListbox(System.Windows.Controls.ListBox lstbox)
        {

            if (lstbox.SelectedIndex > -1)
                return (lstbox.SelectedItem.ToString());
            else
                return (S_DEFAULT);
        }

        private void renameEpisodes(List<EPISODE> eList, string[] nmeList)
        {
            int k = 0;
            string sPath;
            LOG("nmeList : " + nmeList.Length.ToString());
            LOG("eList : " + eList.Count.ToString());
            if (nmeList.Length == eList.Count)
            {
                foreach (EPISODE e in eList)
                {
                    if (System.IO.File.Exists(e.Path))
                    {
                        sPath = e.Path.Remove(e.Path.LastIndexOf("\\") + 1) + nmeList[k];
                        System.IO.File.Move(e.Path, sPath);
                        LOG("Moved \t" + e.Path + "\n to \t" + sPath);
                    }
                    else if (e.Path != nmeList[k])
                    {
                        ERROR(nmeList[k] + " already exists file not renamed");
                    }
                    k++;
                }

            }
            else
            {
                ERROR("The new name list size does not correspond with selected episode list size");
            }
        }

        private void LOG(string p)
        {
            lstbox_Log.Items.Add(p);
            lstbox_Log.Items.Add("=========================================================================");
        }

        private void ERROR(string p)
        {
            System.Windows.MessageBox.Show("ERROR \n" + p);
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
            if (lstbox_Series.SelectedIndex > -1)
            {
                UpdateListBoxes(lstbox_Series.SelectedItem.ToString());
                UpdateSelectedView("serie", lstbox_Series.SelectedItem.ToString());
            }

            CheckIfRenameEnable();

        }

        private void lstbox_Seasons_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (lstbox_Series.SelectedIndex > -1 && lstbox_Seasons.SelectedIndex > -1)
            {
                UpdateListBoxes(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
                UpdateSelectedView("season", lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString());
            }
            CheckIfRenameEnable();
        }

        private void lstbox_Episodes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstbox_Series.SelectedIndex > -1 && lstbox_Seasons.SelectedIndex > -1 && lstbox_Episodes.SelectedIndex > -1)
            {
                UpdateListBoxes(lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString(), lstbox_Episodes.SelectedItem.ToString());
                UpdateSelectedView("episode", lstbox_Series.SelectedItem.ToString(), lstbox_Seasons.SelectedItem.ToString(), lstbox_Episodes.SelectedItem.ToString());
            }
            CheckIfRenameEnable();
        }

        private void btnRenameSelected_Click(object sender, RoutedEventArgs e)
        {
            SERIE tmp_SERIE = new SERIE();
            string sRName = txtbox_RenameSelected.Text.ToString();

            tmp_SERIE = getSelectedSerie(lstbox_Series.SelectedItem.ToString())[0];
            string sPath = tmp_SERIE.Path.Remove(tmp_SERIE.Path.LastIndexOf("\\") + 1) + sRName;

            try
            {
                System.IO.Directory.Move(tmp_SERIE.Path, sPath);
                LOG("Moved \t" + tmp_SERIE.Path + "\nto \t" + sPath);

            }
            catch (Exception ex)
            {
                LOG("Could not move directory : " + ex.ToString());
                ERROR("Could not move directory : " + ex.ToString());
            }

            Initiate_Program(s_Source_Directory);
        }

        private void ChangeSeasonName(string serie, string season)
        {

        }

        private void btn_Log_Click(object sender, RoutedEventArgs e)
        {
            if (lstbox_Log.Visibility == System.Windows.Visibility.Hidden)
            {
                lstbox_Log.Visibility = System.Windows.Visibility.Visible;
                btn_Log.Content = "Hide Log";
                window_Main.Height = 910;
            }
            else
            {
                lstbox_Log.Visibility = System.Windows.Visibility.Hidden;
                btn_Log.Content = "Show Log";
                window_Main.Height = 785;
            }
        }

        private void btnNumberSelected_Click(object sender, RoutedEventArgs e)
        {
            int newNum;
            if (!int.TryParse(txtbox_NumberSelected.Text.ToString(), out newNum))
            {
                ERROR(txtbox_NumberSelected.Text.ToString() + " is not a valid number");
                return;
            }
            else
            {
                if (lstbox_Selected.Items[0].ToString() == "SEASON")
                {
                    if (lstbox_Seasons.SelectedIndex > -1)
                    {
                        changeNumber(currSeason, newNum, lstbox_Seasons.SelectedIndex);
                        //LOG("I know its a SEASON");
                    }
                }
                else if (lstbox_Selected.Items[0].ToString() == "EPISODE")
                {
                    if (lstbox_Episodes.SelectedIndex > -1)
                    {
                        changeNumber(currEpisode, newNum);
                        //LOG("I know its a EPISODE");
                    }
                }
            }
            //Initiate_Program(s_Source_Directory);
        }

        private void changeNumber(EPISODE currEpisode, int newNum)
        {
            if (lstbox_Episodes.Items.Count > 0)
            {
                string sBasePath = currSeason.Path + "\\";
                int corrSNum = currSeason.Number;
                string sNewPath;
                string sName;
                EPISODE newEpisode;

                newEpisode = currEpisode;
                sName = GenerateNewName(currEpisode.Path, currSerie.Name, corrSNum, newNum);
                sNewPath = sBasePath + sName;
                if (!System.IO.File.Exists(sNewPath))
                {
                    System.IO.File.Move(currEpisode.Path, sNewPath);
                    LOG("Moved file :\t" + currEpisode.Path + "\nto :\t\t" + sNewPath);
                    newEpisode.Path = sNewPath;
                    newEpisode.Name = sName;
                    newEpisode.Number = newNum;
                    replaceListItem(currEpisode, newEpisode);
                    currEpisode = newEpisode;

                    UpdateListBoxes(currSerie.Name, currSeason.Name);

                }
                else
                {
                    LOG("ERORR\nThe directory '" + sNewPath + "' already exists!");
                }

            }
        }

        private void changeNumber(SEASON currSeason, int newNum, int index)
        {
            string sBasePath = currSeason.Path.Remove(currSeason.Path.LastIndexOf("\\") + 1);
            string sNewPath = sBasePath + set_SeasonName + CheckSpace(set_Season1) + newNum.ToString();
            SEASON newSeason = currSeason;
            if (!System.IO.Directory.Exists(sNewPath))
            {
                System.IO.Directory.Move(currSeason.Path, sNewPath);
                LOG("Moved directory :\t" + currSeason.Path + "\nto :\t" + sNewPath);
                newSeason.Path = sNewPath;
                newSeason.Name = set_SeasonName + CheckSpace(set_Season1) + newNum.ToString();
                newSeason.Number = newNum;
                replaceListItem(currSeason, newSeason);
                /*   foreach (SEASON sn in getSelectedSeason(currSerie.ID))
                   {
                       // Console.WriteLine("newNAME :\t" + sn.Name + "\nnewPath :\t" + sn.Path);
                   }*/
                currSeason = newSeason;

                UpdateListBoxes(currSerie.Name);

            }
            else
            {
                ERROR("The directory '" + sNewPath + "' already exists!");
            }

        }

        private void replaceListItem(SEASON oldSeason, SEASON newSeason)
        {
            int pos = L_SEASONS.IndexOf(oldSeason);
            L_SEASONS.RemoveAt(pos);
            L_SEASONS.Insert(pos, newSeason);
        }
        private void replaceListItem(SERIE oldSerie, SERIE newSerie)
        {
            int pos = L_SERIES.IndexOf(oldSerie);
            L_SERIES.RemoveAt(pos);
            L_SERIES.Insert(pos, newSerie);
        }
        private void replaceListItem(EPISODE oldEpisode, EPISODE newEpisode)
        {
            int pos = L_EPISODES.IndexOf(oldEpisode);
            L_EPISODES.RemoveAt(pos);
            L_EPISODES.Insert(pos, newEpisode);
        }

        private void btn_ClearLog_Click(object sender, RoutedEventArgs e)
        {
            lstbox_Log.Items.Clear();
        }
        private string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        private int getEpisodeNumFromName(string sName)
        {
            /*  int res = numFromReverseMethod(sName);
              if (res > -1)
                  return res;
              else*/
            return numFromLookingForE(sName);





        }

        private int numFromLookingForE(string sName)
        {
            string sNum = "";
            string stmp = System.IO.Path.GetFileName(sName);
            int iNum = -1;
            int result = -1;
            int counter = -1;
            //  bool foundE = false;
            //bool foundEndOfNumbers = false;
            int pos = -1;
            // Console.WriteLine("Processing :\t" + stmp);
            foreach (char c in stmp)
            {
                counter++;
                if (c.ToString() == "E" || c.ToString() == "e" || c.ToString() == "ep")
                {
                    // Console.WriteLine("Found E/e @ " + counter.ToString());
                    pos = counter;
                    sNum = stmp.Substring(pos + 1, 2);
                    // Console.WriteLine("sNum :\t" + sNum);
                    if (int.TryParse(sNum, out iNum))
                    {
                        result = iNum;
                        break;
                    }
                }
            }

            return result;
        }

        private int numFromReverseMethod(string sName)
        {
            string sNum = "";
            int iNum = -1;
            int result = -1;
            bool foundStartOfNumbers = false;
            bool foundEndOfNumbers = false;
            sName = ReverseString(sName);
            foreach (char c in sName)
            {
                bool res = int.TryParse(c.ToString(), out iNum);
                if (res && !foundEndOfNumbers)
                {
                    foundStartOfNumbers = true;
                    sNum += iNum.ToString();
                }
                else
                {
                    if (foundStartOfNumbers)
                        foundEndOfNumbers = true;
                }


                //LOG("sNum :\t" + sNum);
            }
            if (sNum.Length > 1)
            {
                // // Console.WriteLine("sNum :\t" + sNum);
                if (sNum.Length > 2)
                    sNum = sNum.Remove(2);
                result = Convert.ToInt32(ReverseString(sNum));
            }
            else
                result = -1;
            return result;
        }
        private int getSeasonNumFromName(string sName)
        {
            string sNum = "";
            int iNum = -1;
            int result = -1;
            bool foundStartOfNumbers = false;
            bool foundEndOfNumbers = false;
            sName = ReverseString(sName);
            foreach (char c in sName)
            {
                bool res = int.TryParse(c.ToString(), out iNum);
                if (res && !foundEndOfNumbers)
                {
                    foundStartOfNumbers = true;
                    sNum += iNum.ToString();
                }
                else
                {
                    if (foundStartOfNumbers)
                        foundEndOfNumbers = true;
                }


            }
            result = Convert.ToInt32(ReverseString(sNum));

            return result;
        }

        private void btn_AutoRename_Click(object sender, RoutedEventArgs e)
        {
            if (lstbox_Seasons.Items.Count > 0)
            {
                string sBasePath = currSerie.Path + "\\";
                int CorrNum;
                string sOldPath;
                string sNewPath;

                for (int k = 0; k < lstbox_Seasons.Items.Count; k++)
                {
                    CorrNum = getSeasonNumFromName(lstbox_Seasons.Items[k].ToString());
                    if (CorrNum > 0)
                    {
                        sNewPath = sBasePath + set_SeasonName + CheckSpace(set_Season1) + CorrNum.ToString();
                        sOldPath = sBasePath + lstbox_Seasons.Items[k].ToString();
                        if (sOldPath != sNewPath)
                        {
                            System.IO.Directory.Move(sOldPath, sBasePath + set_SeasonName + CheckSpace(set_Season1) + CorrNum.ToString());
                            LOG("Moved directory :\t" + sOldPath + "\nto :\t" + sBasePath + set_SeasonName + CheckSpace(set_Season1) + CorrNum.ToString());
                        }

                    }
                }
            }
            Initiate_Program(s_Source_Directory);
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lstbox_Selected.Items[0].ToString() == "SERIE")
            {
                DialogResult dialogRes = System.Windows.Forms.MessageBox.Show("Are you sure you wish to delete the directory " + currSerie.Path +
                    " and all of its contents?", "Delete directory", MessageBoxButtons.YesNo);
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    System.IO.Directory.Delete(currSerie.Path, true);
                    LOG("Deleted :\t" + currSerie.Path);
                    Initiate_Program(s_Source_Directory);
                }
            }
            else if (lstbox_Selected.Items[0].ToString() == "SEASON")
            {
                DialogResult dialogRes = System.Windows.Forms.MessageBox.Show("Are you sure you wish to delete the directory " + currSeason.Path +
                    " and all of its contents?", "Delete directory", MessageBoxButtons.YesNo);
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    System.IO.Directory.Delete(currSeason.Path, true);
                    LOG("Deleted :\t" + currSeason.Path);
                    Initiate_Program(s_Source_Directory);
                }
            }
            else if (lstbox_Selected.Items[0].ToString() == "EPISODE")
            {
                DialogResult dialogRes = System.Windows.Forms.MessageBox.Show("Are you sure you wish to delete the file " + currEpisode.Path, "Delete file", MessageBoxButtons.YesNo);
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    System.IO.File.Delete(currEpisode.Path);
                    LOG("Deleted :\t" + currEpisode.Path);
                    Initiate_Program(s_Source_Directory);
                }
            }

        }

        private void btnMoveSelected_Click(object sender, RoutedEventArgs e)
        {
            string sNewPath;
            if (lstbox_Selected.Items[0].ToString() == "SEASON")
            {
                sNewPath = basePath(currSeason.Path, 2) + "\\" + cmbobox_MoveTo.SelectedItem.ToString() + "\\" + currSeason.Name;
                DialogResult dialogRes = System.Windows.Forms.MessageBox.Show("Are you sure you wish to move " + currSeason.Name + " of " + currSerie.Name + " to\n" + cmbobox_MoveTo.SelectedItem.ToString(), "Move Season", MessageBoxButtons.YesNo);
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    if (System.IO.Directory.Exists(sNewPath))
                    {
                        ERROR("Directory already exists!");
                    }
                    else
                    {
                        System.IO.Directory.Move(currSeason.Path, sNewPath);
                        LOG("Moved :\t" + currSeason.Name + " to " + cmbobox_MoveTo.SelectedItem.ToString() + "\n" +
                            "FROM :\t" + currSeason.Path + " to :\t" + sNewPath);
                        Initiate_Program(sNewPath);
                    }
                }

            }

        }

        private string basePath(string p, int up = 1)
        {
            string result = p;
            for (int k = 1; k <= up; k++)
            {
                result = result.Remove(result.LastIndexOf("\\"));
            }
            return result;
        }



        private void btn_AutoRename2_Click(object sender, RoutedEventArgs e)
        {
            //List<SEASON> tmpLstSeason = getSelectedSeason(currSerie.ID);
            if (lstbox_Episodes.Items.Count > 0)
            {
                string sBasePath = currSeason.Path + "\\";
                int corrSNum = currSeason.Number;
                int corrENum;
                string sNewPath;
                string sName;
                EPISODE newEpisode;
                for (int k = 0; k < lstbox_Episodes.Items.Count; k++)
                {
                    currEpisode = findEpisode(currSerie.Name, currSeason.Name, lstbox_Episodes.Items[k].ToString());
                    newEpisode = currEpisode;
                    corrENum = getEpisodeNumFromName(lstbox_Episodes.Items[k].ToString());
                    sName = GenerateNewName(currEpisode.Path, currSerie.Name, corrSNum, corrENum);
                    sNewPath = sBasePath + sName;
                    if (!System.IO.File.Exists(sNewPath))
                    {
                        System.IO.File.Move(currEpisode.Path, sNewPath);
                        LOG("Moved file :\t" + currEpisode.Path + "\nto :\t\t" + sNewPath);
                        newEpisode.Path = sNewPath;
                        newEpisode.Name = sName;
                        newEpisode.Number = corrENum;
                        replaceListItem(currEpisode, newEpisode);


                        currEpisode = newEpisode;

                        UpdateListBoxes(currSerie.Name, currSeason.Name);

                    }
                    else
                    {
                        LOG("ERORR\nThe directory '" + sNewPath + "' already exists!");
                    }
                }
            }

        }

        private void rdbtn_Default_Checked(object sender, RoutedEventArgs e)
        {
            /*  if (rdbtn_Default.IsChecked == true)
                  // Console.WriteLine("Default");
              else
                  // Console.WriteLine("Chrono");
              */
        }

        private void txtbox_RenameSelected_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnNewSelected_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogRes = System.Windows.Forms.MessageBox.Show("Do you wish to create the directory '" + txtbox_NewSelected.Text.ToString() + "'", "Create directory", MessageBoxButtons.YesNo);
            if (dialogRes == System.Windows.Forms.DialogResult.Yes)
            {

                string sBasePath;
                string snewPath;
                if (lstbox_Selected.Items[0].ToString() == "SERIE")
                {
                    if (lstbox_Series.Items.Count > 0)
                    {
                        sBasePath = findSerie(lstbox_Series.Items[0].ToString()).Path;
                        snewPath = basePath(sBasePath) + "\\" + txtbox_NewSelected.Text.ToString();
                        if (!System.IO.Directory.Exists(snewPath))
                        {
                            System.IO.Directory.CreateDirectory(snewPath);
                            //Console.WriteLine("Created :\t" + snewPath);
                        }
                    }
                }
                if (lstbox_Selected.Items[0].ToString() == "SEASON")
                {
                    if (lstbox_Series.Items.Count > 0 && lstbox_Seasons.Items.Count > 0)
                    {
                        sBasePath = findSeason(lstbox_Series.Items[0].ToString(), lstbox_Seasons.Items[0].ToString()).Path;
                        snewPath = basePath(sBasePath) + "\\" + txtbox_NewSelected.Text.ToString();
                        if (!System.IO.Directory.Exists(snewPath))
                        {
                            System.IO.Directory.CreateDirectory(snewPath);
                            //Console.WriteLine("Created :\t" + snewPath);
                        }
                    }
                }
            }
            Initiate_Program(s_Source_Directory);
        }
    }
}
