using Microsoft.Xna.Framework;
using RPG_GAME.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace RPG_GAME.Core;

[Serializable]
public class Entity
{
    private readonly List<Component> _components = new();

    public Entity(Game game, string name = "untitled")
    {
        Game = game;
        Name = name;
        Trans = new RectTransform();
        AddComponent(Trans);
    }

	public Game Game { get; }
	public string Name { get; }
    public RectTransform Trans { get; }


	public Entity AddComponent(Component component)
    {
        component.Entity = this;
        _components.Add(component);
        return this;
    }

    public T GetComponent<T>() where T : Component
    {
        return _components.FirstOrDefault(x => x is T) as T;
    }

    public void Load()
    {
        foreach (var item in _components) item.OnLoad();
    }

    public void Update()
    {
        foreach (var item in _components) item.OnUpdate();
    }

    public void Draw()
    {
        foreach (var item in _components) item.OnDraw();
    }

    public void Gizmos()
    {
        foreach (var item in _components) item.OnGizmos();
    }

    public void ImGui()
    {
        foreach (var item in _components) item.OnImGui();
    }
}
