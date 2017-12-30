module Camanis.Program

open Microsoft.Xna.Framework

[<EntryPoint>]
let main argv =
    use game = new CamanisGame()
    use graphics = new GraphicsDeviceManager(game)
    
    game.Run()
    0
