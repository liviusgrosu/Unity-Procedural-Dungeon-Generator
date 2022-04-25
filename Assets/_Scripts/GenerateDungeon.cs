using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDungeon : MonoBehaviour 
{
    RoomGrid grid;
    List<Room> rooms;
    List<GameObject> roomObjects;

    [SerializeField] int gridSize;
    [SerializeField] int roomsAmount;
    [SerializeField] int roomMinSize, roomMaxSize;
    [Range(0.0f, 1.0f)]
    [SerializeField] float randomHallwayChance;
    [SerializeField] GameObject debugRoomModel;
    public Triangulation triangulation;
    public MST mst;
    public AStar aStar;
    private RenderMap renderer;

    private void Awake()
    {
        grid = new RoomGrid(gridSize);
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        renderer = GetComponent<RenderMap>();

        GenerateRooms();
        GenerateHallways();
        renderer.Render(rooms, aStar);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rooms.Clear();
            
            ClearMap();
            GenerateRooms();
            GenerateHallways();
            renderer.Render(rooms, aStar);
        }
    }

    private void GenerateRooms()
    {
        for(int i = 0; i < roomsAmount; i++)
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

            int newRoomX = UnityEngine.Random.Range(0, gridSize - newRoomSizeW);
            int newRoomY = UnityEngine.Random.Range(0, gridSize - newRoomSizeH);

            Room newRoom = new Room(newRoomX, newRoomY, newRoomSizeW, newRoomSizeH);

            if (!AddRoom(newRoom))
            {
                continue;
            }
        }
    }

    private void ClearMap()
    {
        foreach(GameObject room in roomObjects)
        {
            Destroy(room);
        }
        roomObjects.Clear();
    }

    private void PerformDelaunayTriangulation()
    {
        List<Vector2> pointList = new List<Vector2>();

        foreach(Room room in rooms)
        {
            // TODO: might need to convert these to int
            // Decimal might break the A* code
            Vector2 middlePos = new Vector2(room.x, room.y) + (new Vector2(room.width, room.height) / 2f);
            pointList.Add(middlePos);
        }
        
        triangulation = new Triangulation(pointList);
    }

    private void PerformMST()
    {
        mst = new MST(triangulation.vertices, triangulation.allEdges, randomHallwayChance);
    }

    private void PerformAStar()
    {
        aStar = new AStar(mst.resultingPath);
    }

    private void GenerateHallways()
    {
        PerformDelaunayTriangulation();
        PerformMST();
        PerformAStar();
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