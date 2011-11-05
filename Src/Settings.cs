using System.Collections.Generic;
using RT.Util;
using RT.Util.Forms;

namespace ZiimHelper
{
    [Settings("ZiimHelper", SettingsKind.UserSpecific)]
    sealed class Settings : SettingsBase
    {
        public List<ArrowInfo> Arrows = new List<ArrowInfo>();
        public List<int> SelectedIndices = new List<int>();
        public int OutlineIndex = 0;
        public bool ViewGrid = true;
        public bool ViewConnectionLines = true;
        public bool ViewInstructions = true;
        public bool ViewAnnotations = true;
        public ManagedForm.Settings FormSettings = new ManagedForm.Settings();
    }
}
