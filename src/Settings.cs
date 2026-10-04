using System;
using System.IO;
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
    private static MelonPreferences_Entry<int> _productLimit;
    private static MelonPreferences_Entry<int> _packagingLimit;
    private static MelonPreferences_Entry<int> _mixerLimit;
    private static MelonPreferences_Entry<int> _precursorLimit;
    private static MelonPreferences_Entry<int> _chemicalLimit;
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

        _productLimit = cat.CreateEntry("ProductLimit", 100, "Products",
            "Weed, meth, cocaine, shrooms and every mix you create, loose or packaged (game default 20). 0 = game default.");
        _packagingLimit = cat.CreateEntry("PackagingLimit", 100, "Packaging",
            "Baggies, jars and bricks (game default 10-20). 0 = game default.");
        _mixerLimit = cat.CreateEntry("MixerLimit", 100, "Mix ingredients",
            "Mixing ingredients such as Cuke, Banana or Mega Bean (game default 20). 0 = game default.");
        _precursorLimit = cat.CreateEntry("PrecursorLimit", 100, "Precursors",
            "Quality-graded ingredients: coca leaf, cocaine base, pseudo, liquid meth (game default 10-20). 0 = game default.");
        _chemicalLimit = cat.CreateEntry("ChemicalLimit", 100, "Chemicals",
            "Plain ingredients such as acid, phosphorus and RDX (game default 10). 0 = game default.");

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

    internal static int LimitFor(ItemGroup group) => group switch
    {
        ItemGroup.Product => _productLimit.Value,
        ItemGroup.Packaging => _packagingLimit.Value,
        ItemGroup.Mixer => _mixerLimit.Value,
        ItemGroup.Precursor => _precursorLimit.Value,
        ItemGroup.Chemical => _chemicalLimit.Value,
        _ => 0
    };

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
        return $"products {_productLimit.Value}, packaging {_packagingLimit.Value}, mixers {_mixerLimit.Value}, " +
               $"precursors {_precursorLimit.Value}, chemicals {_chemicalLimit.Value}";
    }
}
