using System.Collections.Generic;
using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using Object = UnityEngine.Object;

namespace IncreasedStackLimitLatest;

/// <summary>
/// Rewrites <c>StackLimit</c> on item definitions. The definition field is what the game reads everywhere
/// (inventory, storage, deliveries, shop cart, dead drops), so changing it covers every code path.
/// </summary>
internal static class StackLimits
{
    private sealed class Record
    {
        public int Original;
        public int Applied;
        public string TypeName;
    }

    private static readonly Dictionary<string, Record> Records = new();

    internal static void ApplyAll()
    {
        var registry = Object.FindObjectOfType<Registry>();
        if (registry == null)
            return;

        var items = registry.GetAllItems();
        int changed = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (Apply(items[i]))
                changed++;
        }

        if (changed > 0)
            Core.Log.Msg($"Updated stack limits on {changed} of {items.Count} items ({Settings.Describe()}).");
    }

    internal static bool Apply(ItemDefinition def)
    {
        if (def == null)
            return false;

        string id = def.ID;
        if (string.IsNullOrEmpty(id))
            return false;

        if (!Records.TryGetValue(id, out var record))
        {
            string typeName = def.GetIl2CppType().FullName;
            record = new Record { Original = OriginalFor(def.StackLimit, typeName), TypeName = typeName };
            Records[id] = record;
        }

        int target = Settings.Enabled && !Settings.IsExcluded(id) ? Settings.Compute(record.Original) : record.Original;
        record.Applied = target;

        if (def.StackLimit == target)
            return false;

        if (Settings.LogItems)
            Core.Log.Msg($"{id}: {record.Original} -> {target}");
        def.StackLimit = target;
        return true;
    }

    /// <summary>
    /// New product mixes are created with Instantiate(DefaultWeed) and similar, which copies a definition we may
    /// already have changed. If the value matches what we applied to a same-typed definition, use that
    /// definition's original instead, so limits don't compound.
    /// </summary>
    private static int OriginalFor(int current, string typeName)
    {
        foreach (var r in Records.Values)
        {
            if (r.TypeName == typeName && r.Applied == current && r.Applied != r.Original)
                return r.Original;
        }
        return current;
    }
}

[HarmonyPatch(typeof(Registry), nameof(Registry.AddToRegistry))]
internal static class RegistryAddToRegistryPatch
{
    private static void Postfix(ItemDefinition item) => StackLimits.Apply(item);
}
