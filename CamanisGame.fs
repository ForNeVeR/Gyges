namespace Camanis

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type Model =
    { position : float * float
      velocity : float * float
      target : float * float }

type CamanisGame() =
    inherit Game()
    
    let mutable model = 
        { position = 0.0, 3.0
          velocity = 0.0, 0.0
          target = 0.0, 0.0 }

    //override this.LoadContent() =
    //    // let resourceDirectory = Path.Combine(resourceBasePath, "resources")
    //    // textures <- Textures.load resourceDirectory this.GraphicsDevice
    //    // loadCursor()
    //    ()
    
    override this.Draw(gameTime) =
        let gd = this.GraphicsDevice
        gd.Clear(Color.AliceBlue)
        base.Draw(gameTime)
    
    override __.Update(gameTime) =
        // mission <- GameLogic.update mission gameTime
        ()