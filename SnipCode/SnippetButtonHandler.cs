using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipCode
{
    public class SnippetButtonHandler
    {
        public event EventHandler<SnippetButtonPressedEventArgs> ButtonPressed;

        protected virtual void OnButtonPressed(SnippetButtonPressedEventArgs e)
        {
            EventHandler<SnippetButtonPressedEventArgs> handler = ButtonPressed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void InvokePress(SnippetButtonPressedEventArgs e)
        {
            ButtonPressed.Invoke(this, e);
        }
    }
}
