using Microsoft.Xna.Framework;

namespace RPG_GAME;

public class TreeWind : BaseWind
{
    public TreeWind(
        float oscillationSpeed, float amplitude, float phaseShift,
        float dampingCoefficient, float timeToRandomChangeAmplitude) :
        base(oscillationSpeed, amplitude, phaseShift, dampingCoefficient, timeToRandomChangeAmplitude)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        
    }
}