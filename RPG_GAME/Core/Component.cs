using Microsoft.Xna.Framework;
using RPG_GAME.Components;

namespace RPG_GAME.Core;

public abstract class Component
{
    public Entity Entity { get; internal set; }

    public Game Game => Entity.Game;

    public RectTransform Trans => Entity.Trans;

    public virtual void OnLoad() { }
    public virtual void OnUpdate() { }
    public virtual void OnDraw() { }
    public virtual void OnGizmos() { }
    public virtual void OnImGui() { }
}
