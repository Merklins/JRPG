using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RPG_GAME.Core;
using RPG_GAME.MapObjects;
using System.Collections.Generic;
using System.Diagnostics;

namespace RPG_GAME.Components;

public class PlayerController : Component
{
    private AnimationRenderer _renderer;
    private Rigidbody _rb;

    public float Speed = 2f;
    public float SprintSpeed = 10f;

    public override void OnLoad()
    {
        _renderer = Entity.GetComponent<AnimationRenderer>();
        _rb = Entity.GetComponent<Rigidbody>();

        Trans.Position = new Vector2(2100, 1600);
        Game.Services.GetService<Camera>().Target = Trans;
    }

    public override void OnUpdate()
    {
        var kb = Keyboard.GetState();

        var delta = new Vector2(GetAxis(Keys.D, Keys.A), GetAxis(Keys.S, Keys.W));
        if (delta != Vector2.Zero) delta.Normalize();

        var key = delta.X < 0 ? "left"
            : delta.X > 0 ? "right"
            : delta.Y < 0 ? "up" : "down";

        bool sprint = kb.IsKeyDown(Keys.LeftShift);
        var delay = delta == Vector2.Zero ? int.MaxValue : sprint ? 10 : 16;

        var speed = sprint ? SprintSpeed : Speed;
        _rb.Velocity = delta * speed;
        _renderer.SetAnimation(key, delay);
    }

    private static float GetAxis(Keys positive, Keys negative)
    {
        var kb = Keyboard.GetState();
        float x = 0;
        if (kb.IsKeyDown(positive)) x += 1f;
        if (kb.IsKeyDown(negative)) x -= 1f;
        return x;
    }
}