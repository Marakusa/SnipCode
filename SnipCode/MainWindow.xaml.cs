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

        private int selectedTab = 0;
        private string snippetTemplate;

        private SnippetButtonHandler buttonHandler;

        public MainWindow()
        {
            InitializeComponent();

            buttonHandler = new SnippetButtonHandler();
            buttonHandler.ButtonPressed += ButtonHandler_ButtonPressed;

            snippetTemplate = XamlWriter.Save(snippet);

            snippet.Visibility = Visibility.Collapsed;

            LoadSnippets();
            ChangeTab(0);
        }

        private void ButtonHandler_ButtonPressed(object sender, SnippetButtonPressedEventArgs e)
        {
            SnippetPressed(e.id);
        }

        public void AddPanel(string title, string description, int id)
        {
            StringReader sReader = new StringReader(snippetTemplate);
            XmlReader xReader = XmlReader.Create(sReader);
            Canvas template = (Canvas)XamlReader.Load(xReader);

            var titleLabel = (Label)template.Children[1];
            var descLabel = (Label)template.Children[2];

            titleLabel.Content = title;
            descLabel.Content = description;

            panel.Children.Add(template);

            snippets[id].Id = id;
            snippets[id].SnippetButton = template;
        }

        public void ChangeTab(int tab)
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
        }

        public void LoadSnippets()
        {
            AddSnippet("Example", "This is an example snippet.", "js", "print(\"Hello world!\");");
            AddSnippet("Example 2", "This is an another example snippet.", "cs", "Console.WriteLine(\"Hello world!\");");
        }
        private void AddSnippet(string title, string description, string lang, string code)
        {
            Snippet snippet = new Snippet(title, description, lang, code, buttonHandler);
            snippets.Add(snippet);
            AddPanel(title, description, snippets.Count - 1);
        }

        private void SnippetPressed(int id)
        {
            MessageBox.Show(snippets[id].code);
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
    }
}
