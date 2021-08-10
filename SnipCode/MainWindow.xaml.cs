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
        private int selectedTab = 0;

        Canvas newCanvas;

        public MainWindow()
        {
            InitializeComponent();
            
            string saved = XamlWriter.Save(snippet);
            StringReader sReader = new StringReader(saved);
            XmlReader xReader = XmlReader.Create(sReader);
            newCanvas = (Canvas)XamlReader.Load(xReader);

            ChangeTab(0);
        }

        public void AddPanel(string title, string desc)
        {
            newCanvas.Children[1].
            panel.Children.Add(newCanvas);
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
