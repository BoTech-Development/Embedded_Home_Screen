using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embedded_Home_Screen.ViewModels.Dialogs.Update
{
    public class NewUpdateAvailableDialogViewModel
    {
        public string CurrentVersion { get; set; }
        public string NextVersion { get; set; }
        public string LatestVersion { get; set; }
        public string ReleaseNotes { get; set; }
        public string PublishedAt {  get; set; }
    }
}
