using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public ManagedForm.Settings FormSettings = new ManagedForm.Settings();
    }
}
