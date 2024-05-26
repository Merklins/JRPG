using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG_GAME.Components;
using RPG_GAME.Core;
using RPG_GAME.Enemy;
using RPG_GAME.Enemy.Rat;
using RPG_GAME.MapObjects;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace RPG_GAME;

public class World
{
    public Draw draw = new Draw();
    public Control control = new Control();
    public Tool tool = new Tool();
    public Debugs debugs = new Debugs();
    
    public Dictionary<object, float> allObjectsAndY = new Dictionary<object, float>();
    public List<RectangleF> borderTile = new List<RectangleF>();
    public List<BaseEnemy> enemyMap = new List<BaseEnemy>();
    public Dictionary<string, BaseWind> nameAndEachWindTiledObject = new Dictionary<string, BaseWind>();

    public Dictionary<int, List<(int x, int y, int width, int height)>> gidTileAndBorderTile =
        new Dictionary<int, List<(int x, int y, int width, int height)>>()
        {
            {7, new List<(int x, int y, int width, int height)>()
            {
               (0, 0, 96, 144) 
            }},
            {89, new List<(int x, int y, int width, int height)>()
            {
               (-8, -4, 36, 40) 
            }},
            {101, new List<(int x, int y, int width, int height)>()
            {
               (-31, -46, 18, 1) 
            }},
            {102, new List<(int x, int y, int width, int height)>()
            {
                (0, -46, 23, 1)
            }},
            {117, new List<(int x, int y, int width, int height)>()
            {
                (-31, 2, 17, 1)
            }},
            {118, new List<(int x, int y, int width, int height)>()
            {
                (0, 2, 23, 1)
            }},
            {129, new List<(int x, int y, int width, int height)>()
            {
                (-23, 2, 39, 1), (-22, 2, 1, 48)
            }},
            {130, new List<(int x, int y, int width, int height)>()
            {
                (0, 2, 48, 1)
            }},
            {131, new List<(int x, int y, int width, int height)>()
            {
                (0, 2, 48, 1)
            }},
            {132, new List<(int x, int y, int width, int height)>()
            {
                (10, 2, 42, 1), (-31, 2, 1, 48)
            }},
            {145, new List<(int x, int y, int width, int height)>()
            {
                (-22, 2, 1, 48)
            }},
            {148, new List<(int x, int y, int width, int height)>()
            {
               (-31, 2, 1, 48) 
            }},
            {161, new List<(int x, int y, int width, int height)>()
            {
                (-22, 2, 1, 48)
            }},
            {164, new List<(int x, int y, int width, int height)>()
            {
                (-31, 2, 1, 48)
            }},
            {177, new List<(int x, int y, int width, int height)>()
            {
               (-23, 2, 1, 49), (-23, -46, 25, 1)
            }},
            {178, new List<(int x, int y, int width, int height)>()
            {
                (0, -46, 48, 1)
            }},
            {179, new List<(int x, int y, int width, int height)>()
            {
                (0, -46, 48, 1)
            }},
            {180, new List<(int x, int y, int width, int height)>()
            {
                (0, -46, 31, 1), (-31, 0, 1, 47)
            }}
        };

    private GraphicsDevice device;
    private ContentManager content;

    public class Draw
    {
        private SpriteFont font;
        
        private ShapeBatch ShapeBatch;
        private Texture2D TilesetTexture;
        public Texture2D TreeSprite;
        public Texture2D BigRockSprite;
        public Texture2D BushSprite;
        public Texture2D GreenSprite;
        public Texture2D BluePlantSprite;

        public TiledMap map;
        private Dictionary<int, TiledTileset> tilesets;
        
        public void Mainloop(Player player, Camera camera, World world, SpriteBatch surface)
        {
            DrawBackground(player, camera, world, surface);
            DrawObject(player, camera, world, surface);
        }
            
        public void Load(World world, ContentManager Content, GraphicsDevice device)
        {
            world.content = Content;
            world.device = device;
            this.ShapeBatch = new ShapeBatch(device, Content);
            world.debugs.ShapeBatch = new ShapeBatch(device, Content);
            map = new TiledMap(Content.RootDirectory + "\\Map\\Field.tmx");
            
            TilesetTexture = Content.Load<Texture2D>("Map\\Forest");
            TreeSprite = Content.Load<Texture2D>("Map\\Tree1");
            BigRockSprite = Content.Load<Texture2D>("Map\\Big_rock");
            BushSprite = Content.Load<Texture2D>("Map\\Bush");
            GreenSprite = Content.Load<Texture2D>("Map\\Green");
            BluePlantSprite = Content.Load<Texture2D>("Map\\Blue_plant");

            font = Content.Load<SpriteFont>("Map\\Font");

            tilesets = map.GetTiledTilesets(Content.RootDirectory + "\\Map\\");
            
            world.tool.SpawnEnemy(world, device, Content);
        }

        public void DrawBackground(Player player, Camera camera, World world, SpriteBatch surface)
        {
            var dest = new Rectangle(0, 0, map.TileWidth, map.TileHeight);
            var src = new Rectangle(0, 0, map.TileWidth, map.TileHeight);

            foreach (var layer in map.Layers) {
                if (layer.name != "Background" && layer.name != "Украшения") continue;
                for (int x = 0; x < layer.width; x++) {
                    for (int y = 0; y < layer.height; y++) {
                        var index = (y * layer.width) + x;
                        var gid = layer.data[index];
                        var tileX = x * map.TileWidth;
                        var tileY = y * map.TileHeight;
                        
                        if (!new RectangleF(camera.x, camera.y, 1280, 720).Intersects(
                            new RectangleF((float)tileX, (float)tileY, 48, 48)))
                            continue;
                      

                        if (gid == 0) continue;

                        var mapTileset = map.GetTiledMapTileset(gid);
                        var tileset = tilesets[mapTileset.firstgid];

                        var rect = map.GetSourceRect(mapTileset, tileset, gid);

                        var source = new Rectangle(rect.x, rect.y, map.TileWidth, map.TileHeight);
                        var destination = new Rectangle(
                            (int)(tileX - camera.x),
                            (int)(tileY - camera.y),  
                            map.TileWidth, map.TileHeight);

                        surface.Draw(TilesetTexture, destination, source, Color.White);
                        // surface.DrawString(font, $"{gid}", new Vector2(tileX - camera.x, tileY - camera.y), Color.Black); //for print gid
                    }
                }
            }
        }

        public void DrawObject(Player player, Camera camera, World world, SpriteBatch surface)
        {
            foreach (var obj in world.allObjectsAndY.Keys)
            {
                //if (obj is Player)
                //    ((Player)obj).draw.Mainloop(player, surface);

                //else 
                if (obj is BaseTiledObject tiledObject)
                {
                    if (BaseTiledObject.noWindSensetiveObject.Contains(tiledObject.name))
                    {
                        tiledObject.DrawWithoutWind(surface, camera);
                        continue;
                    }

                    if (BaseTiledObject.useBaseWind)
                    {
                        if (tiledObject is Tree tree)
                            tree.DrawWithBaseWind(surface, camera, world.nameAndEachWindTiledObject[tree.name]);
                        
                        else
                            tiledObject.DrawWithBaseWind(surface, camera, world.nameAndEachWindTiledObject[tiledObject.name]);
                    }

                    else
                    {
                        if (tiledObject is Tree tree)
                            tree.DrawWithSpecificWind(surface, camera);
                    
                        else
                            tiledObject.DrawWithSpecificWind(surface, camera);
                    }
                }
                
                else if (obj is Rat ratEnemy)
                {
                    // ratEnemy.draw.Mainloop(ratEnemy, surface, camera);
                }
            }
        }
    }

    public class Control
    {
        public void Mainloop(Player player, Camera camera, World world, GameTime gameTime)
        {
            MoveEnemy(world, player);
            world.tool.UpdateEnemyRect(world);
            player.tool.UpdatePlayerRect(player, world);
            UpdateTiledObjects(world, camera, gameTime);
            world.tool.SortObjects(player, camera, world);
            
            CheckAndStartFightWithEnemy(world, player, camera);
        }

        public void CheckAndStartFightWithEnemy(World world, Player player, Camera camera)
        {
            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat ratEnemy)
                {
                    if (!player.fight.EnemyChasedPlayer(player.draw.rect, ratEnemy.draw.rect)) continue;
                    
                    //player.fight.startFight(player, enemy, camera
                }
            }
        }

        public void UpdateTiledObjects(World world, Camera camera, GameTime gameTime)
        {
            if (BaseTiledObject.useBaseWind) {
                foreach (BaseWind wind in world.nameAndEachWindTiledObject.Values)
                    wind.Update(gameTime);
            }

            else {
                foreach (object tiledObject in world.allObjectsAndY.Keys) {
                    if (tiledObject is BaseTiledObject baseTiledObject)
                        baseTiledObject.Update(gameTime);
                }
            }
        }
        
        public void MoveEnemy(World world, Player player)
        {
            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat ratEnemy)
                {
                    ratEnemy.control.Mainloop(ratEnemy, player);
                }
            }

            foreach (BaseEnemy enemy in world.enemyMap) {
                foreach (RectangleF borderRect in world.borderTile) {
                    if (enemy is Rat ratEnemy)
                    {
                        if (!ratEnemy.tool.SeePlayer(ratEnemy.draw.rectVisionArea, player)) continue;
                        ratEnemy.control.CalculateAllowVectorsBorder(borderRect, ratEnemy.draw.collideRect);
                    }
                }
            }


            foreach (BaseEnemy enemy in world.enemyMap) {
                foreach (object obj in world.allObjectsAndY.Keys) {
                    if (!(obj is TiledObject)) continue;
                    
                    if (enemy is Rat ratEnemy)
                    {
                        if (!ratEnemy.tool.SeePlayer(ratEnemy.draw.rectVisionArea, player)) continue;
                        ratEnemy.control.CalculateAllowVectorsObject(world, obj, ratEnemy.draw.collideRect);
                    }
                }
            }
            

            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat ratEnemy)
                {
                    if (!ratEnemy.tool.SeePlayer(ratEnemy.draw.rectVisionArea, player)) continue;
                    ratEnemy.control.AddAllowVectors(ref ratEnemy.draw.rect, ref ratEnemy.draw.collideRect, ref ratEnemy.draw.rectVisionArea);
                    ratEnemy.tool.AnimationMove(ratEnemy, player);
                }
            }
            
        }
    }

    public class Tool
    {
        public bool add = false;
        
        public void UpdateEnemyRect(World world)
        {
            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat ratEnemy)
                    world.allObjectsAndY[enemy] = ratEnemy.draw.rect.Y;
            }
        }
        
        public void SpawnEnemy(World world, GraphicsDevice device, ContentManager content)
        {
            for (int i = 0; i < 0; i++)
            {
                int x = Random.Shared.Next(500, 5000);
                int y = Random.Shared.Next(500, 5000);
                
                world.enemyMap.Add(new Rat(device, content, x, y));
            }
        }

        public void AddWorldBorderTile(Player player, Camera camera, World world)
        {
            foreach (TiledLayer layer in world.draw.map.Layers) {
                if (layer.name != "Background" && layer.name != "Украшения") continue;
                for (int x = 0; x < layer.width; x++) {
                    for (int y = 0; y < layer.height; y++) {
                        var index = (y * layer.width) + x;
                        var gid = layer.data[index];
                        var tileX = x * world.draw.map.TileWidth;
                        var tileY = y * world.draw.map.TileHeight;

                       

                        foreach (var gidTileData in world.gidTileAndBorderTile) {
                            if (gid == gidTileData.Key) {
                                foreach (var rect in gidTileData.Value) {
                                    world.borderTile.Add(new RectangleF(
                                        tileX - rect.x, tileY - rect.y,
                                        rect.width, rect.height));
                                }
                            }
                        }

           
                    }
                }
            }
        }
        
        public void AddWorldObjects(Player player, Camera camera, World world)
        {
            foreach (var layer in world.draw.map.Layers)
            {
                if (layer.name != "Лесные стены") continue;
                foreach (TiledObject tiledObject in layer.objects)
                {
                    if (tiledObject.name == "Blue_plant")
                    {
                        if (!world.nameAndEachWindTiledObject.ContainsKey("Blue_plant"))
                            world.nameAndEachWindTiledObject.Add("Blue_plant", new BaseWind(
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.7f, timeToRandomChangeParameters:4f));
                        
                        world.allObjectsAndY.Add(new Flower(tiledObject, world.draw.BluePlantSprite,
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.7f, timeToRandomChangeParameters:4f),
                            tiledObject.y - tiledObject.height);
                    }
                    
                    else if (tiledObject.name == "Tree")
                    {
                        if (!world.nameAndEachWindTiledObject.ContainsKey("Tree"))
                            world.nameAndEachWindTiledObject.Add("Tree", new BaseWind(
                                oscillationSpeed:1.5f, amplitude:18, phaseShift:0f,
                                dampingCoefficient:0.3f, timeToRandomChangeParameters:7f));
                        
                        world.allObjectsAndY.Add(new Tree(tiledObject, world.draw.TreeSprite,
                                oscillationSpeed:1.5f, amplitude:18, phaseShift:Random.Shared.NextFloat(0f, 3f),
                                dampingCoefficient:0.3f, timeToRandomChangeParameters:7f),
                            tiledObject.y - tiledObject.height);
                    }
                    
                    else if (tiledObject.name == "Bush")
                    {
                        if (!world.nameAndEachWindTiledObject.ContainsKey("Bush"))
                            world.nameAndEachWindTiledObject.Add("Bush", new BaseWind(
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.9f, timeToRandomChangeParameters:4f));
                        
                        world.allObjectsAndY.Add(new Bush(tiledObject, world.draw.BushSprite, 
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.9f, timeToRandomChangeParameters:4f),
                            tiledObject.y - tiledObject.height);
                    }
                    
                    else if (tiledObject.name == "Rock")
                    {
                        if (!world.nameAndEachWindTiledObject.ContainsKey("Rock"))
                            world.nameAndEachWindTiledObject.Add("Rock", new BaseWind(
                                oscillationSpeed:0f, amplitude:0f, phaseShift:0f,
                                dampingCoefficient:0f, timeToRandomChangeParameters:0));
                        
                        world.allObjectsAndY.Add(new Rock(tiledObject, world.draw.BigRockSprite,
                                oscillationSpeed:0f, amplitude:0f, phaseShift:0f,
                                dampingCoefficient:0f, timeToRandomChangeParameters:0),
                            tiledObject.y - tiledObject.height);
                    }
                    
                    else if (tiledObject.name == "Green")
                    {
                        if (!world.nameAndEachWindTiledObject.ContainsKey("Green"))
                            world.nameAndEachWindTiledObject.Add("Green", new BaseWind(
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.9f, timeToRandomChangeParameters:10f));
                        
                        world.allObjectsAndY.Add(new Green(tiledObject, world.draw.GreenSprite,
                                oscillationSpeed:2f, amplitude:9f, phaseShift:0f,
                                dampingCoefficient:0.9f, timeToRandomChangeParameters:10f),
                            tiledObject.y - tiledObject.height);
                    }
                }
            }
        }
        
        public RectangleF GetCollideRect(BaseTiledObject baseTiledObject)
        {
            if (baseTiledObject.name == "Tree")
            {
                return new RectangleF(
                    baseTiledObject.x + 70,
                    baseTiledObject.y - 70,
                    100, 70);
            }
            
            else if (baseTiledObject.name == "Bush")
            {
                return new RectangleF(
                    baseTiledObject.x,
                    baseTiledObject.y - 48,
                    baseTiledObject.width,
                    baseTiledObject.height / 2);
            }
            
            else if (baseTiledObject.name == "Green")
            {
                return new RectangleF(
                    baseTiledObject.x,
                    baseTiledObject.y - 40,
                    baseTiledObject.width,
                    baseTiledObject.height - 56);
            }
            
            else if (baseTiledObject.name == "Rock")
            {
                return new RectangleF(
                    baseTiledObject.x,
                    baseTiledObject.y - 56,
                    baseTiledObject.width,
                    baseTiledObject.height - 48);
            }
            
            else if (baseTiledObject.name == "Blue_plant")
            {
                return new RectangleF(0, 0, 0, 0);
            }

            return new RectangleF(baseTiledObject.x, baseTiledObject.y - baseTiledObject.height, baseTiledObject.width, baseTiledObject.height);
        }

        public void SetBottomLeftCornerObjectsY(Player player, World world)
        {
            foreach (var obj in world.allObjectsAndY.Keys)
            {
                if (obj is Player)
                {
                    world.allObjectsAndY[obj] += ((Player)obj).draw.rect.Height;
                }

                else if (obj is BaseTiledObject)
                {
                    world.allObjectsAndY[obj] += ((BaseTiledObject)obj).height;
                }
                
                else if (obj is Rat)
                {
                    world.allObjectsAndY[obj] += ((Rat)obj).draw.sprite.Height;
                }
            }
        }

        public void SetUpperLeftCornerObjectsY(Player player, World world)
        {
            foreach (var obj in world.allObjectsAndY.Keys)
            {
                if (obj is Player)
                {
                    world.allObjectsAndY[obj] -= ((Player)obj).draw.rect.Height;
                }

                else if (obj is BaseTiledObject)
                {
                    world.allObjectsAndY[obj] -= ((BaseTiledObject)obj).height;
                }
                
                else if (obj is Rat)
                {
                    world.allObjectsAndY[obj] -= ((Rat)obj).draw.sprite.Height;
                }
            }
        }

        public void SortObjects(Player player, Camera camera, World world)
        {
            SetBottomLeftCornerObjectsY(player, world);
            world.allObjectsAndY = world.allObjectsAndY.OrderBy(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            SetUpperLeftCornerObjectsY(player, world);
        }

        
    }

    public class Debugs
    {
        public ShapeBatch ShapeBatch;

        public void DrawRectVisionAreaEnemy(World world, Camera camera)
        {
            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat ratEnemy)
                {
                    ratEnemy.debugs.DrawVisionArea(ratEnemy.draw.rectVisionArea, camera);
                }
            }
        }
        
        public void DrawCollideRectEnemy(World world, Camera camera)
        {
            foreach (BaseEnemy enemy in world.enemyMap)
            {
                if (enemy is Rat)
                {
                    Rat obj = (Rat)enemy;
                    obj.debugs.DrawHitbox(obj.draw.collideRect, camera);
                }
            }
        }
        
        public void DrawBorderTile(Camera camera, World world, Entity player)
        {
            foreach (RectangleF tileRect in world.borderTile)
            {
                ShapeBatch.Begin();
                ShapeBatch.DrawRectangle(
                    new Vector2(tileRect.X - camera.x, tileRect.Y - camera.y),
                    new Vector2(tileRect.Width, tileRect.Height),
                    new Color(0, 0, 0, 0), Color.Red);
                ShapeBatch.End();
            }
        }
        
        public void DrawObjectHitbox(Player player, Camera camera, World world, SpriteBatch surface)
        {
            ShapeBatch.Begin();
            
            foreach (var obj in world.allObjectsAndY.Keys)
            {
                if (obj is BaseTiledObject tiledObject)
                {
                    RectangleF rect = world.tool.GetCollideRect(tiledObject);
                    ShapeBatch.DrawRectangle(
                        new Vector2(rect.X - camera.x, rect.Y - camera.y),
                        new Vector2(rect.Width, rect.Height),
                        new Color(0, 0, 0, 0),
                        Color.Red,
                        1f);
                }
            }
            
            ShapeBatch.End();
        }
    }
}