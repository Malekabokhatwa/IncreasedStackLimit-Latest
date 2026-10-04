# IncreasedStackLimit-Latest

A small [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for **Schedule I** that raises item stack limits to 250. It's built for the current **0.4.7 beta (IL2CPP)**.

It started as a rewrite of froggy's *IncreasedStackLimit*, which no longer behaves correctly on 0.4.7. The main difference is *where* the limit gets changed:
- **This mod** edits each item definition's own stack limit.
- **The old mod** only overrode the value an item reports.

The game reads the definition directly for deliveries, the delivery app's "fits in vehicle" check, the shop cart and dead drops. Changing it there means every part of the game agrees on the new limit.

## What it changes
Every item that stacks gets a limit of 250. Items are grouped by **type**, and each group has its own setting. New mixes, and items added by other mods that use the same types, are covered automatically.

| Group | Examples | Game default |
|---|---|---|
| Products | weed, meth, cocaine, shrooms, every mix you create (loose or packaged) | 20 |
| Packaging | baggies, jars, bricks | 10–20 |
| Mix ingredients | Cuke, Banana, Mega Bean, Energy Drink… | 20 |
| Precursors | coca leaf, cocaine base, pseudo, liquid meth | 10–20 |
| Seeds | weed and coca seeds | 10 |
| Soil | soil, long life soils, mushroom substrate | 10 |
| Grow additives | fertilizer, PGR, Speed Grow | 10 |
| Shroom supplies | spore syringes, shroom spawn | 10 |
| Placeables | furniture, stations, lights, storage, pots, grow tents | 10–20 |
| Everything else | acid, phosphorus, RDX, grain bags, trash bags, spray paint… | 10 |

Never changed:
- **Guns and melee weapons:** detected from the item's equipped weapon.
- **Ammo:** whatever a gun loads, for example shotgun shells. Raising it causes a bug. It's detected from the guns themselves, so ammo from modded guns is skipped too.
- **Items that don't stack (limit 1):** tools like the watering can and pliers, clothing, skateboards and cash.

A limit is never lowered.

Optional, off by default: mixing station capacity, mixing time per item, and drying rack capacity.

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
Settings are in `UserData/IncreasedStackLimit-Latest.cfg`, with a comment on each one. Save the file while the game is running and the change applies within a second or so.

| Setting | Default | What it does |
|---|---|---|
| `Enabled` | `true` | Off restores every original stack limit. |
| `ProductLimit` | `250` | Products. |
| `PackagingLimit` | `250` | Packaging. |
| `MixerLimit` | `250` | Mix ingredients. |
| `PrecursorLimit` | `250` | Precursors. |
| `SeedLimit` | `250` | Seeds. |
| `SoilLimit` | `250` | Soil. |
| `AdditiveLimit` | `250` | Grow additives. |
| `ShroomSupplyLimit` | `250` | Shroom supplies. |
| `PlaceableLimit` | `250` | Placeables. |
| `OtherLimit` | `250` | Everything else. |
| `MixingStationCapacity` | `0` | Max items per mix. `0` keeps the game's value. |
| `MixTimePerItem` | `0` | Mixing minutes per item. `0` keeps the game's value. |
| `DryingRackCapacity` | `0` | Max items on a drying rack. `0` keeps the game's value. |
| `LogItems` | `false` | Log every item's type and old/new limit. |

Set a group limit to `0` to keep the game's values for that group.

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
