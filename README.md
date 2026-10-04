# IncreasedStackLimit-Latest

A small [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for **Schedule I** that raises item stack limits. It's built for the current **0.4.7 beta (IL2CPP)**.

It started as a rewrite of froggy's *IncreasedStackLimit*, which no longer behaves correctly on 0.4.7. The main difference is *where* the limit gets changed:
- **This mod** edits each item definition's own stack limit.
- **The old mod** only overrode the value an item reports.

The game reads the definition directly for deliveries, the delivery app's "fits in vehicle" check, the shop cart and dead drops. Changing it there means every part of the game agrees on the new limit.

## Features
- Raise every stackable item to a fixed limit (default 100) or multiply its original limit.
- Never lowers a limit and leaves non-stackable items alone, unless you ask it to.
- Works with new product mixes created during play.
- Optional settings for mixing station capacity, mixing time per item and drying rack capacity. These are off by default.
- All settings live in one commented file, `UserData/IncreasedStackLimit-Latest.cfg`. Edits are picked up while the game is running, with no restart.

## Requirements
- Schedule I on the default or `beta` branch (IL2CPP). Tested on **0.4.7f9**.
- MelonLoader **0.7.3** or newer. Start the game with MelonLoader once before adding mods.

The Mono (`alternate`) branch isn't supported yet.

## Install
1. Download `IncreasedStackLimit-Latest.dll` from [Releases](https://github.com/Malekabokhatwa/IncreasedStackLimit-Latest/releases).
2. Put it in `Schedule I/Mods/`.
3. Start the game. The settings file `Schedule I/UserData/IncreasedStackLimit-Latest.cfg` is created on the first launch.

If you used the original IncreasedStackLimit, remove it first. Both mods change the same thing.

## Settings
Settings live in `UserData/IncreasedStackLimit-Latest.cfg`. Save the file and the change applies within a second or so.

| Setting | Default | What it does |
|---|---|---|
| `Enabled` | `true` | Off restores every original stack limit. |
| `Mode` | `"Fixed"` | `"Fixed"` uses `FixedLimit`. `"Multiplier"` multiplies each item's original limit. |
| `FixedLimit` | `100` | Stack limit in Fixed mode. |
| `Multiplier` | `5.0` | Multiplier in Multiplier mode. For example, 20 becomes 100. |
| `AllowLowering` | `false` | Allow limits below an item's original value. |
| `AffectUnstackable` | `false` | Also change items that normally don't stack (limit 1). |
| `ExcludedItemIds` | `""` | Comma-separated item IDs to leave alone. |
| `LogItems` | `false` | Log every item's old and new limit. |
| `MixingStationCapacity` | `0` | Max items per mix. `0` keeps the game's value. |
| `MixTimePerItem` | `0` | Mixing minutes per item. `0` keeps the game's value. |
| `DryingRackCapacity` | `0` | Max items on a drying rack. `0` keeps the game's value. |

## Good to know
- **Multiplayer:** everyone in the lobby should run the mod with the same settings. Stack limits are checked on both the host and the clients.
- **Removing the mod:** the game caps oversized stacks when it loads them, so stacks above the normal limit get cut down if you uninstall. Split big stacks before removing the mod.
- **Station UI:** if you change station settings mid-game, the station's start threshold slider uses the new maximum after you reload the save.

## Building
You need the .NET SDK (6.0 or newer) and an IL2CPP copy of the game that has run once with MelonLoader. The project references `MelonLoader/net6` and `MelonLoader/Il2CppAssemblies` from that copy.

```sh
dotnet build src -c Release -p:GameDir="/path/to/Schedule I"
# add -p:DeployToGame=true to copy the DLL into the game's Mods folder
```

## Credits
- froggy, for the original IncreasedStackLimit that this mod is based on.
- The [LavaGang](https://github.com/LavaGang) team for MelonLoader, and the Schedule I modding community for its [docs](https://s1modding.github.io/docs/moddevs/).
- Written with the help of an AI coding assistant (Claude).

No game files or decompiled game code are included in this repository.

## License
[MIT](LICENSE)
