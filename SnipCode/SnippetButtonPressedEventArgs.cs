using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipCode
{
    public class SnippetButtonPressedEventArgs : EventArgs
    {
        public int Id { get; set; }
        public bool Starred { get; set; }
        public bool StarredSetting { get; set; }
    }
}
