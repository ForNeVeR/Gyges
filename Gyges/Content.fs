namespace Gyges

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Content =
    { Ship: Texture2D
      Bullet: Texture2D }

module Content =    
    let load (contentManager: ContentManager): Content =
        { Ship = contentManager.Load("ship")
          Bullet = contentManager.Load("bullet") }

