using Microsoft.Xna.Framework.Graphics;
using TiledCS;

namespace RPG_GAME.MapObjects;

public class Rock : BaseTiledObject
{
    public Rock(TiledObject source, Texture2D texture, float oscillationSpeed, float amplitude, float phaseShift,
        float dampingCoefficient, float timeToRandomChangeParameters) : 
        base(source, texture, oscillationSpeed, amplitude,
            phaseShift, dampingCoefficient, timeToRandomChangeParameters)
    {
        
    }
}