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
using CefSharp;
using CefSharp.Wpf;

namespace SnipCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeChromium();
        }

        public ChromiumWebBrowser chromeBrowser;

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("file:///C:/GitHub/SnipCode/SnipCode/Interface/index.html");
            // Add it to the form and fill it to the form window.
            this.Controls.Children.Add(chromeBrowser);
            chromeBrowser.HorizontalAlignment = HorizontalAlignment.Stretch;
            chromeBrowser.VerticalAlignment = VerticalAlignment.Stretch;
            chromeBrowser.Margin = new Thickness(0, 0, 0, 0);
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

        private void FavouritesButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
