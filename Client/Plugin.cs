using BepInEx;
using BepInEx.Logging;

namespace _simpleWorkoutQTE
{
    [BepInPlugin("com.acidphantasm.simpleworkoutqte", "acidphantasm-simpleworkoutqte", "2.2.2")]
    public class Plugin : BaseUnityPlugin
    {
        internal void Awake()
        {
            // Client side needs to go away but I'll do that later later later. If I do it now then people will leave it installed.
            //new WorkoutPatch().Enable();
        }
    }
}