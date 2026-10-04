using MelonLoader;

[assembly: MelonInfo(typeof(IncreasedStackLimitLatest.Core), "IncreasedStackLimit-Latest", "1.0.0", "Malekabokhatwa",
    "https://github.com/Malekabokhatwa/IncreasedStackLimit-Latest")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace IncreasedStackLimitLatest;

public sealed class Core : MelonMod
{
    internal static MelonLogger.Instance Log;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;
        Settings.Init();
        Log.Msg($"Stack rule: {Settings.Describe()}");
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        StackLimits.ApplyAll();
        Stations.ApplyAll();
    }

    public override void OnUpdate()
    {
        if (!Settings.ReloadIfChanged(UnityEngine.Time.unscaledTime))
            return;

        Log.Msg($"Settings reloaded. Stack rule: {Settings.Describe()}");
        StackLimits.ApplyAll();
        Stations.ApplyAll();
    }
}
