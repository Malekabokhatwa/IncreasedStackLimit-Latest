using System.Collections.Generic;
using HarmonyLib;
using Il2CppScheduleOne;
using Il2CppScheduleOne.Core.Items.Framework;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.Product.Packaging;
using Object = UnityEngine.Object;

namespace IncreasedStackLimitLatest;

/// <summary>Item types the mod changes. Anything else keeps its game limit.</summary>
internal enum ItemGroup
{
    None,
    Product,
    Packaging,
    Mixer,
    Precursor,
    Chemical
}

/// <summary>
/// Rewrites <c>StackLimit</c> on item definitions. The definition field is what the game reads everywhere
/// (inventory, storage, deliveries, shop cart, dead drops), so changing it covers every code path.
/// Items are matched by definition type, so new mixes and modded items of the same type are included.
/// </summary>
internal static class StackLimits
{
    private const string PlainStorableType = "ScheduleOne.ItemFramework.StorableItemDefinition";

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

        var group = Classify(def, record.TypeName);
        int target = Settings.Enabled && group != ItemGroup.None ? Settings.Compute(group, record.Original) : record.Original;
        record.Applied = target;

        if (Settings.LogItems)
            Core.Log.Msg($"ITEM {id} | {group} | {record.TypeName} | {record.Original} -> {target}");

        if (def.StackLimit == target)
            return false;

        def.StackLimit = target;
        return true;
    }

    /// <summary>
    /// Most specific type first: ProductDefinition derives from PropertyItemDefinition.
    /// Soil, additives, seeds, spores and spawn, placeables, equipment, water containers, clothing and cash are
    /// deliberately not matched: they are used through equip/pour tasks or carry per-item state.
    /// </summary>
    private static ItemGroup Classify(ItemDefinition def, string typeName)
    {
        if (def.TryCast<ProductDefinition>() != null)
            return ItemGroup.Product;
        if (def.TryCast<PackagingDefinition>() != null)
            return ItemGroup.Packaging;
        if (def.TryCast<PropertyItemDefinition>() != null)
            return ItemGroup.Mixer;
        if (def.TryCast<QualityItemDefinition>() != null)
            return ItemGroup.Precursor;
        if (typeName == PlainStorableType && def.Category == EItemCategory.Ingredient)
            return ItemGroup.Chemical;
        return ItemGroup.None;
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
