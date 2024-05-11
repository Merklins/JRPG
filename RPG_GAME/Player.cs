using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apos.Shapes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG_GAME.MapObjects;
using SharpDX;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Rectangle = SharpDX.Rectangle;
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
        public void mainloop(Player player, SpriteBatch surface)
        {
            animation_move(player);
            draw_sprite(player, surface);
        }

        public int animation_frame = 0;
        
        public List<Texture2D> walkdown = new List<Texture2D>();
        public List<Texture2D> walkup = new List<Texture2D>();
        public List<Texture2D> walkright = new List<Texture2D>();
        public List<Texture2D> walkleft = new List<Texture2D>();

        public Texture2D sprite;
        public RectangleF rect;
        public RectangleF rectCollide;
        public RectangleF rectCollideConst;
        private Vector2 position_screen;
        public ShapeBatch ShapeBatch;

        public void load_sprite_player(Player player, ContentManager content)
        {
            List<string> direction_name = new List<string>()
            {
                "left", "right", "up", "down"
            };

            foreach (string direction in direction_name) {
                for (int i = 1; i <= 3; i++) {
                    switch (direction) {
                        case "left": {
                            walkleft.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "right": {
                            walkright.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "up": {
                            walkup.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                        case "down": {
                            walkdown.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
                            break;
                        }
                    }
                }
            }
        }

        public void load_draw_player(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
        {
            float x_offset = -0;
            float y_offset = -140;
            float width_offset = 0;
            float height_offset = 70;
            
            player.content = content;
            player.device = device;
            this.ShapeBatch = new ShapeBatch(device, content);
            sprite = content.Load<Texture2D>("Player\\Ninja_down1");
            position_screen = new Vector2((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2);
            rect = new RectangleF((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2, sprite.Width, sprite.Height);
            rectCollide = new RectangleF(
                (manager.PreferredBackBufferWidth - sprite.Width - x_offset) / 2 + 5,
                (manager.PreferredBackBufferHeight - sprite.Height - y_offset) / 2,
                sprite.Width - width_offset - 10,
                sprite.Height - height_offset
            );
            rectCollideConst = new RectangleF(
                (manager.PreferredBackBufferWidth - sprite.Width - x_offset) / 2 + 5,
                (manager.PreferredBackBufferHeight - sprite.Height - y_offset) / 2,
                sprite.Width - width_offset - 10,
                sprite.Height - height_offset
            );
        }

        public void load(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
        {
            player.draw.load_draw_player(player, content, device, manager);
            player.draw.load_sprite_player(player, content);
            player.ui.load(content, device);
        }

        public void draw_sprite(Player player, SpriteBatch surface)
        {
            surface.Draw(sprite, position_screen, Color.White);
        }

        public void animation_move(Player player)
        {
            int count_animation_frame = 16;
            if (player.control.sprinting) count_animation_frame = 10;

            if (animation_frame + 1 >= count_animation_frame * walkdown.Count) animation_frame = 0;
            
            if (player.control.up)
            {
                if (!player.control.move)
                {
                    sprite = walkup[0];
                    animation_frame = 0;
                }

                else
                {
                    sprite = walkup[animation_frame / count_animation_frame];
                    animation_frame++;
                }
                player.control.old_direct = "up";
            }
        
            else if (player.control.down)
            {
                if (!player.control.move)
                {
                    sprite = walkdown[0];
                    animation_frame = 0;
                }
                else
                {
                    sprite = walkdown[animation_frame / count_animation_frame];
                    animation_frame++;
                }
                
                player.control.old_direct = "down";
            }
        
            else if (player.control.left)
            {
                if (!player.control.move)
                {
                    sprite = walkleft[0];
                    animation_frame = 0;
                }
                else
                {
                    sprite = walkleft[animation_frame / count_animation_frame];
                    animation_frame++;
                }
                
                player.control.old_direct = "left";
            }
        
            else if (player.control.right)
            {
                if (!player.control.move)
                {
                    sprite = walkright[0];
                    animation_frame = 0;
                }
                else
                {
                    sprite = walkright[animation_frame / count_animation_frame];
                    animation_frame++;
                }
                
                player.control.old_direct = "right";
            }
            
            else {
                switch (player.control.old_direct) {
                    case "up": {
                        sprite = walkup[0];
                        break;
                    }
                    
                    case "down": {
                        sprite = walkdown[0];
                        break;
                    }

                    case "right": {
                        sprite = walkright[0];
                        break;
                    }

                    case "left": {
                        sprite = walkleft[0];
                        break;
                    }
                }
            }
        }
    }

    public class Control
    {
        public bool noCollisionMode = false;
        
        public int sprint_point_max = 3600;
        public int sprint_point = 3600;
        public int sprint_point_regeneration = 2;
        public int sprint_point_cost = 5;
        
        public (float x, float y) vector = (0, 0);
        public (string direction, int value) undo_vector = ("none", 0);
        public float player_speed = 5.62f;

        public bool up = false;
        public bool down = false;
        public bool right = false;
        public bool left = false;
        public string old_direct = "none";
        public bool sprinting = false;

        public bool pressedQ = false;

        public bool move = false;

        public void mainloop(Player player, Camera camera, World world)
        {
            key_move(player, camera);
            key_sprint(player);

            move_position(player, world);
        }

        public void key_move(Player player, Camera camera)
        {
            vector.x = 0; vector.y = 0;
            up = false;
            down = false;
            left = false;
            right = false;
            
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                vector.x -= player_speed;
                left = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                vector.x += player_speed;
                right = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                vector.y -= player_speed;
                up = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                vector.y += player_speed;
                down = true;
                move = true;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.Q) && !pressedQ)
            {
                // player.tool.set_position_player(player, camera, 2600, 1920);
                if (BaseTiledObject.useBaseWind)
                    BaseTiledObject.useBaseWind = false;
                else
                    BaseTiledObject.useBaseWind = true;

                pressedQ = true;
            }
            
            else if (Keyboard.GetState().IsKeyUp(Keys.Q))
            {
                pressedQ = false;
            }
            
            else if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                if (player.control.noCollisionMode)
                    player.control.noCollisionMode = false;
                else
                    player.control.noCollisionMode = true;
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

        public void key_sprint(Player player)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (vector.x == 0 && vector.y == 0)
                {
                    player.tool.restore_sprint_point(player);
                    sprinting = false;
                }

                if (sprint_point >= sprint_point_cost && (vector.x != 0 || vector.y != 0))
                {
                    vector.x = vector.x * 1.5f;
                    vector.y = vector.y * 1.5f;
                    sprint_point -= sprint_point_cost;
                    sprinting = true;
                }

                else if (sprint_point < sprint_point_cost)
                {
                    sprint_point = 0;
                    sprinting = false;
                }
            }

            else if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                player.tool.restore_sprint_point(player);
                sprinting = false;
            }
        }

        public void move_position(Player player, World world)
        {
            List<(int pixel, string direction)> vectors = new List<(int pixel, string direction)>();

            foreach (var rect_tile in world.borderTile) {
                var rectPlayer = player.draw.rectCollide;
                if (vector.x > 0) {
                    for (int i = 1; i < vector.x+1; i++) {
                        rectPlayer.X += 1;

                        if (rect_tile.Intersects(rectPlayer)) {
                            rectPlayer.X -= 1;
                            vectors.Add((i - 1, "x"));
                            break;
                        }
                    }
                }
                
                else if (vector.x < 0) {
                    for (int i = 1; i < -vector.x+1; i++) {
                        rectPlayer.X -= 1;
                    
                        if (rect_tile.Intersects(rectPlayer)) {
                            rectPlayer.X += 1;
                            vectors.Add( (-(i - 1), "x") );
                            break;
                        }
                    }
                }
            
                else if (vector.y > 0) {
                    for (int i = 1; i < vector.y+1; i++) {
                        rectPlayer.Y += 1;
                    
                        if (rect_tile.Intersects(rectPlayer)) {
                            rectPlayer.Y -= 1;
                            vectors.Add((i - 1, "y"));
                            break;
                        }
                    }
                }
            
                else if (vector.y < 0) {
                    for (int i = 1; i < -vector.y+1; i++) {
                        rectPlayer.Y -= 1;
                    
                        if (rect_tile.Intersects(rectPlayer)) {
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
                    bool pixel_minus = false;
                    vectors.Sort();
                    if (vectors[0].pixel < 0)
                        pixel_minus = true;
                
                    int value_delta_pixel = pixel_minus ? vectors.Last().pixel : vectors[0].pixel;
                
                    if (vectors[0].direction == "x")
                    {
                        vector.x = value_delta_pixel;
                        if (vector.x == 0) move = false;
                    }
                
                    else if (vectors[0].direction == "y")
                    {
                        vector.y = value_delta_pixel;
                        if (vector.y == 0) move = false;
                    }
                }
            }
            
            if (!move && sprinting)
            {
                player.control.sprint_point += player.control.sprint_point_cost;
                player.tool.restore_sprint_point(player);
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
        
        public void draw_hitbox(Player player, SpriteBatch surface)
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
        public ShapeBatch sprint_bar_surface;
        
        public void mainloop(Player player)
        {
            sprint_bar_surface.Begin();
            draw_sprint_box();
            draw_sprint_line(player);
            sprint_bar_surface.End();
        }

        public void draw_sprint_box()
        {
            sprint_bar_surface.DrawRectangle(
                new Vector2(50, 620),
                new Vector2(240, 30),
                new Color(0, 0, 0, 0),
                new Color(255, 255, 200, 255),
                100
            );
        }

        public void draw_sprint_line(Player player)
        {
            float percentLine;
            if (player.control.sprint_point == 0) return;
            else
            {
                percentLine = 
                    (float)player.control.sprint_point /
                    (float)player.control.sprint_point_max;
            }

            sprint_bar_surface.DrawRectangle(
                new Vector2(55, 625),
                new Vector2(230 * percentLine, 20),
                new Color(0, 0, 0, 0),
                new Color(200, 0, 0, 200),
                100
                );
        }

        public void load(ContentManager content, GraphicsDevice device)
        {
            sprint_bar_surface = new ShapeBatch(device, content);
        }
    }

    public class Fight
    {
        public RectangleF battleRect;
        public bool isFighting;

        public void startFight(Player player, Enemy enemy, Camera camera)
        {
            player.tool.set_position_player(player, camera, -200, -200);
        }

        public bool EnemyChasedPlayer(RectangleF playerRect, RectangleF enemyRect)
        {
            return playerRect.Intersects(enemyRect);
        }

    }

    public class Tool
    {
        public void restore_sprint_point(Player player)
        {
            if (player.control.sprint_point == player.control.sprint_point_max) return;
            
            if (player.control.sprint_point + player.control.sprint_point_regeneration <= player.control.sprint_point_max)
            {
                player.control.sprint_point += player.control.sprint_point_regeneration;
            }
            else
            {
                player.control.sprint_point += player.control.sprint_point_max - player.control.sprint_point;
            }
        }
        
        public void set_position_player(Player player, Camera camera, int x, int y)
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
        
        public void update_player_rect(Player player, World world)
        {
            world.allObjectsAndY[player] = player.draw.rect.Y;
        }
    }
}