using _simpleWorkoutQTE.Patches;
using BepInEx;
using BepInEx.Logging;

namespace _simpleWorkoutQTE
{
    [BepInPlugin("com.acidphantasm.simpleworkoutqte", "acidphantasm-simpleworkoutqte", "2.2.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        internal void Awake()
        {
            LogSource = Logger;

            new WorkoutPatch().Enable();
        }
    }
}