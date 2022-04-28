using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderMap : MonoBehaviour 
{
    [SerializeField] GameObject baseRoomModel;
    [SerializeField] GameObject Wall;
    [SerializeField] GameObject DoorCutout;

    [SerializeField] GameObject Player;

    public void Render()
    {
        RenderTiles();
        
        Vector3 startingPos = new Vector3(GenerateDungeon.tiles[0].pos.x, 0, GenerateDungeon.tiles[0].pos.y); 
        Instantiate(Player, startingPos, Player.transform.rotation);
        Debug.Break();
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
        }
    }

    private void RenderWall(Vector3 pos, float rotationOffset)
    {
        GameObject wallInstance = Instantiate(Wall, pos, Wall.transform.rotation);
        wallInstance.transform.Rotate(0, 0, rotationOffset);
    }
}