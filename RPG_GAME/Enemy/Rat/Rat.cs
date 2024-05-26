using Apos.Shapes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_GAME.Enemy.Rat;

public class Rat : BaseEnemy
{
    new public Draw draw;
    new public Control control;
    new public Tool tool;
    new public Debugs debugs;

    public Rat(GraphicsDevice device, ContentManager content, int positionX = 0, int positionY = 0)
    {
        // draw = new Draw(positionX, positionY);
        control = new Control();
        tool = new Tool();
        debugs = new Debugs();

        // draw.Load(device, content);
        debugs.ShapeBatch = new ShapeBatch(device, content);
    }

    
    new public class Control : BaseEnemy.Control
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

    new public class Tool : BaseEnemy.Tool
    {
        public void AnimationMove(Rat rat, Player player)
        {
            float deltaX = player.draw.rect.Width - rat.draw.rect.Width;
            int countAnimationFrame = 12;

            if (animationFrame + 1 >= countAnimationFrame * rat.draw.walkLeft.Count) animationFrame = 0;

            if (rat.control.directs.up)
            {
                if (rat.draw.rect.X - deltaX >= player.draw.rect.X)
                {
                    rat.draw.sprite = rat.draw.walkLeft[animationFrame / countAnimationFrame];
                    animationFrame++;
                }

                else if (rat.draw.rect.X - deltaX < player.draw.rect.X)
                {
                    rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                    animationFrame++;
                }

                oldDirect = "up";
            }

            else if (rat.control.directs.down)
            {
                if (rat.draw.rect.X - deltaX >= player.draw.rect.X)
                {
                    rat.draw.sprite = rat.draw.walkLeft[animationFrame / countAnimationFrame];
                    animationFrame++;
                }

                else if (rat.draw.rect.X - deltaX < player.draw.rect.X)
                {
                    rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                    animationFrame++;
                }

                oldDirect = "down";
            }

            else if (rat.control.directs.right)
            {
                rat.draw.sprite = rat.draw.walkRight[animationFrame / countAnimationFrame];
                animationFrame++;

                oldDirect = "right";
            }

            else if (rat.control.directs.left)
            {
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

    new public class Debugs : BaseEnemy.Debugs
    {

    }
}
