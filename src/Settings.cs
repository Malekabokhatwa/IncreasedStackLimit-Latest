using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using MelonLoader.Utils;

namespace IncreasedStackLimitLatest;

public enum LimitMode
{
    Fixed,
    Multiplier
}

internal static class Settings
{
    private const int MaxLimit = 100000;

    internal static readonly string FilePath = Path.Combine(MelonEnvironment.UserDataDirectory, "IncreasedStackLimit-Latest.cfg");

    private static MelonPreferences_Category _category;
    private static DateTime _lastWrite;
    private static float _nextCheck;

    private static MelonPreferences_Entry<bool> _enabled;
    private static MelonPreferences_Entry<LimitMode> _mode;
    private static MelonPreferences_Entry<int> _fixedLimit;
    private static MelonPreferences_Entry<float> _multiplier;
    private static MelonPreferences_Entry<bool> _allowLowering;
    private static MelonPreferences_Entry<bool> _affectUnstackable;
    private static MelonPreferences_Entry<string> _excludedIds;
    private static MelonPreferences_Entry<bool> _logItems;
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
            "Turn the mod on or off. Off restores every item's original stack limit.");
        _mode = cat.CreateEntry("Mode", LimitMode.Fixed, "Mode",
            "Fixed: every stackable item gets FixedLimit. Multiplier: each item's original limit is multiplied by Multiplier.");
        _fixedLimit = cat.CreateEntry("FixedLimit", 100, "Fixed limit",
            "Stack limit used in Fixed mode.");
        _multiplier = cat.CreateEntry("Multiplier", 5f, "Multiplier",
            "Multiplier used in Multiplier mode (e.g. 5 turns 20 into 100).");
        _allowLowering = cat.CreateEntry("AllowLowering", false, "Allow lowering",
            "If false, an item's limit is never set below its original value.");
        _affectUnstackable = cat.CreateEntry("AffectUnstackable", false, "Affect unstackable items",
            "If true, items that normally don't stack (limit 1) are changed too.");
        _excludedIds = cat.CreateEntry("ExcludedItemIds", "", "Excluded item IDs",
            "Comma-separated item IDs to leave untouched, e.g. \"baggie,jar\".");
        _logItems = cat.CreateEntry("LogItems", false, "Log every item",
            "Write each item's original and new stack limit to the MelonLoader log.");

        _mixingStationCapacity = cat.CreateEntry("MixingStationCapacity", 0, "Mixing station capacity",
            "Max items per mix. 0 = game default.");
        _mixTimePerItem = cat.CreateEntry("MixTimePerItem", 0, "Mixing time per item",
            "In-game minutes per mixed item. 0 = game default.");
        _dryingRackCapacity = cat.CreateEntry("DryingRackCapacity", 0, "Drying rack capacity",
            "Max items on a drying rack. 0 = game default.");

        // Own file so it can be watched and edited while the game runs.
        cat.SetFilePath(FilePath, autoload: true, printmsg: false);
        cat.SaveToFile(printmsg: false);
        _lastWrite = LastWrite();
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

    internal static int Compute(int original)
    {
        if (original <= 1 && !_affectUnstackable.Value)
            return original;

        int target = _mode.Value == LimitMode.Fixed
            ? _fixedLimit.Value
            : (int)Math.Round(original * (double)_multiplier.Value);

        target = Math.Clamp(target, 1, MaxLimit);
        if (!_allowLowering.Value && target < original)
            target = original;
        return target;
    }

    internal static bool IsExcluded(string id) => ExcludedIds().Contains(id);

    private static HashSet<string> ExcludedIds() =>
        _excludedIds.Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    internal static string Describe()
    {
        if (!_enabled.Value)
            return "disabled";
        string rule = _mode.Value == LimitMode.Fixed ? $"fixed {_fixedLimit.Value}" : $"x{_multiplier.Value}";
        return $"{rule}, lowering {(_allowLowering.Value ? "on" : "off")}, unstackables {(_affectUnstackable.Value ? "on" : "off")}";
    }
}
