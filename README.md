# Gyges [![Status Enfer][status-enfer]][andivionian-status-classifier] [![Appveyor Build][badge-appveyor]][build-appveyor]

Gyges (named after [one of the planets][gyges] in the [Tyrian][tyrian]
universe) will be a "Shoot 'em up" space shooter.

Will be submitted to the [8-bit SITC Game Jam][sitc-game-jam].

![Syges Demo][gyges-demo]

## Build

[.NET Core 2.2 SDK][netcore-sdk] is required to build the project.

```
$ dotnet build
```

## Run

```
$ dotnet run --project .\Gyges\Gyges.fsproj
```

[gyges]: https://www.youtube.com/watch?v=U2L7rcMN-Bw
[tyrian]: https://en.wikipedia.org/wiki/Tyrian_(video_game)
[sitc-game-jam]: https://itch.io/jam/8-bit-sitc-game-jam
[netcore-sdk]: https://www.microsoft.com/net/download/core#/sdk

[build-appveyor]: https://ci.appveyor.com/project/gsomix/sitc-game-jam/branch/master
[badge-appveyor]: https://ci.appveyor.com/api/projects/status/bg86bnt2ccnrkah5?svg=true

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier##status-enfer-
[status-enfer]: https://img.shields.io/badge/status-enfer-orange.svg

[gyges-demo]: gyges_demo.gif
