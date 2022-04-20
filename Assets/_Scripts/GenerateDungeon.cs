using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDungeon : MonoBehaviour 
{
    RoomGrid grid;
    List<Room> rooms;

    [SerializeField] int gridSize;
    [SerializeField] int roomsAmount;
    [SerializeField] int roomMinSize, roomMaxSize;
    [SerializeField] GameObject debugRoomModel;
    [SerializeField] GameObject debugRoomPoint;
    List<GameObject> debugPoints;

    private void Awake()
    {
        grid = new RoomGrid(gridSize);
        rooms = new List<Room>();

        Triangulation.DisplayPoint += DrawDebugPoint;
        Triangulation.ClearDisplayedPoints += ClearDebugPoints;
        debugPoints = new List<GameObject>();

        GenerateRooms();
        RenderMap();
        PerformDelaunayTriangulation();
    }

    private void GenerateRooms()
    {
        for(int i = 0; i < roomsAmount; i++)
        {
            int newRoomSizeW = UnityEngine.Random.Range(roomMinSize, roomMaxSize + 1);
            int newRoomSizeH = UnityEngine.Random.Range(roomMinSize, roomMaxSize + 1);

            int newRoomX = UnityEngine.Random.Range(0, gridSize - newRoomSizeW);
            int newRoomY = UnityEngine.Random.Range(0, gridSize - newRoomSizeH);

            Room newRoom = new Room(newRoomX, newRoomY, newRoomSizeW, newRoomSizeH);

            if (!AddRoom(newRoom))
            {
                continue;
            }
        }
    }

    private void RenderMap()
    {
        foreach(Room currRoom in rooms)
        {
            Vector3 roomPosition = new Vector3(currRoom.x + (currRoom.width / 2f), 0f, currRoom.y + (currRoom.height / 2f));
            Vector3 roomScale = new Vector3(currRoom.width, debugRoomModel.transform.localScale.y, currRoom.height);

            GameObject roomInstance = Instantiate(debugRoomModel, roomPosition, Quaternion.identity);
            roomInstance.transform.localScale = roomScale;
        }
    }

    private void PerformDelaunayTriangulation()
    {
        List<Vector2> pointList = new List<Vector2>();

        foreach(Room room in rooms)
        {
            Vector2 middlePos = new Vector2(room.x, room.y) + (new Vector2(room.width, room.height) / 2f);
            pointList.Add(middlePos);
        }
        
        Triangulation triangulation = new Triangulation(pointList);
        int i = 5;
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

    public void DrawDebugPoint(Triangulation.Vertex input)
    {
        debugPoints.Add(Instantiate(debugRoomPoint, new Vector3(input.x, 0f, input.y), Quaternion.identity));
    }

    public void ClearDebugPoints()
    {
        if (debugPoints.Count != 0)
        {
            foreach(GameObject point in debugPoints)
            {
                Destroy(point);
            }
            debugPoints.Clear();
        }
    }
}