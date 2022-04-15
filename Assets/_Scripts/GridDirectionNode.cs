using System.Collections.Generic;

class GridRoom
{
    public int[] coordinate;
    public GridRoom parent;
    public List<GridRoom> children;
    public List<int> availableDirections;
    public List<int> adjacentRoomDirections;

    public GridRoom(int[] coordinate, GridRoom parent)
    {
        this.parent = parent;
        Init(coordinate);
    }

    public GridRoom(int[] coordinate)
    {
        Init(coordinate);
    }

    void Init(int[] coordinate)
    {
        this.coordinate = new int[coordinate.Length];
        coordinate.CopyTo(this.coordinate, 0);

        children = new List<GridRoom>();
        adjacentRoomDirections = new List<int>();
        availableDirections = new List<int>();
    }

    public void AddChildRoom(int[] coordinate)
    {
        children.Add(new GridRoom(coordinate, this));
    }
}