using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDungeon : MonoBehaviour 
{
    public static List<Tile> Tiles;
    private List<Room> rooms;
    private List<GameObject> roomObjects;
    [SerializeField] private int gridSize;
    [SerializeField] private int roomsAmount;
    [SerializeField] private int roomMinSize, roomMaxSize;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float randomHallwayChance;
    [SerializeField] private GameObject debugRoomModel;
    public Triangulation triangulation;
    public MST mst;
    public AStar aStar;
    private RenderMap renderer;

    private void Awake()
    {
        Tiles = new List<Tile>();
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        renderer = GetComponent<RenderMap>();

        GenerateRooms();
        GenerateHallways();
        ConstructTiles();
        renderer.Render();
    }

    private void GenerateRooms()
    {
        // Randomly generate rooms with varying size and location
        for(int i = 0; i < roomsAmount; i++)
        {
            Room newRoom = null;
            int fallSafeMax = 100;
            int currentIncrement = 0;
            do
            {
                int newRoomSizeW = UnityEngine.Random.Range(roomMinSize, roomMaxSize + 1);
                int newRoomSizeH = UnityEngine.Random.Range(roomMinSize, roomMaxSize + 1);

                // Ensure that room is always an even number
                if (newRoomSizeW % 2 != 1)
                {
                    newRoomSizeW++;
                }

                if (newRoomSizeH % 2 != 1)
                {
                    newRoomSizeH++;
                }

                // Get new location
                int newRoomX = UnityEngine.Random.Range(0, gridSize - newRoomSizeW);
                int newRoomY = UnityEngine.Random.Range(0, gridSize - newRoomSizeH);

                // Create the room
                newRoom = new Room(newRoomX, newRoomY, newRoomSizeW, newRoomSizeH);

                currentIncrement++;
                if (currentIncrement > fallSafeMax)
                {
                    // Break out of loop if its stuck
                    break;
                }
            } while (!AddRoom(newRoom));
        }
    }

    private void PerformDelaunayTriangulation()
    {
        // Create a point list from the rooms
        List<Vector2> pointList = new List<Vector2>();
        foreach(Room room in rooms)
        {
            Vector2 middlePos = new Vector2(room.x, room.y) + (new Vector2(room.width, room.height) / 2f);
            pointList.Add(middlePos);
        }
        // Pass point list to the triangulation class
        triangulation = new Triangulation(pointList);
    }

    private void PerformMST()
    {
        // Create a MST
        mst = new MST(triangulation.vertices, triangulation.allEdges, randomHallwayChance);
    }

    private void PerformAStar()
    {
        // Calculate AStar paths
        aStar = new AStar(mst.resultingPath);
    }

    private void GenerateHallways()
    {
        //Create the hallways using various algorithms
        PerformDelaunayTriangulation();
        PerformMST();
        PerformAStar();
    }

    private void ConstructTiles()
    {
        // TODO: change this to a non-magic number
        float roomTileOffset = 0.5f;
        foreach(Room room in rooms)
        {
            // For each room create a tile
            for(int x = 0; x < room.width; x++)
            {
                for(int y = 0; y < room.height; y++)
                {
                    Tile tile = new Tile(new Vector2(room.x + x + roomTileOffset, room.y + y + roomTileOffset), Tile.Type.Room);
                    Tiles.Add(tile);
                }
            }
        }

        foreach(List<AStar.Node> currentPath in aStar.TotalPaths) 
        {
            foreach(AStar.Node node in currentPath)
            {
                // Ignore if the hallway overlaps another tile
                if (Tiles.Any(x => x.pos.Equals(node.pos)))
                {
                    continue;
                }

                Tile tile = new Tile(node.pos, Tile.Type.Hallway);
                Tiles.Add(tile);
            }
        }
    }

    private bool AddRoom(Room newRoom)
    {
        foreach(Room currRoom in rooms)
        {
            bool overlap =  newRoom.x <= (currRoom.x + currRoom.width) && (newRoom.x + newRoom.width) >= currRoom.x &&
                            newRoom.y <= (currRoom.y + currRoom.height) && (newRoom.y + newRoom.height) >= currRoom.y;
            
            if (overlap)
            {
                return false;
            }
        }

        rooms.Add(newRoom);
        return true;
    }
}