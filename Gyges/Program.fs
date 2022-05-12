open Gyges
open type SDL2.SDL

[<EntryPoint>]
let main args =
    use video = new Video(VideoScale.scalers[3])

    // Event loop
    let mutable quit = false
    let mutable event = SDL_Event()
    while not quit do
        while SDL_PollEvent(&event) > 0 do
            if event.``type`` = SDL_EventType.SDL_QUIT then
                quit <- true

    0
