using Apos.Shapes;
using ImGuiNET;
using Microsoft.Xna.Framework;
using RPG_GAME;
using RPG_GAME.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Rigidbody : Component
{
    public delegate void CollisionCallback(Entity src, Rectangle other);
    
    public Vector2 Velocity;

    private DrawRectCommand _rectDraw;
    private DrawLineCommand _lineDraw;
    private World _world;
    
    public override void OnLoad()
    {
        _rectDraw = Game.Services.GetService<DrawRectCommand>();
        _lineDraw = Game.Services.GetService<DrawLineCommand>();
        _world = Game.Services.GetService<World>();
    }

    private void Collide(ref Rectangle rb, Rectangle other)
    {
        if (!rb.Intersects(other))
        {
            _rectDraw.Add((other, Color.Orange));
            return;
        } 
        
        _rectDraw.Add((other, Color.Yellow));

        var x = GetPushOut(rb.Left, rb.Right, other.Left, other.Right);
        var y = GetPushOut(rb.Top, rb.Bottom, other.Top, other.Bottom);

        if (x != y && y != 0)
        {
            if (Math.Abs(x) <= Math.Abs(y)) y = 0;
            else x = 0;
        }

        rb.X += x;
        rb.Y += y;
        
        var c = rb.Center.ToVector2();
        _lineDraw.Add((c, c + new Vector2(x, 0), Color.Red));
        _lineDraw.Add((c, c + new Vector2(0, y), Color.Red));
    }
    
    public override void OnUpdate()
    {
        // if (Velocity == Vector2.Zero) return;
        var sw = new Stopwatch();
        sw.Start();

        var rectPlayer = Trans.Rect;
        var endRect = rectPlayer;
        endRect.Location = (endRect.Location.ToVector2() + Velocity).ToPoint();
        
        _rectDraw.Add((rectPlayer, Color.LightGreen));

        foreach (RectangleF rectTileF in _world.borderTile)
        {
            var rectTile = new Rectangle(
                (int)rectTileF.X,
                (int)rectTileF.Y, 
                (int)rectTileF.Width,
                (int)rectTileF.Height
            );
            
            Collide(ref endRect, rectTile);
        }

        Trans.Position = endRect.Location.ToVector2();
        
        sw.Stop();
        Console.WriteLine(sw.Elapsed.TotalMilliseconds);
    }

    private static Point GetPushOut(Point p, Rectangle rect)
    {
        var x = GetPushOut(p.X, rect.Left, rect.Right);
        var y = GetPushOut(p.Y, rect.Top, rect.Bottom);
        return new Point { X = x, Y = y };
    }
    
    private static int GetPushOut(int aMin, int aMax, int bMin, int bMax)
    {
        if (aMax <= bMin || aMin >= bMax) return 0;

        if (aMin >= bMin && aMax <= bMax)
        {
            int left = aMin - bMin;
            int right = bMax - aMax;
            return left <= right ? bMin - aMax : bMax - aMin;
        }

        if (aMin <= bMin && aMax >= bMax)
        {
            int left = bMin - aMin;
            int right = aMax - bMax;
            return left <= right ? bMax - aMin : bMin - aMax;
        }
        
        if (aMin < bMin) return bMin - aMax;

        if (aMax > bMax) return bMax - aMin;

        return 0;
    }

    private static int GetPushOut(int value, int min, int max)
    {
        if (value <= min || value >= max) return 0;
        int minOffset = value - min;
        int maxOffset = max - value;
        return minOffset < maxOffset ? -minOffset : maxOffset;
    }
}