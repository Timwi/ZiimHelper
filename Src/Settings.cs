using RT.PostBuild;
using RT.Serialization;
using RT.Util;
using RT.Util.Forms;

namespace ZiimHelper
{
    enum EditMode
    {
        MoveSelect,
        Draw,
        SetLabelPosition
    }

    [Settings("ZiimHelper", SettingsKind.UserSpecific)]
    sealed class Settings : SettingsBase
    {
        public bool ViewGrid = true;
        public bool ViewConnectionLines = true;
        public bool ViewInstructions = true;
        public bool ViewAnnotations = true;
        public bool ViewInnerClouds = true;
        public bool ViewOwnCloud = true;
        public bool ViewCoordinates = true;
        public EditMode EditMode = EditMode.MoveSelect;
        public ManagedForm.Settings FormSettings = new ManagedForm.Settings();

        public int LastCopyImageFontSize = 24;
        public int LastCopyImageWidth = 1000;
        public int LastCopyImageHeight = 1000;

        private static void PostBuildCheck(IPostBuildReporter rep)
        {
            Classify.PostBuildStep(typeof(Settings), rep);
            Classify.PostBuildStep(typeof(Cloud), rep);
        }
    }
}
