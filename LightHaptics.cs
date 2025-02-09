using System.Collections.Generic;
using GameReaderCommon;
using SimHub.Plugins;

namespace User.PluginSdk
{
    public class LightHaptics
    {
        public static void DataUpdate(PluginManager pluginManager, ref GameData data)
        {
            if (data.NewData != null && data.NewData.Throttle > 0)
                Throttle((int)data.NewData.Throttle);

            if (data.NewData != null && data.OldData != null)
            {
                if (data.NewData.Throttle > 0 && data.OldData.Throttle == 0 && data.NewData.SpeedKmh > 0)
                {
                    BhapticsSdk2Wrapper.SubmitRegisteredWithOption(
                        "launch_vest_0",
                        "strong_launch_vest",
                        new Dictionary<string, double> { { "intensity", 3}, {"duration", 1 } },
                        new Dictionary<string, double> { { "offsetAngleX", 0},{ "offsetY", 0 } }
                    );
                    BhapticsSdk2Wrapper.SubmitRegisteredWithOption(
                        "launch_visor_0",
                        "strong_launch_visor",
                        new Dictionary<string, double> { { "intensity", 3}, {"duration", 1 } },
                        new Dictionary<string, double> { { "offsetAngleX", 0},{ "offsetY", 0 } }
                    );
                }

                if (data.NewData.Brake > 0 && data.OldData.Brake == 0 && data.NewData.SpeedKmh > 0)
                    Brake();
            }
        }
        public static void Throttle(int intensity)
        {
            intensity = intensity / 20 * 10;
            var throttleDots = new List<Dictionary<string, int>>
            {
                new Dictionary<string, int>
                {
                    { "index", 0 },
                    { "intensity", intensity }
                },
                new Dictionary<string, int>
                {
                    { "index", 1 },
                    { "intensity", intensity }
                },
                new Dictionary<string, int>
                {
                    { "index", 2 },
                    { "intensity", intensity }
                }
            };
            BhapticsSdk2Wrapper.SubmitDot("Throttle", "FootR", throttleDots, 100);
        }
        public static void Brake()
        {
            var brakeDots = new List<Dictionary<string, int>>
            {
                new Dictionary<string, int>
                {
                    { "index", 0 },
                    { "intensity", 30 }
                },
                new Dictionary<string, int>
                {
                    { "index", 1 },
                    { "intensity", 30 }
                },
                new Dictionary<string, int>
                {
                    { "index", 2 },
                    { "intensity", 30 }
                }
            };
            BhapticsSdk2Wrapper.SubmitDot("Brake", "FootL", brakeDots, 100);
            BhapticsSdk2Wrapper.SubmitRegisteredWithOption(
                "brake_vest_0",
                "strong_brake",
                new Dictionary<string, double> { { "intensity", 3}, {"duration", 1 } },
                new Dictionary<string, double> { { "offsetAngleX", 0},{ "offsetY", 0 } }
            );
        }
    }
}