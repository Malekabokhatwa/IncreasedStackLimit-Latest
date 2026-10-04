# IncreasedStackLimit-Latest

A small [MelonLoader](https://github.com/LavaGang/MelonLoader) mod for **Schedule I** that raises the stack limit of products and the supplies you use to make them. It's built for the current **0.4.7 beta (IL2CPP)**.

It started as a rewrite of froggy's *IncreasedStackLimit*, which no longer behaves correctly on 0.4.7. The main difference is *where* the limit gets changed:
- **This mod** edits each item definition's own stack limit.
- **The old mod** only overrode the value an item reports.

The game reads the definition directly for deliveries, the delivery app's "fits in vehicle" check, the shop cart and dead drops. Changing it there means every part of the game agrees on the new limit.

## What it changes
Only the items that make inventory management painful, matched by item **type**. That means new mixes, and items added by other mods that use the same types, are covered automatically.

| Type | Examples | Game default | Mod default |
|---|---|---|---|
| Products | weed, meth, cocaine, shrooms, every mix you create (loose or packaged) | 20 | 100 |
| Packaging | baggies, jars, bricks | 10–20 | 100 |
| Mix ingredients | Cuke, Banana, Mega Bean, Energy Drink… | 20 | 100 |
| Precursors | coca leaf, cocaine base, pseudo, liquid meth | 10–20 | 100 |
| Chemicals | acid, phosphorus, RDX | 10 | 100 |

Everything else keeps its normal limit on purpose:
- **Equipment and tools:** watering can, pliers and similar items stay at 1.
- **Placeables:** furniture, stations, lights, pots, grow tents and sprinklers.
- **Grow and shroom supplies:** soil, fertilizer, PGR, speed grow, seeds, spores, spawn and grain bags. These are used through equip-and-pour tasks, which don't expect big stacks.

A limit is never lowered. Items that normally don't stack are never touched.

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
| `ProductLimit` | `100` | Products. `0` keeps the game's value. |
| `PackagingLimit` | `100` | Packaging. `0` keeps the game's value. |
| `MixerLimit` | `100` | Mix ingredients. `0` keeps the game's value. |
| `PrecursorLimit` | `100` | Precursors. `0` keeps the game's value. |
| `ChemicalLimit` | `100` | Chemicals. `0` keeps the game's value. |
| `MixingStationCapacity` | `0` | Max items per mix. `0` keeps the game's value. |
| `MixTimePerItem` | `0` | Mixing minutes per item. `0` keeps the game's value. |
| `DryingRackCapacity` | `0` | Max items on a drying rack. `0` keeps the game's value. |
| `LogItems` | `false` | Log every item's type and old/new limit. |

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
