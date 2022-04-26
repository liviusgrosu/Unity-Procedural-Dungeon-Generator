using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderMap : MonoBehaviour 
{
    [SerializeField] GameObject baseRoomModel;
    [SerializeField] GameObject Wall;
    [SerializeField] GameObject DoorCutout;

    public void Render()
    {
        RenderTiles();
        // RenderRooms();
        // RenderHallways();
    }

    private void RenderTiles()
    {
        foreach(Tile tile in GenerateDungeon.tiles)
        {
            Vector3 roomPosition = new Vector3(tile.pos.x, 0f, tile.pos.y);
            GameObject roomInstance = Instantiate(baseRoomModel, roomPosition, baseRoomModel.transform.rotation);
            
            // Check for NORTH adjacent room 
            if (!GenerateDungeon.tiles.Any(x => x.pos.Equals(tile.pos - new Vector2(0, 1))))
            {
                RenderWall(roomPosition, 270);
            }

            // Check for SOUTH adjacent room 
            if (!GenerateDungeon.tiles.Any(x => x.pos.Equals(tile.pos - new Vector2(0, -1))))
            {
                RenderWall(roomPosition, 90);
            }

            // Check for WEST adjacent room 
            if (!GenerateDungeon.tiles.Any(x => x.pos.Equals(tile.pos - new Vector2(1, 0))))
            {
                RenderWall(roomPosition, 0);
            }

            // Check for EAST adjacent room 
            if (!GenerateDungeon.tiles.Any(x => x.pos.Equals(tile.pos - new Vector2(-1, 0))))
            {
                RenderWall(roomPosition, 180);
            }

            // Check for NORTH adjacent room 
            // if (!GenerateDungeon.tiles.Any(x => x.pos.Equals(tile.pos - new Vector2(0, 1))))
            // {
            //     RenderWall(roomPosition, 180f);
            // }
        }
    }

    // private void RenderRooms()
    // {
    //     foreach(Room currRoom in rooms)
    //     {
    //         for (int x = 0; x < currRoom.width; x++)
    //         {
    //             for (int y = 0; y < currRoom.height; y++)
    //             { 
    //                 float offset = baseRoomModel.transform.localScale.x / 2f;
    //                 Vector3 roomPosition = new Vector3(currRoom.x + x + offset, 0f, currRoom.y + y + offset);
    //                 GameObject roomInstance = Instantiate(baseRoomModel, roomPosition, baseRoomModel.transform.rotation);

    //                 if (x == 0)
    //                 {
    //                     // Render the west wall
    //                     RenderWall(roomPosition, 0f);
    //                 }
    //                 if (x == currRoom.width - 1)
    //                 {
    //                     // Render the east wall
    //                     RenderWall(roomPosition, 180f);
    //                 }
    //                 if (y == 0)
    //                 {
    //                     // Render the south wall
    //                     RenderWall(roomPosition, 270f);
    //                 }
    //                 if (y == currRoom.height - 1)
    //                 {
    //                     // Render the north wall
    //                     RenderWall(roomPosition, 90f);
    //                 }
    //             }
    //         }
    //     }
    // }

    private void RenderWall(Vector3 pos, float rotationOffset)
    {
        GameObject wallInstance = Instantiate(Wall, pos, Wall.transform.rotation);
        wallInstance.transform.Rotate(0, 0, rotationOffset);
    }

    // private void RenderHallways()
    // {
    //     foreach(List<AStar.Node> currentPath in aStar.totalPaths)
    //     {
    //         foreach(AStar.Node node in currentPath)
    //         {
    //             if (CheckHallwayOverlapsRoom(node.pos))
    //             {
    //                 continue;
    //             }
                
    //             Vector3 roomPosition = new Vector3(node.pos.x, 0f, node.pos.y);
    //             GameObject roomInstance = Instantiate(baseRoomModel, roomPosition, baseRoomModel.transform.rotation);

    //             List<int> wallDirections = new List<int> {0, 1, 2, 3}.Except(node.openDirections).ToList();
    //             foreach(int wallDirection in wallDirections)
    //             {
    //                 switch(wallDirection)
    //                 {
    //                     case 0:
    //                         RenderWall(roomPosition, 90f);
    //                         break;
    //                     case 1:
    //                         RenderWall(roomPosition, 180f);
    //                         break;
    //                     case 2:
    //                         RenderWall(roomPosition, 270f);
    //                         break;
    //                     case 3:
    //                         RenderWall(roomPosition, 0f);
    //                         break;
    //                 }
    //             }
    //         }
    //     }
    // }

    // private bool CheckHallwayOverlapsRoom(Vector2 pos)
    // {
    //     // TODO: remove this function and instead prune the astar hallway list
    //     foreach(Room room in rooms)
    //     {
    //         if (pos.x >= room.x && pos.x <= room.x + room.width &&
    //             pos.y >= room.y && pos.y <= room.y + room.height)
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
}