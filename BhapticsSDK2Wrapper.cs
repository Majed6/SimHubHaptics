using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SimHub;
using WebSocket = WebSocketSharp.WebSocket;

namespace User.PluginSdk
{
    /*
     * Possible positions:
     * Vest,
     *  VestFront,
     *  VestBack,
     *  ForearmL,
     *  ForearmR,
     *  Head,
     *  HandL,
     *  HandR,
     *  FootL,
     *  FootR,
     *  GloveL,
     *  GloveR
     */
    public class BhapticsSdk2Wrapper
    {
        private static WebSocket ws;

        public static void Initialize()
        {
            try
            {
                ws = new WebSocket(
                    "ws://localhost:15881/v2/feedbacks?app_name=SimHubHaptics&app_id=com.simhubhaptics.simhubhaptics");
                ws.Connect();
                Task.Run(() => ReceiveFrames());
                RegisterAll();
            }
            catch (Exception e)
            {
                Logging.Current.Info("Failed to connect to bHaptics Player");
                LogException(e);
            }
        }

        private static void LogException(Exception e)
        {
            if (e.InnerException != null) LogException(e.InnerException);
            Logging.Current.Info(e.Message);
            Logging.Current.Info(e.StackTrace);
        }

        private static async Task ReceiveFrames()
        {
            while (ws != null && ws.IsAlive) await Task.Delay(100);
        }

        public static void RegisterAll()
        {
            Logging.Current.Info("Registering all tacts");
            var tactsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tacts");
            Logging.Current.Info($"Tacts folder: {tactsFolder}");
            var files = Directory.GetFiles(Path.Combine(tactsFolder, "light"));
            files = files.Concat(Directory.GetFiles(Path.Combine(tactsFolder, "full"))).ToArray();
            foreach (var filename in files)
            {
                Logging.Current.Info($"Registering {filename}");
                var key = Path.GetFileNameWithoutExtension(filename);
                Register(key, filename);
                Logging.Current.Info($"Registered {key}");
            }
        }

        public static bool IsConnected()
        {
            return ws != null && ws.IsAlive;
        }

        public static void Destroy()
        {
            if (ws != null)
            {
                ws.Close();
                ws = null;
            }
        }

        public static void Register(string key, string fileDirectory)
        {
            var jsonData = File.ReadAllText(fileDirectory);
            Logging.Current.Info(jsonData);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
            var project = JsonSerializer.Deserialize<Dictionary<string, object>>(data["project"].ToString());
            var layout = project["layout"];
            var tracks = project["tracks"];

            var request = new
            {
                Register = new[]
                {
                    new
                    {
                        Key = key,
                        Project = new
                        {
                            Tracks = tracks,
                            Layout = layout
                        }
                    }
                }
            };

            var jsonStr = JsonSerializer.Serialize(request);
            Logging.Current.Info(jsonStr);
            Submit(jsonStr);
        }

        public static void SubmitRegistered(string key)
        {
            var request = new
            {
                Submit = new[]
                {
                    new
                    {
                        Type = "key",
                        Key = key
                    }
                }
            };

            var jsonStr = JsonSerializer.Serialize(request);
            Submit(jsonStr);
        }

        public static void SubmitRegisteredWithOption(string key, string altKey, Dictionary<string, double> scaleOption,
            Dictionary<string, double> rotationOption)
        {
            var request = new
            {
                Submit = new[]
                {
                    new
                    {
                        Type = "key",
                        Key = key,
                        Parameters = new
                        {
                            altKey,
                            rotationOption,
                            scaleOption
                        }
                    }
                }
            };

            var jsonStr = JsonSerializer.Serialize(request);
            Submit(jsonStr);
        }

        public static void Submit(string key, Dictionary<string, object> frame)
        {
            var request = new
            {
                Submit = new[]
                {
                    new
                    {
                        Type = "frame",
                        Key = key,
                        Frame = frame
                    }
                }
            };

            var jsonStr = JsonSerializer.Serialize(request);
            Submit(jsonStr);
        }

        public static void SubmitDot(string key, string position, List<Dictionary<string, int>> dotPoints,
            int durationMillis)
        {
            var frontFrame = new Dictionary<string, object>
            {
                { "position", position },
                { "dotPoints", dotPoints },
                { "durationMillis", durationMillis }
            };
            Submit(key, frontFrame);
        }

        public static void SubmitPath(string key, string position, List<Dictionary<string, int>> pathPoints,
            int durationMillis)
        {
            var frontFrame = new Dictionary<string, object>
            {
                { "position", position },
                { "pathPoints", pathPoints },
                { "durationMillis", durationMillis }
            };
            Submit(key, frontFrame);
        }

        private static void Submit(string jsonStr)
        {
            if (ws != null && ws.IsAlive) ws.Send(jsonStr);
        }
    }
}