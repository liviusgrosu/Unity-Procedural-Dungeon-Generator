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


    private void Awake()
    {
        grid = new RoomGrid(gridSize);
        rooms = new List<Room>();

        GenerateRooms();
        RenderMap();
    }

    private void GenerateRooms()
    {
        for(int i = 0; i < roomsAmount; i++)
        {
            int newRoomSizeW = Random.Range(roomMinSize, roomMaxSize + 1);
            int newRoomSizeH = Random.Range(roomMinSize, roomMaxSize + 1);

            int newRoomX = Random.Range(0, gridSize - newRoomSizeW);
            int newRoomY = Random.Range(0, gridSize - newRoomSizeH);

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
            Vector3 roomPosition = new Vector3(currRoom.x, 0f, currRoom.y);
            Vector3 roomScale = new Vector3(currRoom.width, debugRoomModel.transform.localScale.y, currRoom.height);

            GameObject roomInstance = Instantiate(debugRoomModel, roomPosition, Quaternion.identity);
            roomInstance.transform.localScale = roomScale;
        }
    }

    private bool AddRoom(Room newRoom)
    {
        foreach(Room currRoom in rooms)
        {
            bool overlapsX = newRoom.x >= currRoom.x && newRoom.x < currRoom.x + currRoom.width;
            bool overlapsY = newRoom.y >= currRoom.y && newRoom.y < currRoom.y + currRoom.height;

            if (overlapsX && overlapsY)
            {
                return false;
            }
        }

        rooms.Add(newRoom);
        return true;
    }
}