﻿using System;
using System.Diagnostics;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledCS;

namespace RPG_GAME;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;
    public World world;
    public Player player;
    public Camera camera;

    public Game1()
    {
        double limitFPS = 60; 
        
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = true;
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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        world.draw.Load(world, Content, GraphicsDevice);
        world.tool.AddWorldObjects(player, camera, world);
        world.tool.AddWorldBorderTile(player, camera, world);
        
        player.draw.load(player, Content, GraphicsDevice, _graphics);
        player.tool.set_position_player(player, camera, 2400, 1920);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        player.control.mainloop(player, camera, world);
        camera.mainloop(player);
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

        player.ui.mainloop(player);
        //world.debugs.drawRectVisionAreaEnemy(world, camera);
        //world.debugs.drawBorderTile(camera, world);
        //world.debugs.draw_object_hitbox(player, camera, world, spriteBatch);
        //player.debugs.draw_hitbox(player, spriteBatch);
        //world.debugs.drawCollideRectEnemy(world, camera);
        

        base.Draw(gameTime);
    }
}