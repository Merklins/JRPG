//using RPG_GAME.Enemy.Rat;
//using RPG_GAME.Enemy;

//new public class Draw : BaseEnemy.Draw
//{
//    public Draw(int positionX = 0, int positionY = 0)
//    {
//        int x = positionX;
//        int y = positionY;
//        int width = 100;
//        int height = 112;
//        int deltaHeight = 50;

//        float multipleVisionArea = 4;

//        rect = new RectangleF(x, y, width, height);
//        collideRect = new RectangleF(x, y + deltaHeight, width, height - deltaHeight);
//        rectVisionArea = new RectangleF(
//            x - width * multipleVisionArea,
//            y - height * multipleVisionArea,
//            width * multipleVisionArea * 2,
//            height * multipleVisionArea * 2);
//    }

//    public void Mainloop(Rat rat, SpriteBatch surface, Camera camera)
//    {
//        DrawSprite(rat.draw.rect, surface, camera);
//    }

//    public void Load(GraphicsDevice device, ContentManager content)
//    {
//        walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_1"));
//        walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_2"));
//        walkLeft.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_left_3"));

//        walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_1"));
//        walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_2"));
//        walkRight.Add(content.Load<Texture2D>("Enemy\\rat\\Rat_right_3"));

//        sprite = walkLeft[0];
//    }
//}