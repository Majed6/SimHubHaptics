using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using GameReaderCommon;
using SimHub.Plugins;
using User.PluginSdk.Properties;

namespace User.PluginSdk
{
    [PluginDescription("A bHaptics Plugin For SimHub")]
    [PluginAuthor("Author")]
    [PluginName("SimHubHaptics")]
    public class DataPlugin : IPlugin, IDataPlugin, IWPFSettingsV2
    {
        public DataPluginSettings Settings;

        /// <summary>
        ///     Called one time per game data update, contains all normalized game data,
        ///     raw data are intentionally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
        ///     This method is on the critical path, it must execute as fast as possible and avoid throwing any error
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including current and previous data frame.</param>
        public void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            // Check if the game is running and in a race
            if (data.GameRunning && !data.GameInMenu && !data.GamePaused)
            {
                if (Settings.Theme == Theme.Light) LightHaptics.DataUpdate(pluginManager, ref data);
                // Full Haptics
            }
        }

        /// <summary>
        ///     Instance of the current plugin manager
        /// </summary>
        public PluginManager PluginManager { get; set; }

        /// <summary>
        ///     Called at plugin manager stop, close/dispose anything needed here !
        ///     Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void End(PluginManager pluginManager)
        {
            // Save settings
            this.SaveCommonSettings("GeneralSettings", Settings);
        }

        /// <summary>
        ///     Called once after plugins startup
        ///     Plugins are rebuilt at game change
        /// </summary>
        /// <param name="pluginManager"></param>
        public void Init(PluginManager pluginManager)
        {
            BhapticsSdk2Wrapper.Initialize();
            // Load settings
            Settings = this.ReadCommonSettings("GeneralSettings", () => new DataPluginSettings());
        }

        /// <summary>
        ///     Gets the left menu icon. Icon must be 24x24 and compatible with black and white display.
        /// </summary>
        public ImageSource PictureIcon => this.ToIcon(Resources.sdkmenuicon);

        /// <summary>
        ///     Gets a short plugin title to show in left menu. Return null if you want to use the title as defined in PluginName
        ///     attribute.
        /// </summary>
        public string LeftMenuTitle => "SimHubHaptics";

        /// <summary>
        ///     Returns the settings control, return null if no settings control is required
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <returns></returns>
        public Control GetWPFSettingsControl(PluginManager pluginManager)
        {
            return new SettingsControl(this);
        }
    }
}