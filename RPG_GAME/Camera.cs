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
        if (player.control.undo_vector.direction == "none") return;

        if (player.control.undo_vector.direction == "right")
            x -= player.control.undo_vector.value;
        else if (player.control.undo_vector.direction == "left")
            x += player.control.undo_vector.value;
        else if (player.control.undo_vector.direction == "down")
            y -= player.control.undo_vector.value;
        else if (player.control.undo_vector.direction == "up")
            y += player.control.undo_vector.value;
    }
}