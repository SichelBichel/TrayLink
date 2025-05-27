using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayLink
{
    public class ActionConfig
    {
        public string ActionName { get; set; } = "GenericAction";
        public string ActionType { get; set; } = "URL/FILE/PATH";
        public string PathOrUrl { get; set; } = "Filepath/Path/URL";
    }
}
