using System.Windows;
using System.Windows.Controls;

namespace User.PluginSdk
{
    /// <summary>
    ///     Logique d'interaction pour SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(DataPlugin plugin) : this()
        {
            Plugin = plugin;
            ThemeComboBox.SelectedIndex = Plugin.Settings.Theme == Theme.Light ? 0 : 1;
        }

        public DataPlugin Plugin { get; }

        private void ThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            var selectedText = selectedItem.Content.ToString();
            // Set the theme in DataPluginSettings
            Plugin.Settings.Theme = selectedText == "Light" ? Theme.Light : Theme.Full;
        }


        private void ConnectToBHaptics(object sender, RoutedEventArgs e)
        {
            BhapticsSdk2Wrapper.Initialize();
        }
    }
}