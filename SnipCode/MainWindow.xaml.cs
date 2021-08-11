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
        private List<Snippet> snippets = new List<Snippet>();

        private int editingSnippet = -1;
        private int selectedTab = 0;
        private string snippetTemplate;

        private SnippetButtonHandler buttonHandler;

        private string savesPath = @"C:\";

        public MainWindow()
        {
            InitializeComponent();

            savesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SnipCode/Saves");

            if (!Directory.Exists(savesPath))
                Directory.CreateDirectory(savesPath);

            buttonHandler = new SnippetButtonHandler();
            buttonHandler.ButtonPressed += ButtonHandler_ButtonPressed;

            snippetTemplate = XamlWriter.Save(snippet);

            snippet.Visibility = Visibility.Collapsed;

            codeSnippetInput.Visibility = Visibility.Collapsed;

            LoadSnippets();
            ChangeTab(0);
        }

        private void ButtonHandler_ButtonPressed(object sender, SnippetButtonPressedEventArgs e)
        {
            SnippetPressed(e.id);
        }

        public void AddPanel(string title, string description, string lang, int id)
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
                logo.UriSource = new Uri("pack://siteoforigin:,,,/Interface/lang-icons/" + lang + ".png");
                logo.EndInit();
                icon.Source = logo;
                titleLabel.Content = title;
                descLabel.Content = description;

                panel.Children.Add(template);

                snippets[id].Id = id;
                snippets[id].SnippetButton = template;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load snippet: " + ex.Message);
            }
        }

        public void ChangeTab(int tab)
        {
            if ((editingSnippet > -1 && AskDiscard()) || editingSnippet == -1)
            {
                selectedTab = tab;

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
                        break;
                    case 2:
                        SetStarFill("pack://siteoforigin:,,,/Interface/star_click.png");
                        break;
                    case 3:
                        SetSettingsFill("pack://siteoforigin:,,,/Interface/settings_click.png");
                        break;
                }

                titleText.Text = "";
                descriptionText.Text = "";
                langText.Text = "";

                codeText.Document.Blocks.Clear();

                codeSnippetInput.Visibility = Visibility.Collapsed;
                editingSnippet = -1;
            }
        }

        public void LoadSnippets()
        {
            if (editingSnippet == -1)
            {
                panel.Children.Clear();

                foreach (var file in Directory.GetFiles(savesPath))
                {
                    List<string> lines = File.ReadAllLines(file).ToList();

                    int titleIndex = lines.IndexOf("[Title]") + 1;
                    string title = "New snippet";
                    if (titleIndex < lines.Count)
                        title = lines[titleIndex];

                    int descriptionIndex = lines.IndexOf("[Description]") + 1;
                    string description = "";
                    if (descriptionIndex < lines.Count)
                        description = lines[descriptionIndex];

                    int langIndex = lines.IndexOf("[Language]") + 1;
                    string lang = "txt";
                    if (langIndex < lines.Count)
                        lang = lines[langIndex];

                    string code = File.ReadAllText(file).Split(new string[] { "[Code]" }, StringSplitOptions.None)[1].TrimStart().TrimEnd();

                    AddSnippet(title, description, lang, code, file);
                }
            }
        }
        private void AddSnippet(string title, string description, string lang, string code, string fileName)
        {
            Snippet snippet = new Snippet(title, description, lang, code, buttonHandler, fileName);
            snippets.Add(snippet);
            AddPanel(title, description, lang, snippets.Count - 1);
        }

        private void SnippetPressed(int id)
        {
            codeSnippetInput.Visibility = Visibility.Visible;

            titleText.Text = snippets[id].title;
            descriptionText.Text = snippets[id].description;
            langText.Text = snippets[id].lang;

            codeText.Document.Blocks.Clear();
            codeText.Document.Blocks.Add(new Paragraph(new Run(snippets[id].code)));

            editingSnippet = id;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void starsButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSnippets();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LoadSnippets();
        }


        //========================================
        // Editor functions
        //========================================

        // New snippet
        private void NewSnippet()
        {

        }
        // Save snippet
        private void Save()
        {
            if (editingSnippet > -1)
            {
                string file = "[Title]\n"
                    + titleText.Text
                    + "\n[Description]\n"
                    + descriptionText.Text
                    + "\n[Language]\n"
                    + langText.Text
                    + "\n[Code]\n";

                TextRange textRange = new TextRange(codeText.Document.ContentStart, codeText.Document.ContentEnd);
                file += textRange.Text;

                File.WriteAllText(snippets[editingSnippet].fileName, file);

                saved = true;

                editingSnippet = -1;
                ChangeTab(0);
                LoadSnippets();
            }
        }
        // Discard snippet
        private bool AskDiscard()
        {
            saved = false;

            if (saved && editingSnippet == -1) return true;

            MessageBoxResult r = MessageBox.Show("Are you sure you want to discard made changes?", "Are you sure?", MessageBoxButton.YesNoCancel);

            if (r == MessageBoxResult.Yes)
            {
                return true;
            }

            return false;
        }

        //========================================
        // UI appearance
        //========================================

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
            if (AskDiscard())
            {
                saved = true;

                editingSnippet = -1;
                ChangeTab(0);
                LoadSnippets();
            }
        }

        // Check for editor hotkeys
        private void codeSnippetInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (editingSnippet > -1)
            {
                // Check for save hotkey
                if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    Save();
                }
            }
        }

        // Check for main hotkeys
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (editingSnippet == -1)
            {
                // Check for new snip hotkey
                if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    NewSnippet();
                }
            }
        }
    }
}
