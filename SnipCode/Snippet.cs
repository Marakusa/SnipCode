using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnipCode
{
    public class Snippet
    {
        public string fileName;
        public string title;
        public string description;
        public string code;
        public string lang;

        private int id;
        private bool starred = false;
        private Canvas snippetButton;
        private SnippetButtonHandler buttonHandler;

        public Snippet(string title, string description, string lang, string code, SnippetButtonHandler buttonHandler, string fileName)
        {
            this.title = title;
            this.description = description;
            this.lang = lang;
            this.code = code;
            this.buttonHandler = buttonHandler;
            this.fileName = fileName;
        }

        public Canvas SnippetButton
        {
            get
            {
                return snippetButton;
            }
            set
            {
                snippetButton = value;

                snippetButton.MouseEnter += SnippetButton_MouseEnter;
                snippetButton.MouseLeave += SnippetButton_MouseLeave;
                snippetButton.MouseLeftButtonDown += SnippetButton_MouseLeftButtonDown;
                snippetButton.MouseLeftButtonUp += SnippetButton_MouseLeftButtonUp;

                ((Button)snippetButton.Children[3]).Click += StarButton_Click;

                if (snippetButton != null)
                    ((Button)snippetButton.Children[3]).Content = starred ? "Unstar" : "Star";
            }
        }

        public int Id
        {
            set
            {
                id = value;
            }
        }
        public bool Starred
        {
            get
            {
                return starred;
            }
            set
            {
                starred = value;

                if (snippetButton != null)
                    ((Button)snippetButton.Children[3]).Content = starred ? "Unstar" : "Star";
            }
        }

        private void SnippetButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            snippetButton.Background = new SolidColorBrush(Color.FromRgb(215, 215, 215));
        }
        private void SnippetButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            snippetButton.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));
        }

        private void SnippetButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            snippetButton.Background = new SolidColorBrush(Color.FromRgb(224, 224, 224));

            SnippetButtonPressedEventArgs args = new SnippetButtonPressedEventArgs();
            args.Id = id;
            args.Starred = starred;
            args.StarredSetting = false;
            buttonHandler.InvokePress(args);
        }
        private void SnippetButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            snippetButton.Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        }

        private void StarButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            starred = !starred;

            SnippetButtonPressedEventArgs args = new SnippetButtonPressedEventArgs();
            args.Id = id;
            args.Starred = starred;
            args.StarredSetting = true;
            buttonHandler.InvokePress(args);
        }
    }
}
