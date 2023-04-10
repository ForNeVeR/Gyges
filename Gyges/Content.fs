namespace Gyges

open Raylib_CsLo

type Content =
    { Ship: Texture
      Bullet: Texture
      Enemy: Texture
      ScoreFont: Font }

module Content =    
    let load (): Content =
        { Ship        = Raylib.LoadTexture("Gyges.Resources/ship.png")
          Bullet      = Raylib.LoadTexture("Gyges.Resources/bullet.png")
          Enemy       = Raylib.LoadTexture("Gyges.Resources/enemy.png")
          ScoreFont   = Raylib.LoadFontEx("Gyges.Resources/OpenSans-Regular.ttf", 32, 250) }
