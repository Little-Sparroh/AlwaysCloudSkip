# Always Cloud Skip

A BepInEx mod for Mycopunk that enables Cloud Skip (double jump) at all times.

## Features

- **Always Cloud Skip** — Grants an air jump and sets air jump speed so Cloud Skip is available without the upgrade.
- **Config toggle** — Enable or disable the effect from the BepInEx config file.
- **Hot-reload** — Config changes apply while the game is running (no restart required).
- **Clean disable** — Restores your original air jump values when the toggle is turned off.
- **Local player only** — Does not affect other players in multiplayer.

## Dependencies

- Mycopunk
- [BepInEx](https://github.com/BepInEx/BepInEx) 5.4.2403 or compatible (e.g. BepInExPack_Mycopunk)

## Building

1. Clone this repository.
2. Open the solution in Visual Studio, Rider, or another C# IDE.
3. Build in Release mode to produce `AlwaysCloudSkip.dll`.

Or with the .NET CLI:

```bash
dotnet build --configuration Release
```

The project targets `netstandard2.1`.

## Installing

**Thunderstore (recommended)**

1. Install via the Thunderstore Mod Manager.
2. The mod is placed in the correct directory automatically.

**Manual**

1. Install BepInEx for Mycopunk.
2. Copy `AlwaysCloudSkip.dll` into `<Mycopunk Directory>/BepInEx/plugins/`.

## Usage

The mod loads automatically with BepInEx when the game starts. Confirm it loaded in the BepInEx log:

```text
AlwaysCloudSkip v1.0.1 loaded successfully.
```

## Configuration

Settings live in:

```text
<Mycopunk Directory>/BepInEx/config/sparroh.alwayscloudskip.cfg
```

| Setting           | Default | Description                                    |
|-------------------|---------|------------------------------------------------|
| Enable Cloud Skip | `true`  | Enables Cloud Skip (double jump) at all times. |

Changes are hot-reloaded while the game is open.

## Help

- **Mod not loading?** Confirm BepInEx is installed and check the BepInEx console/log for errors.
- **Cloud Skip not working?** Set `Enable Cloud Skip` to `true`. Edits to the config file are picked up without
  restarting.

## Authors

- Sparroh

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.
