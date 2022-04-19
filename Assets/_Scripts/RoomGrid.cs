using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGrid
{
    int[] rooms;

    public RoomGrid(int size)
    {
        rooms = new int[size * size];
    }
}