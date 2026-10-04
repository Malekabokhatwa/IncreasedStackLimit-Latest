using System.Collections.Generic;
using HarmonyLib;
using Il2CppScheduleOne.ObjectScripts;
using Object = UnityEngine.Object;

namespace IncreasedStackLimitLatest;

/// <summary>
/// Opt-in station tweaks. A value of 0 in the settings keeps the station's own default.
/// Changes apply to new and existing stations; station UI sliders pick up a new maximum after a save reload.
/// </summary>
internal static class Stations
{
    private static readonly Dictionary<int, (int Capacity, int TimePerItem)> MixingDefaults = new();
    private static readonly Dictionary<int, int> RackDefaults = new();

    internal static void ApplyAll()
    {
        foreach (var station in Object.FindObjectsOfType<MixingStation>())
            Apply(station);
        foreach (var rack in Object.FindObjectsOfType<DryingRack>())
            Apply(rack);
    }

    internal static void Apply(MixingStation station)
    {
        if (station == null)
            return;

        int key = station.GetInstanceID();
        if (!MixingDefaults.TryGetValue(key, out var defaults))
        {
            defaults = (station.MaxMixQuantity, station.MixTimePerItem);
            MixingDefaults[key] = defaults;
        }

        station.MaxMixQuantity = Pick(Settings.MixingStationCapacity, defaults.Capacity);
        station.MixTimePerItem = Pick(Settings.MixTimePerItem, defaults.TimePerItem);
    }

    internal static void Apply(DryingRack rack)
    {
        if (rack == null)
            return;

        int key = rack.GetInstanceID();
        if (!RackDefaults.TryGetValue(key, out int capacity))
        {
            capacity = rack.ItemCapacity;
            RackDefaults[key] = capacity;
        }

        rack.ItemCapacity = Pick(Settings.DryingRackCapacity, capacity);
    }

    private static int Pick(int setting, int fallback) => Settings.Enabled && setting > 0 ? setting : fallback;
}

[HarmonyPatch(typeof(MixingStation), nameof(MixingStation.Awake))]
internal static class MixingStationAwakePatch
{
    private static void Postfix(MixingStation __instance) => Stations.Apply(__instance);
}

[HarmonyPatch(typeof(DryingRack), nameof(DryingRack.Awake))]
internal static class DryingRackAwakePatch
{
    private static void Postfix(DryingRack __instance) => Stations.Apply(__instance);
}
