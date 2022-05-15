open Gyges
open Silk.NET.SDL

[<EntryPoint>]
let main args =
    let api = Sdl.GetApi()
    use video = new Video(api, VideoScale.scalers[3])

    // Event loop
    let mutable quit = false
    let mutable event = Event()
    while not quit do
        while api.PollEvent(&event) > 0 do
            if event.Type = uint32 EventType.Quit then
                quit <- true

    0
