using System;
using System.Diagnostics;
using Apos.Shapes;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using TiledCS;

namespace RPG_GAME;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;
    public World world;
    public Player player;
    public Camera camera;
    public ImGuiRenderer renderer;
    
    public Game1()
    {
        
        double limitFPS = 60;
        
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / limitFPS);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        world = new World();
        player = new Player(GraphicsDevice, Content);
        camera = new Camera();
        renderer = new ImGuiRenderer(this);
        renderer.RebuildFontAtlas();
        
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        world.draw.Load(world, Content, GraphicsDevice);
        world.tool.AddWorldObjects(player, camera, world);
        world.tool.AddWorldBorderTile(player, camera, world);
        
        player.draw.Load(player, Content, GraphicsDevice, _graphics);
        player.tool.SetPositionPlayer(player, camera, 2400, 1920);
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

        player.control.Mainloop(player, camera, world);
        camera.Mainloop(player);
        world.control.Mainloop(player, camera, world, gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        world.draw.Mainloop(player, camera, world, spriteBatch);
        spriteBatch.End();

        player.ui.Mainloop(player);
        
        renderer.BeginLayout(gameTime);
        ImGui.ShowDemoWindow();
        renderer.EndLayout();

        //world.debugs.drawRectVisionAreaEnemy(world, camera);
        //world.debugs.drawBorderTile(camera, world);
        //world.debugs.draw_object_hitbox(player, camera, world, spriteBatch);
        //player.debugs.draw_hitbox(player, spriteBatch);
        //world.debugs.drawCollideRectEnemy(world, camera);

        base.Draw(gameTime);
    }
}