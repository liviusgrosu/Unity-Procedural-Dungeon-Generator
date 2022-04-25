using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderMap : MonoBehaviour 
{
    [SerializeField] GameObject baseRoomModel;
    [SerializeField] GameObject Wall;
    [SerializeField] GameObject DoorCutout;
    public void Render(List<Room> rooms, AStar aStar)
    {
        RenderRooms(rooms);
    }

    private void RenderRooms(List<Room> rooms)
    {
        foreach(Room currRoom in rooms)
        {
            for (int x = 0; x < currRoom.width; x++)
            {
                for (int y = 0; y < currRoom.height; y++)
                { 
                    float offset = baseRoomModel.transform.localScale.x / 2f;
                    Vector3 roomPosition = new Vector3(currRoom.x + x + offset, 0f, currRoom.y + y + offset);
                    GameObject roomInstance = Instantiate(baseRoomModel, roomPosition, baseRoomModel.transform.rotation);

                    if (x == 0)
                    {
                        RenderWall(roomPosition, 0f);
                    }
                    if (x == currRoom.width - 1)
                    {
                        RenderWall(roomPosition, 180f);
                    }
                    if (y == 0)
                    {
                        RenderWall(roomPosition, 270f);
                    }
                    if (y == currRoom.height - 1)
                    {
                        RenderWall(roomPosition, 90f);
                    }
                }
            }
        }
    }

    private void RenderWall(Vector3 pos, float rotationOffset)
    {
        GameObject wallInstance = Instantiate(Wall, pos, Wall.transform.rotation);
        wallInstance.transform.Rotate(0, 0, rotationOffset);
    }

    private void RenderHallways()
    {
        // foreach(List<AStar.Node> currentPath in aStar.totalPaths)
        // {
        //     foreach(AStar.Node node in currentPath)
        //     {
        //         if (CheckHallwayOverlapsRoom(node.pos))
        //         {
        //             continue;
        //         }
        //         float offset = baseRoomModel.transform.localScale.x / 2f;
        //         RenderRoom(node.pos.x - offset, node.pos.y - offset);
        //     }
        // }
    }

    private void RenderRoom(float x, float y)
    {
        // float offset = baseRoomModel.transform.localScale.x / 2f;
        // Vector3 roomPosition = new Vector3(x + offset, 0f, y + offset);

        // GameObject roomInstance = Instantiate(baseRoomModel, roomPosition, baseRoomModel.transform.rotation);
        // roomObjects.Add(roomInstance);
    }
}