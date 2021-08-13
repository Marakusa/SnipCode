using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace SnipCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //========================================
        // Variables

        // A list of all the snippets currently loaded
        private List<Snippet> snippets = new List<Snippet>();

        // Is editing a starred snippet
        private bool editingStarred = false;
        // Is the editing snippet new
        private bool editingNew = false;
        // Currently editing snippet (-1 == currently not editing)
        private int editingSnippet = -1;

        // Currently selected tab
        private int selectedTab = 0;
        // A snippet button template
        private string snippetTemplate;

        // A snippet button function handler
        private SnippetButtonHandler buttonHandler;

        // Snippets save path
        private string savesPath = @"C:\";

        //========================================


        //========================================
        // Main window control functions

        // Initialize the window
        public MainWindow()
        {
            // Initialize GUI
            InitializeComponent();

            // Load window settings
            if (Properties.Settings.Default.width > 0)
                Width = Properties.Settings.Default.width;
            if (Properties.Settings.Default.height > 0)
                Height = Properties.Settings.Default.height;

            // Set default save path for snippets
            savesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnipCode/Saves");

            // Check if save path exists and if not create it
            if (!Directory.Exists(savesPath))
                Directory.CreateDirectory(savesPath);

            // Setup the snippet button handler
            buttonHandler = new SnippetButtonHandler();
            buttonHandler.ButtonPressed += ButtonHandler_ButtonPressed;

            // Set the snippet button template
            snippetTemplate = XamlWriter.Save(snippet);
            snippet.Visibility = Visibility.Collapsed;
            codeSnippetInput.Visibility = Visibility.Collapsed;

            // Load snippets
            LoadSnippets();

            // Go to main tab
            ChangeTab(0);
        }

        // Tab change
        public void ChangeTab(int tab)
        {
            var discard = AskDiscard();

            if ((editingSnippet > -1 && discard == 1) || editingSnippet == -1)
            {
                if (tab != selectedTab)
                {
                    scrollPanel.ScrollToTop();
                }

                selectedTab = tab;

                // Update tab button graphics
                SetHomeFill("pack://siteoforigin:,,,/Interface/home.png");
                SetAddFill("pack://siteoforigin:,,,/Interface/add.png");
                SetStarFill("pack://siteoforigin:,,,/Interface/star.png");
                SetSettingsFill("pack://siteoforigin:,,,/Interface/settings.png");

                switch (selectedTab)
                {
                    case 0:
                        SetHomeFill("pack://siteoforigin:,,,/Interface/home_click.png");
                        break;
                    case 1:
                        SetAddFill("pack://siteoforigin:,,,/Interface/add_click.png");
                        NewSnippet();
                        break;
                    case 2:
                        SetStarFill("pack://siteoforigin:,,,/Interface/star_click.png");
                        break;
                    case 3:
                        SetSettingsFill("pack://siteoforigin:,,,/Interface/settings_click.png");
                        break;
                }

                if (selectedTab != 1)
                {
                    // Reset editor fields
                    titleText.Text = "";
                    descriptionText.Text = "";
                    langText.Text = "";
                    codeText.Document.Blocks.Clear();

                    // Close the editor
                    codeSnippetInput.Visibility = Visibility.Collapsed;
                    editingSnippet = -1;
                    editingStarred = false;
                    editingNew = false;
                }

                LoadSnippets();
            }
            else if (editingSnippet > -1 && discard == 0)
            {
                Save();
            }
        }
        // Refresh snippets every time window focused
        private void Window_Activated(object sender, EventArgs e)
        {
            LoadSnippets();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.width = (int)Math.Round(Width);
            Properties.Settings.Default.height = (int)Math.Round(Height);
            Properties.Settings.Default.Save();

            if (editingSnippet > -1 || editingNew)
            {
                int discard = AskDiscard();

                if (discard == 0)
                    Save();
                else if (discard == 2)
                    e.Cancel = true;
            }
        }

        //========================================


        //========================================
        // Snippet functions

        // Get snippets from the save directory
        public void LoadSnippets()
        {
            // Check if a snippet is being edited
            if (editingSnippet == -1)
            {
                // Clear the main tab snippets panel
                panel.Children.Clear();

                // Get snippets from the save directory
                foreach (var file in Directory.GetFiles(savesPath))
                {
                    // Read all lines of snippet file
                    List<string> lines = File.ReadAllLines(file).ToList();

                    if (selectedTab != 2 || lines[0].Trim() == "Starred")
                    {
                        // Get title
                        int titleIndex = lines.IndexOf("[Title]") + 1;
                        string title = "New snippet";
                        if (titleIndex < lines.Count)
                            title = lines[titleIndex];

                        // Get description
                        int descriptionIndex = lines.IndexOf("[Description]") + 1;
                        string description = "";
                        if (descriptionIndex < lines.Count)
                            description = lines[descriptionIndex];

                        // Get code language
                        int langIndex = lines.IndexOf("[Language]") + 1;
                        string lang = "txt";
                        if (langIndex < lines.Count)
                            lang = lines[langIndex];

                        // Get code
                        string code = File.ReadAllText(file).Split(new string[] { "[Code]" }, StringSplitOptions.None)[1].TrimStart().TrimEnd();

                        // Add the snippet button visuals
                        AddSnippet(title, description, lang, code, file, lines[0].Trim() == "Starred");
                    }
                }
            }
        }
        // Add a new snippet
        private void AddSnippet(string title, string description, string lang, string code, string fileName, bool starred)
        {
            Snippet snippet = new Snippet(title, description, lang, code, buttonHandler, fileName);
            snippets.Add(snippet);
            AddPanel(title, description, lang, snippets.Count - 1, starred);
        }
        // Add a button for a snippet
        public void AddPanel(string title, string description, string lang, int id, bool starred)
        {
            try
            {
                StringReader sReader = new StringReader(snippetTemplate);
                XmlReader xReader = XmlReader.Create(sReader);
                Canvas template = (Canvas)XamlReader.Load(xReader);

                var icon = (Image)template.Children[0];
                var titleLabel = (Label)template.Children[1];
                var descLabel = (Label)template.Children[2];

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();

                if (File.Exists(Environment.CurrentDirectory + "/Interface/lang-icons/" + lang + ".png"))
                    logo.UriSource = new Uri("pack://siteoforigin:,,,/Interface/lang-icons/" + lang + ".png");
                else
                    logo.UriSource = new Uri("pack://siteoforigin:,,,/Interface/lang-icons/file.png");

                logo.EndInit();
                icon.Source = logo;
                titleLabel.Content = title;
                descLabel.Content = description;

                panel.Children.Add(template);

                snippets[id].Id = id;
                snippets[id].Starred = starred;
                snippets[id].SnippetButton = template;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load snippet: " + ex.Message);
            }
        }
        // Snippet pressed
        private void ButtonHandler_ButtonPressed(object sender, SnippetButtonPressedEventArgs e)
        {
            if (e.StarredSetting)
                SetSnippetStarred(e.Id, e.Starred);
            else
                SnippetPressed(e.Id, false, e.Starred);
        }
        private void SetSnippetStarred(int id, bool starred)
        {
            int tab = selectedTab;

            editingSnippet = id;
            editingStarred = starred;

            titleText.Text = snippets[id].title;
            descriptionText.Text = snippets[id].description;
            langText.Text = snippets[id].lang;

            codeText.Document.Blocks.Clear();
            codeText.Document.Blocks.Add(new Paragraph(new Run(snippets[id].code)));

            Save();

            ChangeTab(tab);
        }

        //========================================


        //========================================
        // Editor functions

        // New snippet
        private void NewSnippet()
        {
            editingStarred = false;

            SnippetPressed(snippets.Count, true, false);
        }
        // Save snippet
        private void Save()
        {
            if (editingSnippet > -1 && titleText.Text.Trim() != "" && codeText.Document.Blocks.Count > 0)
            {
                string file = "";

                if (editingStarred) file += "Starred\n";

                file += "[Title]\n"
                    + titleText.Text
                    + "\n[Description]\n"
                    + descriptionText.Text
                    + "\n[Language]\n"
                    + langText.Text
                    + "\n[Code]\n";

                if (editingNew)
                {
                    snippets.Add(new Snippet("", "", "", "", null, savesPath + "/" + titleText.Text + ".txt"));
                }

                TextRange textRange = new TextRange(codeText.Document.ContentStart, codeText.Document.ContentEnd);
                file += textRange.Text;

                string fileName = snippets[editingSnippet].fileName;

                while (File.Exists(fileName) && editingNew && !editingStarred)
                {
                    fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileName), System.IO.Path.GetFileNameWithoutExtension(fileName) + " (1)" + ".txt");
                }

                File.WriteAllText(fileName, file);

                saved = true;

                editingStarred = false;
                editingNew = false;
                editingSnippet = -1;

                ChangeTab(0);
                LoadSnippets();
            }
        }
        // Discard snippet
        private int AskDiscard()
        {
            saved = false;

            if (editingSnippet < 0 || codeText.Document.Blocks.Count <= 0) return 1;

            MessageBoxResult r = MessageBox.Show("Do you want to save the made changes?", "SnipCode", MessageBoxButton.YesNoCancel);

            if (r == MessageBoxResult.Yes)
            {
                return 0;
            }
            else if (r == MessageBoxResult.No)
            {
                return 1;
            }
            else if (r == MessageBoxResult.Cancel)
            {
                return 2;
            }

            return 2;
        }

        //========================================


        //========================================
        // UI appearance

        // Home tab button highlight coloring
        private void home_MouseEnter(object sender, MouseEventArgs e)
        {
            SetHomeFill("pack://siteoforigin:,,,/Interface/home_fill.png");
        }
        private void home_MouseLeave(object sender, MouseEventArgs e)
        {
            SetHomeFill("pack://siteoforigin:,,,/Interface/home.png");
        }
        private void home_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetHomeFill("pack://siteoforigin:,,,/Interface/home_click.png");
                ChangeTab(0);
            }
        }
        private void home_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetHomeFill("pack://siteoforigin:,,,/Interface/home.png");
        }
        private void SetHomeFill(string path)
        {
            if (selectedTab == 0)
                home.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "pack://siteoforigin:,,,/Interface/home_click.png")));
            else
                home.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), path)));
        }

        // Add snippet tab button highlight coloring
        private void add_MouseEnter(object sender, MouseEventArgs e)
        {
            SetAddFill("pack://siteoforigin:,,,/Interface/add_fill.png");
        }
        private void add_MouseLeave(object sender, MouseEventArgs e)
        {
            SetAddFill("pack://siteoforigin:,,,/Interface/add.png");
        }
        private void add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetAddFill("pack://siteoforigin:,,,/Interface/add_click.png");
                ChangeTab(1);
            }
        }
        private void add_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetAddFill("pack://siteoforigin:,,,/Interface/add.png");
        }
        private void SetAddFill(string path)
        {
            if (selectedTab == 1)
                add.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "pack://siteoforigin:,,,/Interface/add_click.png")));
            else
                add.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), path)));
        }

        // Favourites tab button highlight coloring
        private void star_MouseEnter(object sender, MouseEventArgs e)
        {
            SetStarFill("pack://siteoforigin:,,,/Interface/star_fill.png");
        }
        private void star_MouseLeave(object sender, MouseEventArgs e)
        {
            SetStarFill("pack://siteoforigin:,,,/Interface/star.png");
        }
        private void star_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetStarFill("pack://siteoforigin:,,,/Interface/star_click.png");
                ChangeTab(2);
            }
        }
        private void star_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetStarFill("pack://siteoforigin:,,,/Interface/star.png");
        }
        private void SetStarFill(string path)
        {
            if (selectedTab == 2)
                star.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "pack://siteoforigin:,,,/Interface/star_click.png")));
            else
                star.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), path)));
        }

        // Settings tab button highlight coloring
        private void settings_MouseEnter(object sender, MouseEventArgs e)
        {
            SetSettingsFill("pack://siteoforigin:,,,/Interface/settings_fill.png");
        }
        private void settings_MouseLeave(object sender, MouseEventArgs e)
        {
            SetSettingsFill("pack://siteoforigin:,,,/Interface/settings.png");
        }
        private void settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SetSettingsFill("pack://siteoforigin:,,,/Interface/settings_click.png");
                ChangeTab(3);
            }
        }
        private void settings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetSettingsFill("pack://siteoforigin:,,,/Interface/settings.png");
        }
        private void SetSettingsFill(string path)
        {
            if (selectedTab == 3)
                settings.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "pack://siteoforigin:,,,/Interface/settings_click.png")));
            else
                settings.Fill = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), path)));
        }

        //========================================


        //========================================
        // Main tab buttons

        // Snippet pressed
        private void SnippetPressed(int id, bool isNew, bool isStarred)
        {
            editingNew = isNew;

            if (!isNew)
            {
                editingStarred = isStarred;

                codeSnippetInput.Visibility = Visibility.Visible;

                titleText.Text = snippets[id].title;
                descriptionText.Text = snippets[id].description;
                langText.Text = snippets[id].lang;

                codeText.Document.Blocks.Clear();
                codeText.Document.Blocks.Add(new Paragraph(new Run(snippets[id].code)));

                editingSnippet = id;
            }
            else
            {
                codeSnippetInput.Visibility = Visibility.Visible;

                titleText.Text = "New Snippet";
                descriptionText.Text = "";
                langText.Text = "";

                codeText.Document.Blocks.Clear();

                editingSnippet = id;
            }
        }
        // Refresh button pressed
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSnippets();
        }
        // New snippet button pressed
        private void topnew_Click(object sender, RoutedEventArgs e)
        {
            NewSnippet();
        }

        //========================================


        //========================================
        // Snippet edit form buttons

        // Set saved boolean
        bool saved = false;
        // Save snippet button click
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        // Cancel button
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            var discard = AskDiscard();

            if (discard == 1)
            {
                saved = true;

                editingSnippet = -1;
                ChangeTab(0);
                LoadSnippets();
            }
            else if (discard == 0)
            {
                Save();
            }
        }

        //========================================


        //========================================
        // Hotkeys

        // Check for hotkeys
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Main hotkeys
            if (editingSnippet < 0)
            {
                // Check for new snip hotkey
                if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    NewSnippet();
                }

                // Check for paste hotkey
                if (Clipboard.GetText(TextDataFormat.Text) != null && e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    NewSnippet();

                    string text = Clipboard.GetText(TextDataFormat.Text);
                    codeText.AppendText(text);
                }
            }
            // Editor hotkeys
            else
            {
                // Check for save hotkey
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    Save();
                }

                // Close editor without saving
                if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to discard the changes\nand close the editor?", "Are you sure?", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        editingSnippet = -1;
                        ChangeTab(0);
                    }
                }
            }
        }

        //========================================
    }
}
