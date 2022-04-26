using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2 pos;

    public enum Type 
    { 
        Room, 
        Hallway 
    };

    public Type type;

    public Tile(Vector2 pos, Type type)
    {
        this.pos = pos;
        this.type = type;
    }

    public bool Equals(Tile other)
    {
        return pos.Equals(other.pos);
    }
}