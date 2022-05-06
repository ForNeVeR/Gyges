# Gyges [![Status Enfer][status-enfer]][andivionian-status-classifier]

Gyges (named after [one of the planets][gyges] in the [Tyrian][tyrian]
universe) will be a "Shoot 'em up" space shooter.

![Gyges Demo][gyges-demo]

## Build

For now, only building on Windows is supported.

Install [.NET SDK][dotnet] 6.0 and .NET Core 3.1 runtime (to run MonoGame content builder task) for your platform, then run:

```
$ dotnet build
```

## Run

```
$ dotnet run -c Release --project .\Gyges\Gyges.fsproj
```

## License

The game is licensed under the terms of the [Apache License][apache-license].

The font [Connection][connection] is licensed under under the [SIL Open Font
License, Version 1.1][sil-license].

[gyges]: https://www.youtube.com/watch?v=U2L7rcMN-Bw
[tyrian]: https://en.wikipedia.org/wiki/Tyrian_(video_game)
[dotnet]: https://dotnet.microsoft.com/download

[andivionian-status-classifier]: https://github.com/ForNeVeR/andivionian-status-classifier##status-enfer-
[status-enfer]: https://img.shields.io/badge/status-enfer-orange.svg

[apache-license]: LICENSE
[connection]: Gyges/Connection.otf
[gyges-demo]: gyges_demo.gif
[sil-license]: SIL%20Open%20Font%20License.txt
