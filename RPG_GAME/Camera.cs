using System.Diagnostics;

namespace RPG_GAME;

public class Camera
{
    public float x = 0;
    public float y = 0;

    public void mainloop(Player player)
    {
        move(player);
    }

    public void move(Player player)
    {
        this.x += player.control.vector.x;
        this.y += player.control.vector.y;
    }

    public void undo_move(Player player)
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