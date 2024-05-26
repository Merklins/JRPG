using Microsoft.Xna.Framework;
using RPG_GAME.Core;

namespace RPG_GAME.Components;

public class RectTransform : Component
{
    public Vector2 Position;
    public Vector2 Size;
    public Vector2 Pivot;

    // public Rectangle Rect => new((Position - Pivot).ToPoint(), Size.ToPoint());
    public Rectangle Rect => new(Position.ToPoint(), Size.ToPoint());
}


