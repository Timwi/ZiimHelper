using System.Collections.Generic;
using RT.Util;
using RT.Util.Forms;

namespace ZiimHelper
{
    enum EditMode
    {
        MoveSelect,
        Draw
    }

    [Settings("ZiimHelper", SettingsKind.UserSpecific)]
    sealed class Settings : SettingsBase
    {
        public bool ViewGrid = true;
        public bool ViewConnectionLines = true;
        public bool ViewInstructions = true;
        public bool ViewAnnotations = true;
        public EditMode EditMode = EditMode.MoveSelect;
        public ManagedForm.Settings FormSettings = new ManagedForm.Settings();
    }
}
