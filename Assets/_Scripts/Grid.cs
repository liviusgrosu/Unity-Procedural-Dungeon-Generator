using System;

public class Grid
{
    public Grid(int minRange, int maxRange)
    {
        Random rand = new Random();

        EdgesCount = 4;
        EdgeSize = rand.Next(minRange, maxRange);
        GridData = new int[EdgeSize, EdgeSize];  
    }

    // 2d array of the grid
    // 1: room
    // 2: starting room
    // 3: ending room
    public int[,] GridData;

    // Temp: will get this from template data
    // We could potentially have different shaped levels like triangles, hexagons, etc...
    // For now focus on a simple boxed level
    public int EdgesCount;
    public int EdgeSize;
}
