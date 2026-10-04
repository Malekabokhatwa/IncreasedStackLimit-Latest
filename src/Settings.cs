using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using MelonLoader.Utils;

namespace IncreasedStackLimitLatest;

internal static class Settings
{
    private const int MaxLimit = 100000;

    internal static readonly string FilePath = Path.Combine(MelonEnvironment.UserDataDirectory, "IncreasedStackLimit-Latest.cfg");

    private static MelonPreferences_Category _category;
    private static DateTime _lastWrite;
    private static float _nextCheck;

    private static MelonPreferences_Entry<bool> _enabled;
    private static MelonPreferences_Entry<bool> _logItems;
    private static readonly Dictionary<ItemGroup, MelonPreferences_Entry<int>> _limits = new();
    private static MelonPreferences_Entry<int> _mixingStationCapacity;
    private static MelonPreferences_Entry<int> _mixTimePerItem;
    private static MelonPreferences_Entry<int> _dryingRackCapacity;

    internal static bool Enabled => _enabled.Value;
    internal static bool LogItems => _logItems.Value;
    internal static int MixingStationCapacity => _mixingStationCapacity.Value;
    internal static int MixTimePerItem => _mixTimePerItem.Value;
    internal static int DryingRackCapacity => _dryingRackCapacity.Value;

    internal static void Init()
    {
        var cat = _category = MelonPreferences.CreateCategory("IncreasedStackLimitLatest", "IncreasedStackLimit-Latest");

        _enabled = cat.CreateEntry("Enabled", true, "Enabled",
            "Turn the mod on or off. Off restores every original stack limit.");

        AddLimit(cat, ItemGroup.Product, "ProductLimit", "Products",
            "Weed, meth, cocaine, shrooms and every mix you create, loose or packaged (game default 20).");
        AddLimit(cat, ItemGroup.Packaging, "PackagingLimit", "Packaging", "Baggies, jars and bricks (game default 10-20).");
        AddLimit(cat, ItemGroup.Mixer, "MixerLimit", "Mix ingredients", "Cuke, Banana, Mega Bean and the other mixers (game default 20).");
        AddLimit(cat, ItemGroup.Precursor, "PrecursorLimit", "Precursors",
            "Quality-graded ingredients: coca leaf, cocaine base, pseudo, liquid meth (game default 10-20).");
        AddLimit(cat, ItemGroup.Seed, "SeedLimit", "Seeds", "Weed and coca seeds (game default 10).");
        AddLimit(cat, ItemGroup.Soil, "SoilLimit", "Soil", "Soil, long life soils and mushroom substrate (game default 10).");
        AddLimit(cat, ItemGroup.Additive, "AdditiveLimit", "Grow additives", "Fertilizer, PGR, Speed Grow (game default 10).");
        AddLimit(cat, ItemGroup.ShroomSupply, "ShroomSupplyLimit", "Shroom supplies", "Spore syringes and shroom spawn (game default 10).");
        AddLimit(cat, ItemGroup.Placeable, "PlaceableLimit", "Placeables",
            "Furniture, decorations, stations, lights, storage, pots, grow tents, sprinklers (game default 10-20).");
        AddLimit(cat, ItemGroup.Other, "OtherLimit", "Everything else",
            "Any other stackable item: acid, phosphorus, RDX, grain bags, trash bags, spray paint... (game default 10). " +
            "Guns, melee weapons, their ammo and items that don't stack are never changed.");

        _mixingStationCapacity = cat.CreateEntry("MixingStationCapacity", 0, "Mixing station capacity",
            "Max items per mix. 0 = game default.");
        _mixTimePerItem = cat.CreateEntry("MixTimePerItem", 0, "Mixing time per item",
            "In-game minutes per mixed item. 0 = game default.");
        _dryingRackCapacity = cat.CreateEntry("DryingRackCapacity", 0, "Drying rack capacity",
            "Max items on a drying rack. 0 = game default.");

        _logItems = cat.CreateEntry("LogItems", false, "Log every item",
            "Write each item's type, original and new stack limit to the MelonLoader log.");

        // Own file so it can be watched and edited while the game runs.
        cat.SetFilePath(FilePath, autoload: true, printmsg: false);
        cat.SaveToFile(printmsg: false);
        _lastWrite = LastWrite();
    }

    private static void AddLimit(MelonPreferences_Category cat, ItemGroup group, string key, string name, string description) =>
        _limits[group] = cat.CreateEntry(key, 250, name, description + " 0 = game default.");

    internal static int LimitFor(ItemGroup group) => _limits.TryGetValue(group, out var entry) ? entry.Value : 0;

    /// <summary>The group's limit, but never below the original and never for items that don't stack.</summary>
    internal static int Compute(ItemGroup group, int original)
    {
        int limit = Math.Min(LimitFor(group), MaxLimit);
        if (original <= 1 || limit <= original)
            return original;
        return limit;
    }

    /// <summary>Reloads the settings file if it changed on disk. Returns true when settings were reloaded.</summary>
    internal static bool ReloadIfChanged(float now)
    {
        if (now < _nextCheck)
            return false;
        _nextCheck = now + 1f;

        var write = LastWrite();
        if (write == _lastWrite)
            return false;

        _lastWrite = write;
        _category.LoadFromFile(printmsg: false);
        return true;
    }

    private static DateTime LastWrite() => File.Exists(FilePath) ? File.GetLastWriteTimeUtc(FilePath) : DateTime.MinValue;

    internal static string Describe()
    {
        if (!_enabled.Value)
            return "disabled";
        return string.Join(", ", _limits.Select(kv => $"{kv.Key} {kv.Value.Value}"));
    }
}
