using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int x, y;
    public int width, height;

    public Room(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;

        this.width = width;
        this.height = height;
    }
}