# Gyges [![Status Enfer][status-enfer]][andivionian-status-classifier] [![Appveyor Build][badge-appveyor]][build-appveyor]

Gyges (named after [one of the planets][gyges] in the [Tyrian][tyrian]
universe) will be a "Shoot 'em up" space shooter.

![Gyges Demo][gyges-demo]

## Build

[.NET Core 2.2 SDK][netcore-sdk] is required to build the project.

```
$ dotnet build
```

## Run

```
$ dotnet run --project .\Gyges\Gyges.fsproj
```

## License

The game is licensed under the terms of the [Apache License][apache-license].

The font [Connection][connection] is licensed under under the [SIL Open Font
License, Version 1.1][sil-license].

[gyges]: https://www.youtube.com/watch?v=U2L7rcMN-Bw
[tyrian]: https://en.wikipedia.org/wiki/Tyrian_(video_game)
[netcore-sdk]: https://www.microsoft.com/net/download/core#/sdk

[build-appveyor]: https://ci.appveyor.com/project/ForNeVeR/gyges/branch/master
[badge-appveyor]: https://ci.appveyor.com/api/projects/status/tjl8vh406aojq35f/branch/master?svg=true

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier##status-enfer-
[status-enfer]: https://img.shields.io/badge/status-enfer-orange.svg

[apache-license]: LICENSE
[connection]: Gyges/Connection.otf
[gyges-demo]: gyges_demo.gif
[sil-license]: SIL%20Open%20Font%20License.txt
