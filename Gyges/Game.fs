namespace Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics;

type Time =
    { Total: float32
      Delta: float32 }
    
type Config =
    { GameWidth: int
      GameHeight: int
      ScreenWidth: int
      ScreenHeight: int
      IsFullscreen: bool
      IsFixedTimeStep: bool
    }

type Canvas =
    { DrawTexture: Texture2D -> Vector2 -> unit
      DrawText: SpriteFont -> string -> Vector2 -> unit
      Clear: Color -> unit }
    
type Game<'World, 'Content, 'Input> =
    { LoadContent: ContentManager -> 'Content
      Init: unit -> 'World
      HandleInput: unit -> 'Input
      Update: 'Input -> Time -> 'World -> 'World
      Draw: Canvas -> 'Content -> 'World -> unit
    }
    
type GameState<'World, 'Content, 'Input>(config: Config, game: Game<_, _, _>) =
    inherit Game()

    let mutable renderTarget: RenderTarget2D = null
    let mutable onScreenRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable spriteBatch: SpriteBatch = null
    let mutable canvas: Canvas = Unchecked.defaultof<Canvas>
    
    let mutable model = Unchecked.defaultof<'World>
    let mutable content = Unchecked.defaultof<'Content>
    let mutable input = Unchecked.defaultof<'Input>

    let mutable fps = 0
    let mutable lastFpsUpdate = 0.
    let fpsUpdateInterval = 100.

    let drawTexture (texture: Texture2D) (pos: Vector2) =
        let width, height =
            texture.Width |> float32, texture.Height |> float32
            
        spriteBatch.Draw(texture, pos - Vector2(width, height)/2.0f, Color.White)
        
    let drawText (font: SpriteFont) (text: string) (pos: Vector2) =
        spriteBatch.DrawString
            ( spriteFont = font,
              text = text,
              position = pos,
              color = Color.White )
    
    let clearScreen (color: Color) =
        spriteBatch.GraphicsDevice.Clear(color)

    override this.Initialize() =
        this.Content.RootDirectory <- "Content"
        let gd = this.GraphicsDevice
        
        renderTarget <- new RenderTarget2D(gd, config.GameWidth, config.GameHeight)
        let screenWidth = gd.PresentationParameters.BackBufferWidth;
        let screenHeight = gd.PresentationParameters.BackBufferHeight;
        let scale = min (screenWidth/config.GameWidth) (screenHeight/config.GameHeight)
        let width = scale * config.GameWidth
        let height = scale * config.GameHeight
        onScreenRect <- Rectangle(screenWidth/2 - width/2, screenHeight/2 - height/2,
                                  width, height)
        
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        canvas <-
            { DrawTexture = drawTexture
              DrawText = drawText
              Clear = clearScreen }
                
        model <- game.Init()
        base.Initialize()

    override this.LoadContent() =
        content <- game.LoadContent (this.Content)
        base.LoadContent()

    override __.Update(gameTime) =
        let time =
            { Total = gameTime.TotalGameTime.TotalSeconds |> float32
              Delta = gameTime.ElapsedGameTime.TotalSeconds |> float32 }
        if gameTime.IsRunningSlowly then printfn "slow %A" gameTime.TotalGameTime
        
        input <- game.HandleInput()
        model <- game.Update input time model
        base.Update(gameTime)

    override this.Draw(gameTime) =
        let gd = this.GraphicsDevice
        gd.Clear Color.Black
        
        gd.SetRenderTarget(renderTarget)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null)
        spriteBatch.GraphicsDevice.Clear(Color.DarkBlue)
        game.Draw canvas content model
        spriteBatch.End()
        
        gd.SetRenderTarget(null)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null)
        spriteBatch.Draw(renderTarget, onScreenRect, Color.White)
        spriteBatch.End()
        base.Draw(gameTime)

module GameState =
    let create (config: Config) (game: Game<'World, 'Content, 'Input>) =
        let loop = new GameState<'World, 'Content, 'Input>(config, game)
        loop.IsFixedTimeStep <- config.IsFixedTimeStep
        
        let graphics = new GraphicsDeviceManager(loop)
        graphics.IsFullScreen <- config.IsFullscreen
        graphics.SynchronizeWithVerticalRetrace <- false
        graphics.PreferredBackBufferWidth <- config.ScreenWidth
        graphics.PreferredBackBufferHeight <- config.ScreenHeight
        graphics.ApplyChanges()
        
        loop
        
    let run (loop: GameState<'Model, 'Content, 'Input>) =
        loop.Run(); loop
