using RT.Util;
using RT.Util.Forms;
using RT.Util.Serialization;

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
        public bool ViewInnerClouds = true;
        public bool ViewOwnCloud = true;
        public bool ViewCoordinates = true;
        public EditMode EditMode = EditMode.MoveSelect;
        public ManagedForm.Settings FormSettings = new ManagedForm.Settings();

        private static void PostBuildCheck(IPostBuildReporter rep)
        {
            Classify.PostBuildStep(typeof(Settings), rep);
            Classify.PostBuildStep(typeof(Cloud), rep);
        }
    }
}
