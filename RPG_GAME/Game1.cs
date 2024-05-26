using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Apos.Shapes;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using RPG_GAME.Components;
using RPG_GAME.Core;
using TiledCS;

namespace RPG_GAME;

public class DrawRectCommand : List<(Rectangle, Color)> { }

public class DrawLineCommand : List<(Vector2, Vector2, Color)> { }

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;
    private ShapeBatch shapeBatch;
    public World world;
    public Player player;
    public Camera camera;
    public ImGuiRenderer renderer;

    private HashSet<Entity> _entities = new HashSet<Entity>();
    private DrawRectCommand _drawRectCommands = new();
    private DrawLineCommand _drawLineCommands = new();

    public Game1()
    {
        float limitFPS = 60;

        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.ApplyChanges();
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1f / limitFPS);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        world = new World();
        player = new Player(GraphicsDevice, Content);
        camera = new Camera(this);
        renderer = new ImGuiRenderer(this);
        renderer.RebuildFontAtlas();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        shapeBatch = new ShapeBatch(GraphicsDevice, Content);

        // TODO: use this.Content to load your game content here
        world.draw.Load(world, Content, GraphicsDevice);
        world.tool.AddWorldObjects(player, camera, world);
        world.tool.AddWorldBorderTile(player, camera, world);

        //player.draw.Load(player, Content, GraphicsDevice, _graphics);
        //player.tool.SetPositionPlayer(player, camera, 2400, 1920);

        Services.AddService(shapeBatch);
        Services.AddService(spriteBatch);
        Services.AddService(camera);
        Services.AddService(world);
        Services.AddService(_drawRectCommands);
        Services.AddService(_drawLineCommands);

        var myPlayer = new Entity(this, "main_player")
                .AddComponent(new AnimationRenderer("Player/move_animation/move_animation"))
                .AddComponent(new PlayerController())
                .AddComponent(new Rigidbody())
            ;

        _entities.Add(myPlayer);

        foreach (var item in _entities) item.Load();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
            Environment.Exit(0);
        }

        // TODO: Add your update logic here

        //player.control.Mainloop(player, camera, world);
        // camera.Mainloop(player);
        world.control.Mainloop(player, camera, world, gameTime);

        _drawRectCommands.Clear();
        _drawLineCommands.Clear();
        foreach (var item in _entities) item.Update();
        camera.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        world.draw.Mainloop(player, camera, world, spriteBatch);
        spriteBatch.End();

        foreach (var item in _entities) item.Draw();

        //player.ui.Mainloop(player);

        // draw gizmos
        shapeBatch.Begin(camera.World, null);
        foreach (var (rect, color) in _drawRectCommands)
        {
            shapeBatch.DrawRectangle(
                rect.Location.ToVector2(),
                rect.Size.ToVector2(),
                Color.Transparent,
                color);
        }

        foreach (var (a, b, color) in _drawLineCommands)
        {
            shapeBatch.DrawLine(a, b, 1f, color, Color.Black, 0f);
        }

        shapeBatch.End();
        // end draw gizmos

        renderer.BeginLayout(gameTime);
        foreach (var item in _entities) item.ImGui();
        renderer.EndLayout();

        //world.debugs.drawRectVisionAreaEnemy(world, camera);
        //world.debugs.DrawBorderTile(camera, world, _entities.First(x => x.Name == "main_player"));
        //world.debugs.draw_object_hitbox(player, camera, world, spriteBatch);
        //player.debugs.draw_hitbox(player, spriteBatch);
        //world.debugs.drawCollideRectEnemy(world, camera);

        base.Draw(gameTime);
    }
}