//using BepInEx;
//using BepInEx.Logging;
//using HarmonyLib;
//using System;
//using System.Linq;
//using System.Reflection;

//[HarmonyPatch]
//public class LogSuppressorPatch
//{
//    // We target the internal method that Unity uses to send logs to the console
//    [HarmonyTargetMethod]
//    static MethodBase TargetMethod()
//    {
//        return AccessTools.Method(typeof(UnityEngine.Debug), "LogPlayerBuildError", new Type[] { typeof(string), typeof(string), typeof(int), typeof(int) })
//               ?? AccessTools.Method(typeof(UnityEngine.Debug), "LogError", new Type[] { typeof(object) });
//    }

//    [HarmonyPrefix]
//    public static bool Prefix(object message)
//    {
//        if (message == null) return true;
//        string msg = message.ToString();

//        // Check for both the Volumetric error and the Jobified rendering warning
//        if (msg.Contains("Volumetric Materials") || msg.Contains("jobified rendering"))
//        {
//            return false; // Blocks the engine from sending this to the log
//        }
//        return true;
//    }
//}



//[BepInPlugin("com.yourname.logcleaner", "Log Cleaner", "1.0.0")]
//public class LogCleaner : BaseUnityPlugin
//{
//    private void Awake()
//    {
//        // IMPORTANT: Use the STATIC logger, not BaseUnityPlugin.Logger
//        var listeners = BepInEx.Logging.Logger.Listeners.ToList();

//        // Remove console listeners
//        foreach (var listener in listeners)
//        {
//            if (listener.GetType().Name.Contains("Console"))
//            {
//                BepInEx.Logging.Logger.Listeners.Remove(listener);
//            }
//        }

//        // Add filtered listener
//        BepInEx.Logging.Logger.Listeners.Add(new FilteredConsoleListener());

//        Logger.LogInfo("Log cleaner loaded");
//    }
//}

//public class FilteredConsoleListener : ILogListener
//{
//    private readonly ConsoleLogListener console = new ConsoleLogListener();

//    public void LogEvent(object sender, LogEventArgs eventArgs)
//    {
//        if (eventArgs.Data == null)
//            return;

//        string msg = eventArgs.Data.ToString();

//        if (msg.Contains("Dissonance:Recording") ||
//            msg.Contains("global startposition") ||
//            msg.Contains("Setting scrap value for item:") ||
//            msg.Contains("spawn position: (") ||
//            msg.Contains("item awake:") ||
//            msg.Contains("placedPosition:") ||
//            msg.Contains("landed on : ShipInside / ShipInside")||
//            msg.Contains("BoxCollider does not support negative") ||
//            msg.Contains("Loading item save data for item:") ||
//            msg.Contains("); rot: (") ||
//            msg.Contains("Instantiating prefab for item") ||
//            msg.Contains("unlockable index:") ||
//            msg.Contains("; item parent :")||
//            msg.Contains("dropping item did not get raycast :") ||
//            msg.Contains("Loading placed object position as: (") ||
//            msg.Contains("Saving placed position as: (") ||
//            msg.Contains("Item sales percentages #") ||
//            msg.Contains("Loading from data: for") ||
//            msg.Contains("item name: ") ||
//            msg.Contains("Suit #") ||
//            msg.Contains(" landed on :") ||
//            msg.Contains("Audio source failed to initialize audio spatializer") ||
//            msg.Contains("Only custom filters can be played") ||
//            msg.Contains("Start falling position"))

//        {
//            return;
//        }

//        console.LogEvent(sender, eventArgs);
//    }

//    public void Dispose()
//    {
//        console.Dispose();
//    }
//}