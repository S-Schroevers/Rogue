﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class GameObjectList : GameObject
{
    protected List<GameObject> children;
    protected List<GameObject> toRemove;

    public GameObjectList(int layer = 0, string id = "") : base(layer, id)
    {
        children = new List<GameObject>();
        toRemove = new List<GameObject>();
    }

    public List<GameObject> Children
    {
        get { return children; }
    }

    public void Add(GameObject obj)
    {
        obj.Parent = this;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i].Layer > obj.Layer)
            {
                children.Insert(i, obj);
                return;
            }
        }
        children.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        toRemove.Add(obj);
        obj.Parent = null;
    }

    public GameObject Find(Func<GameObject, bool> del)
    {
        foreach (GameObject obj in children)
        {
            if (del.Invoke(obj))
            {
                return obj;
            }
            if (obj is GameObjectList)
            {
                GameObjectList objList = obj as GameObjectList;
                GameObject subObj = objList.Find(del);
                if (subObj != null)
                {
                    return subObj;
                }
            }
            if (obj is GameObjectGrid)
            {
                GameObjectGrid objGrid = obj as GameObjectGrid;
                GameObject subObj = objGrid.Find(del);
                if (subObj != null)
                {
                    return subObj;
                }
            }
        }
        return null;
    }

    public GameObject Find(string id)
    {
        return Find((gobj) => gobj.Id == id);
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].HandleInput(inputHelper);
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach (GameObject obj in toRemove)
        {
            children.Remove(obj);
        }

        toRemove.Clear();

        foreach (GameObject obj  in children)
        {
            obj.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera camera)
    {
        if (!visible)
        {
            return;
        }
        List<GameObject>.Enumerator e = children.GetEnumerator();
        while (e.MoveNext())
        {
            e.Current.Draw(gameTime, spriteBatch, camera);
        }
    }

    public override void Reset()
    {
        base.Reset();
        foreach (GameObject obj in children)
        {
            obj.Reset();
        }
    }
}