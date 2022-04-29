using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderMap : MonoBehaviour 
{
    [SerializeField] List<GameObject> floors;
    [SerializeField] List<GameObject> walls;
    [SerializeField] GameObject roof;
    [SerializeField] GameObject torch;
    [SerializeField] Transform objectParent;
    [SerializeField] float objectScale;
    [SerializeField] GameObject Player;
    
    [SerializeField] float torchMaxSpread;
    private float torchCurrentSpread;

    public void Render()
    {
        RenderTiles();
        
        Vector3 startingPos = new Vector3(GenerateDungeon.tiles[0].pos.x * objectScale, Player.GetComponent<CharacterController>().height / 2f, GenerateDungeon.tiles[0].pos.y * objectScale); 
        Instantiate(Player, startingPos, Player.transform.rotation);
    }

    private void RenderTiles()
    {
        foreach(Tile tile in GenerateDungeon.tiles)
        {
            int randomIdx = Random.Range(0, floors.Count);
            Vector3 roomPosition = new Vector3(tile.pos.x, 0f, tile.pos.y);
            GameObject roomObj = Instantiate(floors[randomIdx], roomPosition, floors[randomIdx].transform.rotation);
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
        int randomIdx = Random.Range(0, walls.Count);
        GameObject wallInstance = Instantiate(walls[randomIdx], pos, walls[randomIdx].transform.rotation);
        wallInstance.transform.Rotate(0, rotationOffset - 180f, 0);
        wallInstance.transform.parent = objectParent;

        torchCurrentSpread++;
        if (torchCurrentSpread % torchMaxSpread == 0)
        {
            RenderTorch(pos, rotationOffset);
        }
    }

    private void RenderTorch(Vector3 pos, float rotationOffset)
    {
        GameObject torchInstance = Instantiate(torch, pos, torch.transform.rotation);
        torchInstance.transform.Rotate(0, rotationOffset - 180f, 0);
        torchInstance.transform.parent = objectParent;

    }
}