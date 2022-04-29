using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderMap : MonoBehaviour 
{
    [SerializeField] GameObject floor;
    [SerializeField] GameObject roof;
    [SerializeField] GameObject Wall;
    [SerializeField] GameObject DoorCutout;
    [SerializeField] Transform objectParent;
    [SerializeField] float objectScale;

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
            GameObject roomObj = Instantiate(floor, roomPosition, floor.transform.rotation);
            GameObject roofObj = Instantiate(roof, new Vector3(roomPosition.x, roof.transform.position.y, roomPosition.z), roof.transform.rotation);
            
            roomObj.transform.parent = objectParent;
            roofObj.transform.parent = objectParent;
            
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

        objectParent.localScale = Vector3.one * objectScale;
    }

    private void RenderWall(Vector3 pos, float rotationOffset)
    {
        GameObject wallInstance = Instantiate(Wall, pos, Wall.transform.rotation);
        wallInstance.transform.Rotate(0, rotationOffset - 180f, 0);
        wallInstance.transform.parent = objectParent;
    }
}