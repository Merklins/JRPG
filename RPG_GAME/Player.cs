using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Apos.Shapes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG_GAME.Enemy;
using RPG_GAME.MapObjects;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace RPG_GAME;


public class Player
{
    public Draw draw;
    public Control control;
    public UI ui;
    public Debugs debugs;
    public Tool tool;
    public Fight fight;

    public GraphicsDevice device;
    public ContentManager content;

    public Player(GraphicsDevice device, ContentManager content)
    {
        draw = new Draw();
        control = new Control();
        ui = new UI();
        debugs = new Debugs(device, content);
        tool = new Tool();
        fight = new Fight();
    }

    public class Draw
    {
        public void Mainloop(Player player, SpriteBatch surface)
        {
            AnimationMove(player);
            DrawSprite(player, surface);
        }

        public int animationFrame = 0;
        
        public List<Texture2D> walkDown = new List<Texture2D>();
        public List<Texture2D> walkUp = new List<Texture2D>();
        public List<Texture2D> walkRight = new List<Texture2D>();
        public List<Texture2D> walkLeft = new List<Texture2D>();

        public Texture2D sprite;
        public RectangleF rect;
        public RectangleF rectCollide;
        public RectangleF rectCollideConst;
        private Vector2 positionScreen;
        public ShapeBatch ShapeBatch;

        public void LoadSpritePlayer(Player player, ContentManager content)
        {
            List<string> direction_name = new List<string>()
            {
                "left", "right", "up", "down"
            };

            foreach (string direction in direction_name) {
                for (int i = 1; i <= 3; i++) {
                    switch (direction) {
                        case "left": {
                            walkLeft.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "right": {
                            walkRight.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "up": {
                            walkUp.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "down": {
                            walkDown.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                    }
                }
            }
        }

        public void LoadDrawPlayer(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
        {
            float xOffset = -0;
            float yOffset = -140;
            float widthOffset = 0;
            float heightOffset = 70;
            
            player.content = content;
            player.device = device;
            this.ShapeBatch = new ShapeBatch(device, content);
            sprite = content.Load<Texture2D>("Player\\Ninja_down1");
            positionScreen = new Vector2((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2);
            rect = new RectangleF((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2, sprite.Width, sprite.Height);
            rectCollide = new RectangleF(
                (manager.PreferredBackBufferWidth - sprite.Width - xOffset) / 2 + 5,
                (manager.PreferredBackBufferHeight - sprite.Height - yOffset) / 2,
                sprite.Width - widthOffset - 10,
                sprite.Height - heightOffset
            );
            rectCollideConst = new RectangleF(
                (manager.PreferredBackBufferWidth - sprite.Width - xOffset) / 2 + 5,
                (manager.PreferredBackBufferHeight - sprite.Height - yOffset) / 2,
                sprite.Width - widthOffset - 10,
                sprite.Height - heightOffset
            );
        }

        public void Load(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
        {
            player.draw.LoadDrawPlayer(player, content, device, manager);
            player.draw.LoadSpritePlayer(player, content);
            player.ui.Load(content, device);
        }

        public void DrawSprite(Player player, SpriteBatch surface)
        {
            surface.Draw(sprite, positionScreen, Color.White);
        }

        public void AnimationMove(Player player)
        {
            int count_animation_frame = 16;
            if (player.control.sprinting) count_animation_frame = 10;

            if (animationFrame + 1 >= count_animation_frame * walkDown.Count) animationFrame = 0;
            
            if (player.control.up)
            {
                if (!player.control.move)
                {
                    sprite = walkUp[0];
                    animationFrame = 0;
                }

                else
                {
                    sprite = walkUp[animationFrame / count_animation_frame];
                    animationFrame++;
                }
                player.control.oldDirect = "up";
            }
        
            else if (player.control.down)
            {
                if (!player.control.move)
                {
                    sprite = walkDown[0];
                    animationFrame = 0;
                }
                else
                {
                    sprite = walkDown[animationFrame / count_animation_frame];
                    animationFrame++;
                }
                
                player.control.oldDirect = "down";
            }
        
            else if (player.control.left)
            {
                if (!player.control.move)
                {
                    sprite = walkLeft[0];
                    animationFrame = 0;
                }
                else
                {
                    sprite = walkLeft[animationFrame / count_animation_frame];
                    animationFrame++;
                }
                
                player.control.oldDirect = "left";
            }
        
            else if (player.control.right)
            {
                if (!player.control.move)
                {
                    sprite = walkRight[0];
                    animationFrame = 0;
                }
                else
                {
                    sprite = walkRight[animationFrame / count_animation_frame];
                    animationFrame++;
                }
                
                player.control.oldDirect = "right";
            }
            
            else {
                switch (player.control.oldDirect) {
                    case "up": {
                        sprite = walkUp[0];
                        break;
                    }
                    
                    case "down": {
                        sprite = walkDown[0];
                        break;
                    }

                    case "right": {
                        sprite = walkRight[0];
                        break;
                    }

                    case "left": {
                        sprite = walkLeft[0];
                        break;
                    }
                }
            }
        }
    }

    public class Control
    {
        public bool noCollisionMode = false;
        
        public int sprintPointMax = 3600;
        public int sprintPoint = 3600;
        public int sprintPointRegeneration = 2;
        public int sprintPointCost = 5;
        
        public (float x, float y) vector = (0, 0);
        public (string direction, int value) undoVector = ("none", 0);
        public float playerSpeed = 5.62f;

        public bool up = false;
        public bool down = false;
        public bool right = false;
        public bool left = false;
        public string oldDirect = "none";
        public bool sprinting = false;
        public bool move = false;

        public Dictionary<Keys, bool> pressedKeys = new Dictionary<Keys, bool>();

        public void Mainloop(Player player, Camera camera, World world)
        {
            KeyMove(player);
            KeySprint(player);
            KeyUse(player);

            MovePosition(player, world);
        }

        public bool BaseLogicPressButton(Keys key)
        {
            bool result = false;
            
            if (!pressedKeys.ContainsKey(key))
                pressedKeys.Add(key, false);

            if (Keyboard.GetState().IsKeyDown(key))
            {
                if (!pressedKeys[key]) result = true;
                pressedKeys[key] = true;
            }

            else if (Keyboard.GetState().IsKeyUp(key))
            {
                pressedKeys[key] = false;
            }

            return result;
        }

        public void KeyUse(Player player)
        {
            if (BaseLogicPressButton(Keys.Q))
            {
                if (BaseTiledObject.useBaseWind)
                    BaseTiledObject.useBaseWind = false;
                else
                    BaseTiledObject.useBaseWind = true;
            }

            if (BaseLogicPressButton(Keys.E))
            {
                if (player.control.noCollisionMode)
                    player.control.noCollisionMode = false;
                else
                    player.control.noCollisionMode = true;
            }
        }

        public void KeyMove(Player player)
        {
            vector.x = 0; vector.y = 0;
            up = false;
            down = false;
            left = false;
            right = false;
            
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                vector.x -= playerSpeed;
                left = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                vector.x += playerSpeed;
                right = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                vector.y -= playerSpeed;
                up = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                vector.y += playerSpeed;
                down = true;
                move = true;
            }

            else
            {
                up = false;
                down = false;
                right = false;
                left = false;
                move = false;
            }
        }

        public void KeySprint(Player player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (vector.x == 0 && vector.y == 0)
                {
                    player.tool.RestoreSprintPoint(player);
                    sprinting = false;
                }

                if (sprintPoint >= sprintPointCost && (vector.x != 0 || vector.y != 0))
                {
                    vector.x = vector.x * 1.5f;
                    vector.y = vector.y * 1.5f;
                    sprintPoint -= sprintPointCost;
                    sprinting = true;
                }

                else if (sprintPoint < sprintPointCost)
                {
                    sprintPoint = 0;
                    sprinting = false;
                }
            }

            else if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                player.tool.RestoreSprintPoint(player);
                sprinting = false;
            }
        }

        public void MovePosition(Player player, World world)
        {
            List<(int pixel, string direction)> vectors = new List<(int pixel, string direction)>();

            foreach (RectangleF rectTile in world.borderTile) {
                RectangleF rectPlayer = player.draw.rectCollide;
                if (vector.x > 0) {
                    for (int i = 1; i < vector.x+1; i++) {
                        rectPlayer.X += 1;

                        if (rectTile.Intersects(rectPlayer)) {
                            rectPlayer.X -= 1;
                            vectors.Add((i - 1, "x"));
                            break;
                        }
                    }
                }
                
                else if (vector.x < 0) {
                    for (int i = 1; i < -vector.x+1; i++) {
                        rectPlayer.X -= 1;
                    
                        if (rectTile.Intersects(rectPlayer)) {
                            rectPlayer.X += 1;
                            vectors.Add( (-(i - 1), "x") );
                            break;
                        }
                    }
                }
            
                else if (vector.y > 0) {
                    for (int i = 1; i < vector.y+1; i++) {
                        rectPlayer.Y += 1;
                    
                        if (rectTile.Intersects(rectPlayer)) {
                            rectPlayer.Y -= 1;
                            vectors.Add((i - 1, "y"));
                            break;
                        }
                    }
                }
            
                else if (vector.y < 0) {
                    for (int i = 1; i < -vector.y+1; i++) {
                        rectPlayer.Y -= 1;
                    
                        if (rectTile.Intersects(rectPlayer)) {
                            rectPlayer.Y += 1;
                            vectors.Add(( -(i - 1), "y" ));
                            break;
                        }
                    }
                }
            }
            
            foreach (var obj in world.allObjectsAndY.Keys) {
                if (!(obj is BaseTiledObject)) continue;

                var rectPlayer = player.draw.rectCollide;
                var rectObj = world.tool.GetCollideRect((BaseTiledObject)obj);

                if (vector.x > 0) {
                    for (int i = 1; i < vector.x+1; i++) {
                        rectPlayer.X += 1;

                        if (rectObj.Intersects(rectPlayer)) {
                            rectPlayer.X -= 1;
                            vectors.Add((i - 1, "x"));
                            break;
                        }
                    }
                }
                
                else if (vector.x < 0) {
                    for (int i = 1; i < -vector.x+1; i++) {
                        rectPlayer.X -= 1;
                    
                        if (rectObj.Intersects(rectPlayer)) {
                            rectPlayer.X += 1;
                            vectors.Add( (-(i - 1), "x") );
                            break;
                        }
                    }
                }
            
                else if (vector.y > 0) {
                    for (int i = 1; i < vector.y+1; i++) {
                        rectPlayer.Y += 1;
                    
                        if (rectObj.Intersects(rectPlayer)) {
                            rectPlayer.Y -= 1;
                            vectors.Add((i - 1, "y"));
                            break;
                        }
                    }
                }
                
                else if (vector.y < 0) {
                    for (int i = 1; i < -vector.y+1; i++) {
                        rectPlayer.Y -= 1;
                    
                        if (rectObj.Intersects(rectPlayer)) {
                            rectPlayer.Y += 1;
                            vectors.Add(( -(i - 1), "y" ));
                            break;
                        }
                    }
                }
            }

            if (!noCollisionMode)
            {
                if (vectors.Count > 0) {
                    bool pixelMinus = false;
                    vectors.Sort();
                    if (vectors[0].pixel < 0)
                        pixelMinus = true;
                
                    int valueDeltaPixel = pixelMinus ? vectors.Last().pixel : vectors[0].pixel;
                
                    if (vectors[0].direction == "x")
                    {
                        vector.x = valueDeltaPixel;
                        if (vector.x == 0) move = false;
                    }
                
                    else if (vectors[0].direction == "y")
                    {
                        vector.y = valueDeltaPixel;
                        if (vector.y == 0) move = false;
                    }
                }
            }
            
            if (!move && sprinting)
            {
                player.control.sprintPoint += player.control.sprintPointCost;
                player.tool.RestoreSprintPoint(player);
            }
            
            player.draw.rect.X += vector.x;
            player.draw.rect.Y += vector.y;
            player.draw.rectCollide.X += vector.x;
            player.draw.rectCollide.Y += vector.y;
        }
    }

    public class Debugs
    {
        public ShapeBatch ShapeBatch;

        public Debugs(GraphicsDevice device, ContentManager content)
        {
            ShapeBatch = new ShapeBatch(device, content);
        }
        
        public void DrawHitbox(Player player, SpriteBatch surface)
        {
            ShapeBatch.Begin();
            ShapeBatch.DrawRectangle(
                new Vector2(player.draw.rectCollideConst.X, player.draw.rectCollideConst.Y),
                new Vector2(player.draw.rectCollideConst.Width, player.draw.rectCollideConst.Height),
                new Color(0, 0, 0, 0),
                Color.Purple,
                2f);
            ShapeBatch.End();
        }
    }

    public class UI
    {
        public ShapeBatch sprintBarSurface;
        
        public void Mainloop(Player player)
        {
            sprintBarSurface.Begin();
            DrawSprintBox();
            DrawSprintLine(player);
            sprintBarSurface.End();
        }

        public void DrawSprintBox()
        {
            sprintBarSurface.DrawRectangle(
                new Vector2(50, 620),
                new Vector2(240, 30),
                new Color(0, 0, 0, 0),
                new Color(255, 255, 200, 255),
                100
            );
        }

        public void DrawSprintLine(Player player)
        {
            float percentLine;
            if (player.control.sprintPoint == 0) return;
            else
            {
                percentLine = 
                    (float)player.control.sprintPoint /
                    (float)player.control.sprintPointMax;
            }

            sprintBarSurface.DrawRectangle(
                new Vector2(55, 625),
                new Vector2(230 * percentLine, 20),
                new Color(0, 0, 0, 0),
                new Color(200, 0, 0, 200),
                100
                );
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            sprintBarSurface = new ShapeBatch(device, content);
        }
    }

    public class Fight
    {
        public RectangleF battleRect;
        public bool isFighting;

        public void startFight(Player player, BaseEnemy enemy, Camera camera)
        {
            player.tool.SetPositionPlayer(player, camera, -200, -200);
        }

        public bool EnemyChasedPlayer(RectangleF playerRect, RectangleF enemyRect)
        {
            return playerRect.Intersects(enemyRect);
        }

    }

    public class Tool
    {
        public void RestoreSprintPoint(Player player)
        {
            if (player.control.sprintPoint == player.control.sprintPointMax) return;
            
            if (player.control.sprintPoint + player.control.sprintPointRegeneration <= player.control.sprintPointMax)
            {
                player.control.sprintPoint += player.control.sprintPointRegeneration;
            }
            else
            {
                player.control.sprintPoint += player.control.sprintPointMax - player.control.sprintPoint;
            }
        }
        
        public void SetPositionPlayer(Player player, Camera camera, int x, int y)
        {
            x -= (int)player.draw.rect.X;
            y -= (int)player.draw.rect.Y + (int)player.draw.rect.Height - 48;

            player.draw.rect.X += x;
            player.draw.rect.Y += y;
            player.draw.rectCollide.X += x;
            player.draw.rectCollide.Y += y;
            camera.x += x;
            camera.y += y;
        }
        
        public void UpdatePlayerRect(Player player, World world)
        {
            world.allObjectsAndY[player] = player.draw.rect.Y;
        }
    }
}