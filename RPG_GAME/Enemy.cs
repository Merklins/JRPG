using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apos.Shapes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG_GAME.MapObjects;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace RPG_GAME;

public abstract class Enemy
{
    public Draw draw;
    public Control control;
    public Tool tool;
    public Debugs debugs;

    public class Draw
    {
        public Texture2D sprite;
        public RectangleF rect;
        public RectangleF collideRect;
        public RectangleF rectVisionArea;
        
        public List<Texture2D> walkLeft = new List<Texture2D>();
        public List<Texture2D> walkRight = new List<Texture2D>();
        public List<Texture2D> walkUp = new List<Texture2D>();
        public List<Texture2D> walkDown = new List<Texture2D>();

        public virtual void DrawSprite(RectangleF rect, SpriteBatch surface, Camera camera)
        {
            surface.Draw(sprite, new Vector2(rect.X - camera.x, rect.Y - camera.y), Color.White);
        }
    }

    public class Control
    {
        public string chaseDirection = "none";
        public char chasePriority = 'x';
        public float enemySpeed;
        public (float x, float y) vector = (0, 0);
        public bool moved;
        public bool catchPlayerOnAxis = false;
        public string oldDirect;

        public (bool right, bool left, bool up, bool down) directs = (false, false, false, false);
        protected List<(int pixel, string direction)> vectors = new List<(int pixel, string direction)>();
        public List<(string direct, RectangleF rect)> historyDirection = new List<(string direct, RectangleF rect)>();

        public virtual void AddAllowVectors(ref RectangleF rectEnemy, ref RectangleF collidRectEnemy, ref RectangleF rectVisionArea)
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
                    if (vector.x == 0) moved = false;
                }
                
                else if (vectors[0].direction == "y")
                {
                    vector.y = valueDeltaPixel;
                    if (vector.y == 0) moved = false;
                }
            }
            
            if (vector.x > 0)
                directs.right = true;
            else if (vector.x < 0)
                directs.left = true;
            else if (vector.y > 0)
                directs.down = true;
            else if (vector.y < 0)
                directs.up = true;
            
            rectEnemy.X += vector.x;
            rectEnemy.Y += vector.y;
            collidRectEnemy.X += vector.x;
            collidRectEnemy.Y += vector.y;
            rectVisionArea.X += vector.x;
            rectVisionArea.Y += vector.y;
            
            vectors.Clear();
        }

        public virtual void CalculateAllowVectorsObject(World world, object tiledObject, RectangleF collideRectEnemy)
        {
            if (!(tiledObject is BaseTiledObject)) return;

            RectangleF rectObj = world.tool.GetCollideRect((BaseTiledObject)tiledObject);

            if (vector.x > 0) {
                for (int i = 1; i < vector.x+1; i++) {
                    collideRectEnemy.X += 1;

                    if (rectObj.Intersects(collideRectEnemy)) {
                        collideRectEnemy.X -= 1;
                        vectors.Add((i - 1, "x"));
                        break;
                    }
                }
            }
                
            else if (vector.x < 0) {
                for (int i = 1; i < -vector.x+1; i++) {
                    collideRectEnemy.X -= 1;
                    
                    if (rectObj.Intersects(collideRectEnemy)) {
                        collideRectEnemy.X += 1;
                        vectors.Add( (-(i - 1), "x") );
                        break;
                    }
                }
            }
            
            else if (vector.y > 0) {
                for (int i = 1; i < vector.y+1; i++) {
                    collideRectEnemy.Y += 1;
                    
                    if (rectObj.Intersects(collideRectEnemy)) {
                        collideRectEnemy.Y -= 1;
                        vectors.Add((i - 1, "y"));
                        break;
                    }
                }
            }
            
            else if (vector.y < 0) {
                for (int i = 1; i < -vector.y+1; i++) {
                    collideRectEnemy.Y -= 1;
                    
                    if (rectObj.Intersects(collideRectEnemy)) {
                        collideRectEnemy.Y += 1;
                        vectors.Add(( -(i - 1), "y" ));
                        break;
                    }
                }
            }
        }
        
        public virtual void CalculateAllowVectorsBorder(RectangleF borderRect, RectangleF collideRectEnemy)
        {
            if (vector.x > 0) {
                for (int i = 1; i < vector.x+1; i++) {
                    collideRectEnemy.X += 1;

                    if (borderRect.Intersects(collideRectEnemy)) {
                        collideRectEnemy.X -= 1;
                        vectors.Add((i - 1, "x"));
                        break;
                    }
                }
            }
            
            else if (vector.x < 0) {
                for (int i = 1; i < -vector.x+1; i++) {
                    collideRectEnemy.X -= 1;
                
                    if (borderRect.Intersects(collideRectEnemy)) {
                        collideRectEnemy.X += 1;
                        vectors.Add( (-(i - 1), "x") );
                        break;
                    }
                }
            }
        
            else if (vector.y > 0) {
                for (int i = 1; i < vector.y+1; i++) {
                    collideRectEnemy.Y += 1;
                
                    if (borderRect.Intersects(collideRectEnemy)) {
                        collideRectEnemy.Y -= 1;
                        vectors.Add((i - 1, "y"));
                        break;
                    }
                }
            }
        
            else if (vector.y < 0) {
                for (int i = 1; i < -vector.y+1; i++) {
                    collideRectEnemy.Y -= 1;
                
                    if (borderRect.Intersects(collideRectEnemy)) {
                        collideRectEnemy.Y += 1;
                        vectors.Add(( -(i - 1), "y" ));
                        break;
                    }
                }
                
            }
        }
        
        public virtual void Move(Player player, RectangleF rectEnemy, RectangleF collideRectEnemy)
        {
            vector.x = 0; vector.y = 0;
            directs.right = false;
            directs.left = false;
            directs.up = false;
            directs.down = false;
            
            if (chaseDirection == "up")
            {
                vector.y -= enemySpeed;
            }
            else if (chaseDirection == "down")
            {
                vector.y += enemySpeed;
            }
            else if (chaseDirection == "y_tp") //TODO: bug
            {
                vector.y -= (collideRectEnemy.Y + collideRectEnemy.Height) -
                            (player.draw.rect.Y + player.draw.rect.Height);
            }
            
            
            else if (chaseDirection == "left")
            {
                vector.x -= enemySpeed;
            }
            else if (chaseDirection == "right")
            {
                vector.x += enemySpeed;
            }
            else if (chaseDirection == "x_tp")
            {
                vector.x -= (rectEnemy.X - (player.draw.rect.Width - rectEnemy.Width) - player.draw.rect.X);
            }
        }

        public virtual void AddChaseInHistory(RectangleF rectEnemy)
        {
            if (historyDirection.Count != 5)
            {
                historyDirection.Add((chaseDirection, rectEnemy));
            }

            else
            {
                historyDirection.Insert(0, (chaseDirection, rectEnemy));
                historyDirection.RemoveAt(5);
            }
        }

        public virtual bool IsMoved()
        {
            return historyDirection.All(item => item.rect == historyDirection[0].rect);
        }

        public virtual void ChasePriorityCalculate()
        {
            if (historyDirection.Count != 5) return;

            string direct = historyDirection[0].direct;
            if (direct == "none") return;

            if (catchPlayerOnAxis)
            {
                if (chasePriority == 'x')
                    chasePriority = 'y';
                
                else
                    chasePriority = 'x';

                catchPlayerOnAxis = false;
            }
            
            else if (historyDirection.Count(item => item.direct == direct) == 5 && IsMoved())
            {
                if (chasePriority == 'x')
                    chasePriority = 'y';
                
                else
                    chasePriority = 'x';
            }
        }

        protected virtual void ChaseForY(Player player, RectangleF rectEnemy, float deltaY)
        {
            if (Math.Abs(rectEnemy.Y - deltaY - player.draw.rect.Y) <= enemySpeed)
                chaseDirection = "y_tp";
                
            else if (rectEnemy.Y - enemySpeed - deltaY > player.draw.rect.Y)
                chaseDirection = "up";
                
            else if (rectEnemy.Y - enemySpeed - deltaY < player.draw.rect.Y)
                chaseDirection = "down";
        }

        protected virtual void ChaseForX(Player player, RectangleF rectEnemy, float deltaX)
        {
            if (Math.Abs(rectEnemy.X - deltaX - player.draw.rect.X) <= enemySpeed)
                chaseDirection = "x_tp";
                
            else if (rectEnemy.X - enemySpeed - deltaX > player.draw.rect.X)
            {
                chaseDirection = "left";
            }
                
            else if (rectEnemy.X - enemySpeed - deltaX < player.draw.rect.X)
            {
                chaseDirection = "right";
            }
        }
        
        public virtual void ChaseCalculate(Player player, RectangleF rectEnemy)
        {
            float deltaY = player.draw.rect.Height - rectEnemy.Height;
            float deltaX = player.draw.rect.Width - rectEnemy.Width;
            chaseDirection = "none";

            if (chasePriority == 'y' && rectEnemy.Y - deltaY != player.draw.rect.Y) {
                ChaseForY(player, rectEnemy, deltaY);
                catchPlayerOnAxis = false;
            }
            
            else if (chasePriority == 'x' && rectEnemy.X - deltaX != player.draw.rect.X) {
                ChaseForX(player, rectEnemy, deltaX);
                catchPlayerOnAxis = false;
            }
            
            else if (rectEnemy.Y - deltaY == player.draw.rect.Y) {
                if (!catchPlayerOnAxis)
                {
                    catchPlayerOnAxis = true;
                    return;
                }
                ChaseForX(player, rectEnemy, deltaX);
            }
            
            else if (rectEnemy.X - deltaX == player.draw.rect.X) {
                if (!catchPlayerOnAxis)
                {
                    catchPlayerOnAxis = true;
                    return;
                }
                ChaseForY(player, rectEnemy, deltaY);
            }
        }
    }

    public class Tool
    {
        public int animationFrame;
        public string oldDirect;

        public virtual bool SeePlayer(RectangleF rectVisionArea, Player player)
        {
            return rectVisionArea.Intersects(player.draw.rect);
        }

        public virtual void Stay(ref (bool, bool, bool, bool) directs, ref string chaseDirection)
        {
            directs.Item1 = false;
            directs.Item2 = false;
            directs.Item3 = false;
            directs.Item4 = false;
            chaseDirection = "none";
        }
    }

    public class Debugs
    {
        public ShapeBatch ShapeBatch;
        
        public virtual void DrawHitbox(RectangleF collideRect, Camera camera)
        {
            ShapeBatch.Begin();
            ShapeBatch.DrawRectangle(
                new Vector2(collideRect.X - camera.x, collideRect.Y - camera.y),
                new Vector2(collideRect.Width, collideRect.Height),
                new Color(0, 0, 0, 0),
                Color.Brown,
                3f);
            ShapeBatch.End();
        }

        public virtual void DrawVisionArea(RectangleF rectVisionArea, Camera camera)
        {
            ShapeBatch.Begin();
            ShapeBatch.DrawRectangle(
                new Vector2(rectVisionArea.X - camera.x, rectVisionArea.Y - camera.y),
                new Vector2(rectVisionArea.Width, rectVisionArea.Height),
                new Color(0, 0, 0, 0),
                Color.Blue,
                3f);
            ShapeBatch.End();
        }
    }
}

public class Rat : Enemy
{
    new public Draw draw;
    new public Control control;
    new public Tool tool;
    new public Debugs debugs;

    public Rat(GraphicsDevice device, ContentManager content, int positionX=0, int positionY=0)
    {
        draw = new Draw(positionX, positionY);
        control = new Control();
        tool = new Tool();
        debugs = new Debugs();

        draw.Load(device, content);
        debugs.ShapeBatch = new ShapeBatch(device, content);
    }
    
    new public class Draw : Enemy.Draw
    {
        public Draw(int positionX=0, int positionY=0)
        {
            int x = positionX;
            int y = positionY;
            int width = 100;
            int height = 112;
            int deltaHeight = 50;

            float multipleVisionArea = 4;
            
            rect = new RectangleF(x, y, width, height);
            collideRect = new RectangleF(x, y + deltaHeight, width, height - deltaHeight);
            rectVisionArea = new RectangleF(
                x - width * multipleVisionArea,
                y - height * multipleVisionArea,
                width * multipleVisionArea * 2,
                height * multipleVisionArea * 2);
        }
        
        public void Mainloop(Rat rat, SpriteBatch surface, Camera camera)
        {
            DrawSprite(rat.draw.rect, surface, camera);
        }

        public void Load(GraphicsDevice device, ContentManager content)
        {
            walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_1"));
            walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_2"));
            walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_3"));
            
            walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_1"));
            walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_2"));
            walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_3"));

            sprite = walkLeft[0];
        }
    }
    
    new public class Control : Enemy.Control
    {
        public Control()
        {
            enemySpeed = 3f;
        }
        
        public void Mainloop(Rat rat, Player player)
        {
            if (rat.tool.SeePlayer(rat.draw.rectVisionArea, player))
            {
                AddChaseInHistory(rat.draw.rect);
                ChasePriorityCalculate();
                ChaseCalculate(player, rat.draw.rect);
                Move(player, rat.draw.rect, rat.draw.collideRect);
            }

            else
            {
                rat.tool.Stay(ref rat.control.directs, ref rat.control.chaseDirection);
            }
            
        }
    }

    new public class Tool : Enemy.Tool
    {
        public void AnimationMove(Rat rat, Player player)
        {
            float deltaX = player.draw.rect.Width - rat.draw.rect.Width;
            int countAnimationFrame = 12;

            if (animationFrame + 1 >= countAnimationFrame * rat.draw.walkLeft.Count) animationFrame = 0;

            if (rat.control.directs.up) {
                if (rat.draw.rect.X - deltaX >= player.draw.rect.X) {
                    rat.draw.sprite = rat.draw.walkLeft[animationFrame / countAnimationFrame];
                    animationFrame++;
                }
                
                else if (rat.draw.rect.X - deltaX < player.draw.rect.X) {
                    rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                    animationFrame++;
                }
                
                oldDirect = "up";
            }
            
            else if (rat.control.directs.down) {
                if (rat.draw.rect.X - deltaX >= player.draw.rect.X) {
                    rat.draw.sprite = rat.draw.walkLeft[animationFrame / countAnimationFrame];
                    animationFrame++;
                }
                
                else if (rat.draw.rect.X - deltaX < player.draw.rect.X) {
                    rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                    animationFrame++;
                }

                oldDirect = "down";
            }
            
            else if (rat.control.directs.right) {
                rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                animationFrame++;

                oldDirect = "right";
            }
            
            else if (rat.control.directs.left) {
                rat.draw.sprite = rat.draw.walkLeft[animationFrame / countAnimationFrame];
                animationFrame++;

                oldDirect = "left";
            }

            else
            {
                animationFrame = 0;
                
                if (rat.draw.rect.X - deltaX >= player.draw.rect.X) 
                    rat.draw.sprite = rat.draw.walkLeft[0];
                
                else if (rat.draw.rect.X - deltaX < player.draw.rect.X) 
                    rat.draw.sprite = rat.draw.walkRight[0];
            }
        }
    }

    new public class Debugs : Enemy.Debugs
    {
        
    }
}
