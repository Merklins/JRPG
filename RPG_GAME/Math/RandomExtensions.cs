using System;
using Microsoft.Xna.Framework;

namespace RPG_GAME;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, float minValue, float maxValue)
    {
        return MathHelper.Lerp(minValue, maxValue, random.NextSingle());
    }
}