using System;
using Apos.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPG_GAME.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RPG_GAME.Components;

public class AnimationRenderer : Component
{
    private readonly string _assetPath;
    private Dictionary<string, Texture2D[]> _assets = new();
    private Texture2D[] _currentAnimation;
    private int _currentIndex = 0;
    private int _delay;
    private int _frame;

    public Color color = Color.White;

    public Texture2D Texture => _currentAnimation[_currentIndex];

    public AnimationRenderer(string assetPath)
    {
        _assetPath = assetPath;
    }

    public void SetAnimation(string key, int delay)
    {
        var nextAnimation = _assets[key];
        _delay = delay;

        if (ReferenceEquals(nextAnimation, _currentAnimation)) return;

        _currentAnimation = nextAnimation;
        _currentIndex = 0;
        _frame = 0;
        Trans.Size = Texture.Bounds.Size.ToVector2();
        Trans.Pivot = Trans.Size / 2f;
    }

    public override void OnLoad()
    {
        var animationPath = Path.Combine(Game.Content.RootDirectory, _assetPath + ".json");
        var json = File.ReadAllText(animationPath);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json);

        var spriteDir = Path.GetDirectoryName(_assetPath);

        foreach (var (key, values) in dict)
        {
            var textures = new Texture2D[values.Length];
            int i = 0;
            foreach (var spriteName in values)
                textures[i++] = Game.Content.Load<Texture2D>(Path.Combine(spriteDir, spriteName));

            _assets.Add(key, textures);
        }
    }

    public override void OnUpdate()
    {
        if (_currentAnimation == null) return;

        if (_frame++ >= _delay)
        {
            _currentIndex = (_currentIndex + 1) % _currentAnimation.Length;
            _frame = 0;
        }
    }

    public override void OnDraw()
    {
        if (_currentAnimation == null) return;

        var batch = Game.Services.GetService<SpriteBatch>();
        var camera = Game.Services.GetService<Camera>();
        batch.Begin(transformMatrix: camera.World);
        var texture = _currentAnimation[_currentIndex];

        var source = new Rectangle(0, 0, texture.Width, texture.Height);
        batch.Draw(texture, Trans.Rect, source, color);

        batch.End();
    }


    //public void Mainloop(Player player, SpriteBatch surface)
    //{
    //	AnimationMove(player);
    //	DrawSprite(player, surface);
    //}

    //public int animationFrame = 0;

    //public List<Texture2D> walkDown = new List<Texture2D>();
    //public List<Texture2D> walkUp = new List<Texture2D>();
    //public List<Texture2D> walkRight = new List<Texture2D>();
    //public List<Texture2D> walkLeft = new List<Texture2D>();

    //public Texture2D sprite;
    //public RectangleF rect;
    //public RectangleF rectCollide;
    //public RectangleF rectCollideConst;
    //private Vector2 positionScreen;
    //public ShapeBatch ShapeBatch;

    //public void OnStart2()
    //{
    //	var content = Game.Content;

    //	List<string> direction_name = new List<string>() { "left", "right", "up", "down" };

    //	foreach (string direction in direction_name)
    //	{
    //		for (int i = 1; i <= 3; i++)
    //		{
    //			switch (direction)
    //			{
    //				case "left":
    //					{
    //						walkLeft.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
    //						break;
    //					}
    //				case "right":
    //					{
    //						walkRight.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
    //						break;
    //					}
    //				case "up":
    //					{
    //						walkUp.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
    //						break;
    //					}
    //				case "down":
    //					{
    //						walkDown.Add(content.Load<Texture2D>($"Player\\move_animation\\Ninja_{direction}{i}"));
    //						break;
    //					}
    //			}
    //		}
    //	}
    //}

    //public void LoadDrawPlayer(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
    //{
    //	float xOffset = -0;
    //	float yOffset = -140;
    //	float widthOffset = 0;
    //	float heightOffset = 70;

    //	player.content = content;
    //	player.device = device;
    //	this.ShapeBatch = new ShapeBatch(device, content);
    //	sprite = content.Load<Texture2D>("Player\\Ninja_down1");
    //	positionScreen = new Vector2((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2);
    //	rect = new RectangleF((manager.PreferredBackBufferWidth - sprite.Width) / 2, (manager.PreferredBackBufferHeight - sprite.Height) / 2, sprite.Width, sprite.Height);
    //	rectCollide = new RectangleF(
    //		(manager.PreferredBackBufferWidth - sprite.Width - xOffset) / 2 + 5,
    //		(manager.PreferredBackBufferHeight - sprite.Height - yOffset) / 2,
    //		sprite.Width - widthOffset - 10,
    //		sprite.Height - heightOffset
    //	);
    //	rectCollideConst = new RectangleF(
    //		(manager.PreferredBackBufferWidth - sprite.Width - xOffset) / 2 + 5,
    //		(manager.PreferredBackBufferHeight - sprite.Height - yOffset) / 2,
    //		sprite.Width - widthOffset - 10,
    //		sprite.Height - heightOffset
    //	);
    //}

    //public void Load(Player player, ContentManager content, GraphicsDevice device, GraphicsDeviceManager manager)
    //{
    //	player.draw.LoadDrawPlayer(player, content, device, manager);
    //	player.draw.LoadSpritePlayer(player, content);
    //	player.ui.Load(content, device);
    //}

    //public void DrawSprite(Player player, SpriteBatch surface)
    //{
    //	surface.Draw(sprite, positionScreen, Color.White);
    //}

    //public void AnimationMove(Player player)
    //{
    //	int count_animation_frame = 16;
    //	if (player.control.sprinting) count_animation_frame = 10;

    //	if (animationFrame + 1 >= count_animation_frame * walkDown.Count) animationFrame = 0;

    //	if (player.control.up)
    //	{
    //		if (!player.control.move)
    //		{
    //			sprite = walkUp[0];
    //			animationFrame = 0;
    //		}

    //		else
    //		{
    //			sprite = walkUp[animationFrame / count_animation_frame];
    //			animationFrame++;
    //		}
    //		player.control.oldDirect = "up";
    //	}

    //	else if (player.control.down)
    //	{
    //		if (!player.control.move)
    //		{
    //			sprite = walkDown[0];
    //			animationFrame = 0;
    //		}
    //		else
    //		{
    //			sprite = walkDown[animationFrame / count_animation_frame];
    //			animationFrame++;
    //		}

    //		player.control.oldDirect = "down";
    //	}

    //	else if (player.control.left)
    //	{
    //		if (!player.control.move)
    //		{
    //			sprite = walkLeft[0];
    //			animationFrame = 0;
    //		}
    //		else
    //		{
    //			sprite = walkLeft[animationFrame / count_animation_frame];
    //			animationFrame++;
    //		}

    //		player.control.oldDirect = "left";
    //	}

    //	else if (player.control.right)
    //	{
    //		if (!player.control.move)
    //		{
    //			sprite = walkRight[0];
    //			animationFrame = 0;
    //		}
    //		else
    //		{
    //			sprite = walkRight[animationFrame / count_animation_frame];
    //			animationFrame++;
    //		}

    //		player.control.oldDirect = "right";
    //	}

    //	else
    //	{
    //		switch (player.control.oldDirect)
    //		{
    //			case "up":
    //				{
    //					sprite = walkUp[0];
    //					break;
    //				}

    //			case "down":
    //				{
    //					sprite = walkDown[0];
    //					break;
    //				}

    //			case "right":
    //				{
    //					sprite = walkRight[0];
    //					break;
    //				}

    //			case "left":
    //				{
    //					sprite = walkLeft[0];
    //					break;
    //				}
    //		}
    //	}
    //}
}