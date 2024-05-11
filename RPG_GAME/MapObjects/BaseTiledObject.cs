using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using TiledCS;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace RPG_GAME.MapObjects;

public abstract class BaseTiledObject : TiledObject
{
    public static string[] noWindSensetiveObject = new string[] { "Rock" };
    public static float increareRangeSeePixel = 200;
    public static bool useBaseWind = false;
    
    public int id;
    public string name;
    public string type;
    public string @class;
    public float x;
    public float y;
    public float rotation;
    public float width;
    public float height;
    public int gid;
    public byte dataRotationFlag;
    
    public TiledProperty[] properties;
    public TiledPolygon polygon;
    public TiledPoint point;
    public TiledEllipse ellipse;
    
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public float OscillationSpeed { get; set; }
    public float OscillationSpeedConst { get; set; }
    public float Amplitude { get; set; }
    public float AmplitudeConst { get; set; }
    public float PhaseShift { get; set; }
    public float DampingCoefficient { get; set; }
    
    public float windOffset = 0f;
    public float time = 0f;
    public float timeToRandomChangeParametersConst;
    public float timeToRandomChangeParameters = 0f;
    public float lastWindOffset = 0f;
    
    public BaseTiledObject(TiledObject source, Texture2D sprite, float oscillationSpeed, float amplitude, float phaseShift,
        float dampingCoefficient, float timeToRandomChangeParameters)
    {
        this.id = source.id;
        this.name = source.name;
        this.type = source.type;
        this.@class = source.@class;
        this.x = source.x;
        this.y = source.y;
        this.rotation = source.rotation;
        this.width = source.width;
        this.height = source.height;
        this.gid = source.gid;
        this.dataRotationFlag = source.dataRotationFlag;

        this.properties = source.properties;
        this.polygon = source.polygon;
        this.point = source.point;
        this.ellipse = source.ellipse;
        
        this.Texture = sprite;
        this.Position = new Vector2(source.x, source.y - source.height);
        
        this.OscillationSpeed = oscillationSpeed;
        this.OscillationSpeedConst = oscillationSpeed;
        this.Amplitude = amplitude;
        this.AmplitudeConst = amplitude;
        this.PhaseShift = phaseShift;
        this.DampingCoefficient = dampingCoefficient;
        this.timeToRandomChangeParametersConst = timeToRandomChangeParameters;
    }

    public virtual void Update(GameTime gameTime)
    {
        if (timeToRandomChangeParametersConst != 0f)
            timeToRandomChangeParameters += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        time += (float)gameTime.ElapsedGameTime.TotalSeconds * OscillationSpeed;

        // В начале вычисляем сырое смещение ветра без учета замедления
        float rawWindOffset = Amplitude * (float)Math.Sin(time + PhaseShift);

        // Расчет коэффициента замедления отклонения относительно близости текущего смещения к максимальной амплитуде
        float damping = 1f - (Math.Abs(rawWindOffset) / Amplitude) * DampingCoefficient;
        damping = Math.Clamp(damping, 0f, 1f); // убедимся, что замедление не выходит за пределы [0, 1]

        // Применяем замедление к смещению ветра
        windOffset = rawWindOffset * damping;

        // объект примерно выравнялся, чтобы при смене амплитуды не было резкого скачка
        if ( !(-0.1f <= windOffset && windOffset <= 0.1f) ) return;
        
        if ( !(timeToRandomChangeParameters >= timeToRandomChangeParametersConst
               && timeToRandomChangeParametersConst != 0) ) return;
            
        float multiplyAmplitude = Random.Shared.NextFloat(0.75f, 1.5f);
        float multiplyOscillationSpeed = Random.Shared.NextFloat(0.75f, 1.5f);
        Amplitude = AmplitudeConst * multiplyAmplitude;
        OscillationSpeed = OscillationSpeedConst * multiplyOscillationSpeed;
        DampingCoefficient = Random.Shared.NextFloat(0f, 1f);
        timeToRandomChangeParameters = 0f;
    }

    public virtual bool PlayerSeeObject(Camera camera)
    {
        return new RectangleF(Position.X, Position.Y, Texture.Width, Texture.Height).Intersects(
            new RectangleF(camera.x - increareRangeSeePixel, camera.y - increareRangeSeePixel,
                1280 + increareRangeSeePixel * 2, 720 + increareRangeSeePixel * 2));
    }

    public virtual void DrawWithBaseWind(SpriteBatch spriteBatch, Camera camera, BaseWind wind)
    {
        if (!PlayerSeeObject(camera)) return;
        
        for (int y = 0; y < Texture.Height; y++)
        {
            // Уменьшение колебаний к основанию цветка
            float scaleY = (Texture.Height - y) / (float)Texture.Height;
            float localWindStrength = wind.windOffset * scaleY;

            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X + localWindStrength - camera.x, Position.Y + y - camera.y),
                new Rectangle(0, y, Texture.Width, 1),
                Color.White
            );
        }
    }

    public virtual void DrawWithoutWind(SpriteBatch spriteBatch, Camera camera)
    {
        if (!PlayerSeeObject(camera)) return;
        
        spriteBatch.Draw(Texture, new Vector2(x - camera.x, y - camera.y - Texture.Height), Color.White);
    }

    public virtual void DrawWithSpecificWind(SpriteBatch spriteBatch, Camera camera)
    {
        if (!PlayerSeeObject(camera)) return;
        
        for (int y = 0; y < Texture.Height; y++)
        {
            // Уменьшение колебаний к основанию цветка
            float scaleY = (Texture.Height - y) / (float)Texture.Height;
            float localWindStrength = windOffset * scaleY;

            spriteBatch.Draw(
                Texture,
                new Vector2(Position.X + localWindStrength - camera.x, Position.Y + y - camera.y),
                new Rectangle(0, y, Texture.Width, 1),
                Color.White
            );
        }
    }
}
