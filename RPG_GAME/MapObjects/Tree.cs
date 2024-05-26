using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace RPG_GAME.MapObjects;

public class Tree : BaseTiledObject
{
    public Tree(
        TiledObject source, Texture2D texture, float oscillationSpeed, float amplitude, float phaseShift,
        float dampingCoefficient, float timeToRandomChangeParameters) : 
        base(source, texture, oscillationSpeed, amplitude,
            phaseShift, dampingCoefficient, timeToRandomChangeParameters) { }

    public float SmoothStepFunction(float scaleY)
    {
        if (scaleY < 0.15f)
            return 0f; // Параметр меньше 0.15, возвращаем 0
        else
        {
            return (scaleY - 0.15f) / (0.35f - 0.15f);
        }
    }


    public override void DrawWithBaseWind(SpriteBatch spriteBatch, Camera camera, BaseWind wind)
    {
        if (!PlayerSeeObject(camera)) return;
        
        for (int y = 0; y < Texture.Height; y++)
        {
            // Уменьшение колебаний к основанию цветка
            float scaleY = (Texture.Height - y) / (float)Texture.Height;
            float localWindStrength = wind.windOffset * scaleY;

            if (scaleY <= 0.35f) localWindStrength *= SmoothStepFunction(scaleY);

            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X + localWindStrength - camera.x, Position.Y + y - camera.y),
                new Rectangle(0, y, Texture.Width, 1),
                Color.White
            );
        }
    }

    public override void DrawWithSpecificWind(SpriteBatch spriteBatch, Camera camera)
    {
        //if (!PlayerSeeObject(camera)) return;
        
        for (int y = 0; y < Texture.Height; y++)
        {
            // Уменьшение колебаний к основанию цветка
            float scaleY = (Texture.Height - y) / (float)Texture.Height;
            float localWindStrength = windOffset * scaleY;

            if (scaleY <= 0.35f) localWindStrength *= SmoothStepFunction(scaleY);

            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X + localWindStrength - camera.x, Position.Y + y - camera.y),
                new Rectangle(0, y, Texture.Width, 1),
                Color.White
            );
        }
    }
}