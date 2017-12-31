namespace Camanis

open System
open System.IO
open System.Reflection

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Textures =
    { cursor : Texture2D
      aircraft : Texture2D }
    with
    static member load (graphics : GraphicsDevice) =
        let path = Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath |> Path.GetDirectoryName
        let fromFile fileName =
            use stream = new FileStream(Path.Combine(path, fileName), FileMode.Open)
            Texture2D.FromStream(graphics, stream)
        { cursor = fromFile "cursor.png"
          aircraft = fromFile "aircraft.png" }

type Model =
    { position : float32 * float32
      velocity : float32 * float32
      target : float32 * float32 }

module Render =
    let cursor (textures : Textures) (sb : SpriteBatch) (model : Model) =
        let (x, y) = model.target
        sb.Draw(textures.cursor, Vector2(x, y), Color.Red)

    let aircraft (textures : Textures) (sb : SpriteBatch) (model : Model) =
        let (x, y) = model.position
        sb.Draw(textures.aircraft, Vector2(x, y), Color.Red)

type CamanisGame() =
    inherit Game()

    let mutable model =
        { position = 0.0f, 3.0f
          velocity = 0.0f, 0.0f
          target = 0.0f, 0.0f }

    let mutable textures = None
    let mutable spriteBatch = None

    let render (textures : Textures) (sb : SpriteBatch) =
        sb.Begin()
        Render.aircraft textures sb model
        Render.cursor textures sb model
        sb.End()
        ()

    let updateModel model (gameTime : GameTime) =
        let (x, y) = model.position
        let (vx, vy) = model.velocity
        let (tx, ty) = model.target

        let x' = x + vx * float32 gameTime.ElapsedGameTime.TotalSeconds
        let y' = y + vy * float32 gameTime.ElapsedGameTime.TotalSeconds

        let position = Vector2(x', y')
        let target = Vector2(tx, ty)

        let heading = target - position
        let normalized = Vector2.Multiply(Vector2.Normalize heading, 500.0f)
        let velocity = if heading.Length() < normalized.Length() then heading else normalized

        let mouse = Mouse.GetState()

        { model with position = x', y'
                     velocity = velocity.X, velocity.Y
                     target = float32 mouse.X, float32 mouse.Y }

    override this.LoadContent() =
        let gd = this.GraphicsDevice
        spriteBatch <- Some(new SpriteBatch(gd))
        textures <- Some(Textures.load gd)

    override this.Draw(gameTime) =
        this.GraphicsDevice.Clear(Color.AliceBlue)
        match textures, spriteBatch with
        | Some ts, Some sb -> render ts sb
        | _ -> failwithf "Inconsistent state"
        base.Draw(gameTime)

    override __.Update(gameTime) =
        model <- updateModel model gameTime
        ()

    override __.Dispose(disposing) =
        base.Dispose(disposing)
        if disposing then
            Option.iter (fun x -> (x :> IDisposable).Dispose()) spriteBatch
