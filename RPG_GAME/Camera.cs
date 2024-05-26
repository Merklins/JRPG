using Microsoft.Xna.Framework;
using RPG_GAME.Components;
using System.Diagnostics;

namespace RPG_GAME;

public class Camera
{
    private readonly Game _game;
    private Matrix _world;

    public float x = 0;
    public float y = 0;

    public Camera(Game game)
    {
        _game = game;
    }

    public RectTransform Target { get; set; }
    public Vector2 Pos => new Vector2(x, y);
    public Matrix World => _world;

    public void Update()
    {
        if (Target == null) return;

        x = Target.Position.X;
        y = Target.Position.Y;

        var vp = _game.GraphicsDevice.Viewport;
        var center = new Vector2(vp.Width / 2, vp.Height / 2);
        var pos = Pos - center;
        pos.Round();
        _world = Matrix.CreateWorld(new Vector3(-pos, 0f), Vector3.Forward, Vector3.Up);
    }

    public void Mainloop(Player player)
    {
        Move(player);
    }

    public void Move(Player player)
    {
        this.x += player.control.vector.x;
        this.y += player.control.vector.y;
    }

    public void UndoMove(Player player)
    {
        if (player.control.undoVector.direction == "none") return;

        if (player.control.undoVector.direction == "right")
            x -= player.control.undoVector.value;
        else if (player.control.undoVector.direction == "left")
            x += player.control.undoVector.value;
        else if (player.control.undoVector.direction == "down")
            y -= player.control.undoVector.value;
        else if (player.control.undoVector.direction == "up")
            y += player.control.undoVector.value;
    }
}