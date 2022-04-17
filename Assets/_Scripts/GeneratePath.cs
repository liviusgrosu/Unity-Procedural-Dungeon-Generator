using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePath : MonoBehaviour
{   
    [Header("Room Model Types")]
    [Space(5)]
    public GameObject PlatformModel;
    public GameObject WallModel;
    public GameObject CornerModel;
    public GameObject DebugModel1, DebugModel2, DebugModel3, DebugModel4;
    public float DebugScaleFactor = 1f;

    // The chances of the path going towards the end side
    [Tooltip("The chance of traversing towards the end")]
    [Range(1, 100)]
    public int EndDirectionFactor = 50;
    [Header("Optional Paths")]
    [Space(5)]
    [Tooltip("Occurance of optional rooms")]
    [Range(0, 1)]
    public float OptionalRoomCoverage = 0.5f;
    public int MinDepthRange;
    public int MaxDepthRange;

    [Header("Direction Models")]
    [Space(5)]
    public GameObject NorthModel;
    public GameObject EastModel;
    public GameObject SouthModel;
    public GameObject WestModel;

    [Header("Misc.")]
    // TODO: add custom editor range
    // https://docs.unity3d.com/ScriptReference/EditorGUILayout.MinMaxSlider.html
    public int MinRange;
    public int MaxRange;

    private int startDirection, endDirection, leftDirection, rightDirection;

    private Grid grid;
    private GridRoom gridDirectionParentNode, currentDirectionNode;

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    private int[] startRoom, endRoom;
    private int[] currentRoom;

    void Awake()
    {   
        // --- Init Grid ---
        grid = new Grid(MinRange, MaxRange);

        startSideRooms = new int[grid.EdgeSize, 2];
        endSideRooms = new int[grid.EdgeSize, 2];

        // Select a random direction as the start
        startDirection = UnityEngine.Random.Range(0, grid.EdgesCount);

        // Get the surronding available directions
        leftDirection = (startDirection + 1) % grid.EdgesCount;
        endDirection = (leftDirection + 1) % grid.EdgesCount;
        rightDirection = (endDirection + 1) % grid.EdgesCount; 

        // Store a list of rooms that exist in the start and end rooms
        switch(startDirection)
        {
            case 0:
                // Starting Edge:   North
                // Ending Edge:     South
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, 0, i, grid.EdgeSize - 1, i);
                }
                break;
            case 1:
                // Starting Edge:   East
                // Ending Edge:     West
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, i, grid.EdgeSize - 1, i, 0);
                }
                break;
            case 2:
                // Starting Edge:   South
                // Ending Edge:     North
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, grid.EdgeSize - 1, i, 0, i);
                }
                break;
            case 3:
                // Starting Edge:   West
                // Ending Edge:     East
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, i, 0, i, grid.EdgeSize - 1);
                }
                break;
            default:
                break;
        }

        // Pick a random room from the starting edge. That will be the starting room
        int randNum = UnityEngine.Random.Range(0, grid.EdgeSize);
        startRoom = new int[] {startSideRooms[randNum, 0], startSideRooms[randNum, 1]};
        currentRoom = new int[2];
        startRoom.CopyTo(currentRoom, 0);

        // Create a new GridRoom object. This will be the parent node
        gridDirectionParentNode = new GridRoom(currentRoom, null);
        currentDirectionNode = gridDirectionParentNode;

        // Create the main path
        TraversePath(startDirection);
        // Store the available paths around each room in the main path
        CalculateAvailableDirectionsForPath();
        // Calculate optional paths
        TraverseOptionalPath();
        // Recalculate available directions and connecting rooms
        RecalculatePathInformation(gridDirectionParentNode);
        // Render the path with models
        RenderPath();
        AddBiggerRooms();
    }

    void TraversePath(int previousDirection)
    {
        // Save current room into the grid data
        grid.GridData[currentRoom[0], currentRoom[1]] = 1;

        if (CheckIfRoomOnEdge(endSideRooms, currentRoom[0], currentRoom[1]))
        {
            // Stop when a tile on the end side is traversed
            endRoom = new int[] { currentRoom[0], currentRoom[1] };
            grid.GridData[currentRoom[0], currentRoom[1]] = 3;
            return;
        }

        if (startRoom.SequenceEqual(currentRoom))
        {
            // Store the starting room coordinate
            grid.GridData[currentRoom[0], currentRoom[1]] = 2;
        }

        // Check available paths
        int[] availablePaths = { leftDirection, endDirection, rightDirection };

        // Remove possibility of going back
        if (previousDirection == leftDirection)
        {
            availablePaths[2] = -1;
        }
        else if (previousDirection == rightDirection)
        {
            availablePaths[0] = -1;
        }

        // Left direction
        PruneAvailableDirections(0, leftDirection, availablePaths);

        // End direction
        PruneAvailableDirections(1, endDirection, availablePaths);

        // Right direction
        PruneAvailableDirections(2, rightDirection, availablePaths);

        // Remove unavailable paths
        int newDirection = -1;
        List<int> pathCandidates = availablePaths.ToList();
        pathCandidates.RemoveAll(item => item == -1);

        if (pathCandidates.Count == 0)
        {
            // If theres no more availble paths then the path is finished
            return;
        }

        if (pathCandidates.Count >= 2 && pathCandidates.Contains(endDirection))
        {
            if(UnityEngine.Random.Range(0, 100) >= EndDirectionFactor)
            {
                // Make direction towards the ending edge harder to traverse
                pathCandidates.Remove(endDirection);
            }
        }

        // Get random index of possible path candidates
        int rand = UnityEngine.Random.Range(0, pathCandidates.Count);
        int directionIdx = Array.IndexOf(availablePaths, pathCandidates[rand]);
        newDirection = availablePaths[directionIdx];

        // Traverse the next room
        switch (newDirection)
        {
            case 0:
                currentRoom[0] -= 1;
                break;
            case 1:
                currentRoom[1] += 1;
                break;
            case 2:
                currentRoom[0] += 1;
                break;
            case 3:
                currentRoom[1] -= 1;
                break;
            default:
                break;
        }

        // Traverse to the next room
        currentDirectionNode.AddChildRoom(currentRoom);
        currentDirectionNode = currentDirectionNode.children[0];
        TraversePath(newDirection);
    }

    void CalculateAvailableDirectionsForPath()
    {
        currentDirectionNode = gridDirectionParentNode.children[0];
        while(currentDirectionNode.children.Count != 0)
        {
            // Calculate the available directions for this room
            CalculateRoomsAvaibleDirections(currentDirectionNode);
            // Get the next child in the main path
            currentDirectionNode = currentDirectionNode.children[0];
        }
    }

    void TraverseOptionalPath()
    {
        List<GridRoom> rooms = new List<GridRoom>();

        // Add main path the list
        currentDirectionNode = gridDirectionParentNode.children[0];
        while (currentDirectionNode.children.Count != 0)
        {
            rooms.Add(currentDirectionNode);
            currentDirectionNode = currentDirectionNode.children[0];
        }

        // Reset pointer to parent node
        currentDirectionNode = gridDirectionParentNode.children[0];
        
        // Determine how many rooms to traverse for optional paths
        int amountOfRoomsToTraverse = (int)(rooms.Count * (OptionalRoomCoverage) - 1);
        for (int roomIdx = 0; roomIdx < amountOfRoomsToTraverse; roomIdx++)
        {
            // Get a random room
            int randomRoomIdx = UnityEngine.Random.Range(1, rooms.Count - 2);
            GridRoom currentRoom = rooms[randomRoomIdx];
            // Remove the room from the list
            rooms.RemoveAt(randomRoomIdx);
            List<int> availableDirections = currentRoom.availableDirections;
            if (availableDirections.Count != 0)
            {
                // Choose a random available direction
                int randomDirection = availableDirections[UnityEngine.Random.Range(0, availableDirections.Count)];
                // Add that direction and remove it from available directions of the room
                availableDirections.RemoveAll(item => item == randomDirection);

                int randomDepth = UnityEngine.Random.Range(MinDepthRange, MaxDepthRange);

                // Traverse depth
                for (int depthIdx = 0; depthIdx < randomDepth; depthIdx++)
                {
                    // Get coordinate of new room
                    int[] coordinate = new int[2];
                    currentRoom.coordinate.CopyTo(coordinate, 0);

                    switch (randomDirection)
                    {
                        case 0:
                            coordinate[0] -= 1;
                            break;
                        case 1:
                            coordinate[1] += 1;
                            break;
                        case 2:
                            coordinate[0] += 1;
                            break;
                        case 3:
                            coordinate[1] -= 1;
                            break;
                        default:
                            break;
                    }

                    // Save current room into the grid data
                    grid.GridData[coordinate[0], coordinate[1]] = 1;

                    // Update surrounding rooms to remove its available directions
                    UpdateSurroundingAvailableDirections(coordinate);

                    currentRoom.AddChildRoom(coordinate);
                    // Traverse that newly created child
                    currentRoom = currentRoom.children[currentRoom.children.Count - 1];
                    rooms.Add(currentRoom);

                    // Calculate the rooms available directions
                    CalculateRoomsAvaibleDirections(currentRoom);

                    if (currentRoom.availableDirections.Count == 0)
                    {
                        // No available rooms left then stop traversing path
                        break;
                    }

                    // Find a new random direction
                    randomDirection = currentRoom.availableDirections[UnityEngine.Random.Range(0, currentRoom.availableDirections.Count)];
                    // Add that direction and remove it from available directions of the room
                    currentRoom.availableDirections.RemoveAll(item => item == randomDirection);
                }
            }
        }
    }
    void RecalculatePathInformation(GridRoom room)
    {
        // Get coordinates of adjacent rooms
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        List<int> availableDirections = new List<int> {0, 1, 2, 3};

        // Check any adjacent rooms that are free
        if (CheckIfCoordinateExist(gridDirectionParentNode, northCoordinate) || CheckRoomOutOfBound(northCoordinate))
        {
            availableDirections[0] = -1;
        }

        if (CheckIfCoordinateExist(gridDirectionParentNode, eastCoordinate) || CheckRoomOutOfBound(eastCoordinate))
        {
            availableDirections[1] = -1;
        }

        if (CheckIfCoordinateExist(gridDirectionParentNode, southCoordinate) || CheckRoomOutOfBound(southCoordinate))
        {
            availableDirections[2] = -1;
        }

        if (CheckIfCoordinateExist(gridDirectionParentNode, westCoordinate) || CheckRoomOutOfBound(westCoordinate))
        {
            availableDirections[3] = -1;
        }

        availableDirections.RemoveAll(item => item == -1);

        // Update the available directions in the current room
        room.availableDirections = availableDirections;

        // Check connecting rooms
        List<int> adjacentRoomDirections = new List<int>();

        foreach(GridRoom child in room.children)
        {
            if (child.coordinate.SequenceEqual(northCoordinate))
            {
                adjacentRoomDirections.Add(0);
            }
            else if (child.coordinate.SequenceEqual(eastCoordinate))
            {
                adjacentRoomDirections.Add(1);
            }
            else if (child.coordinate.SequenceEqual(southCoordinate))
            {
                adjacentRoomDirections.Add(2);
            }
            else if (child.coordinate.SequenceEqual(westCoordinate))
            {
                adjacentRoomDirections.Add(3);
            }
            // Update the connecting room list
            room.adjacentRoomDirections = adjacentRoomDirections;
            
            // Check the children too
            RecalculatePathInformation(child);
        }
    }

    void AddBiggerRooms()
    {
        int windowH = 3;
        int windowW = 3;

        for(int y = 0; y < grid.EdgeSize - windowH; y += windowH)
        {
            for(int x = 0; x < grid.EdgeSize - windowW; x++)
            {
                if (CheckIfWindowExists(x, y, windowW, windowH))
                {
                    x += windowW + 1;
                }
            }
        }
    }

    bool CheckIfWindowExists(int startX, int startY, int windowW, int windowH)
    {
        /* 
            Create a north, east, south, west list
            Go through each list and add neighbouring rooms as adjacent rooms
        */

        List<(int, int)> northRooms = new List<(int, int)>();
        List<(int, int)> eastRooms  = new List<(int, int)>();
        List<(int, int)> westRooms  = new List<(int, int)>();
        List<(int, int)> southRooms = new List<(int, int)>();
        List<(int, int)> innerRooms = new List<(int, int)>();

        for (int localY = startY; localY < windowH + startY + 1; localY++)
        {
            for (int localX = startX; localX < windowW + startX + 1; localX++)
            {
                if (grid.GridData[localY, localX] != 1)
                {
                    return false;
                }

                
                if (localY == startY)
                {
                    northRooms.Add((localY, localX));
                }
                else if (localY == windowH + startY)
                {
                    southRooms.Add((localY, localX));
                }
                else if (localX == startX)
                {
                    westRooms.Add((localY, localX));
                }
                else if (localX == windowW + startX)
                {
                    eastRooms.Add((localY, localX));
                }
                else 
                {
                    innerRooms.Add((localY, localX));
                }
            }
        }
        // Only show the starting corner
        // GameObject marker = Instantiate(DebugModel, new Vector3(startY, 0f, startX), DebugModel.transform.rotation);
        // marker.name = $"[{startY}, {startX}] Starting Corner";

        // TEMP
        foreach((int, int) room in northRooms)
        {
            Instantiate(DebugModel1, new Vector3(room.Item1, 0f, room.Item2), DebugModel1.transform.rotation);
        }

        foreach((int, int) room in eastRooms)
        {
            Instantiate(DebugModel2, new Vector3(room.Item1, 0f, room.Item2), DebugModel2.transform.rotation);
        }

        foreach((int, int) room in southRooms)
        {
            Instantiate(DebugModel3, new Vector3(room.Item1, 0f, room.Item2), DebugModel3.transform.rotation);
        }

        foreach((int, int) room in westRooms)
        {
            Instantiate(DebugModel4, new Vector3(room.Item1, 0f, room.Item2), DebugModel4.transform.rotation);
        }


        return true;
    }

    void RenderPath()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            // Don't render if no theres no path
            return;
        }

        if (NorthModel != null && EastModel != null && SouthModel != null && WestModel != null)
        {
            // DEBUG: Render the the direction indicators
            NorthModel.transform.position = new Vector3(-1f, -0.5f, DebugScaleFactor * 1.5f * grid.EdgeSize / 2);
            EastModel.transform.position = new Vector3(DebugScaleFactor * 1.5f * grid.EdgeSize / 2, -0.5f, DebugScaleFactor * 1.5f * grid.EdgeSize + 1f);
            SouthModel.transform.position = new Vector3(DebugScaleFactor * 1.5f * grid.EdgeSize + 2f, -0.5f, DebugScaleFactor * 1.5f * grid.EdgeSize / 2);
            WestModel.transform.position = new Vector3(DebugScaleFactor * 1.5f * grid.EdgeSize / 2, -0.5f, -1f);
        }

        // Draw the rooms
        currentDirectionNode = gridDirectionParentNode;
        TraverseAndDrawRoom(currentDirectionNode);
    }

    void CalculateRoomsAvaibleDirections(GridRoom room)
    {
        room.availableDirections = new List<int> {0, 1, 2, 3};
        // Get coordinates around the path
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        // Check north
        if (CheckIfCoordinateExist(gridDirectionParentNode, northCoordinate) || CheckRoomOutOfBound(northCoordinate))
        {
            room.availableDirections[0] = -1;
        }

        // Check east
        if (CheckIfCoordinateExist(gridDirectionParentNode, eastCoordinate) || CheckRoomOutOfBound(eastCoordinate))
        {
            room.availableDirections[1] = -1;
        }

        // Check south
        if (CheckIfCoordinateExist(gridDirectionParentNode, southCoordinate) || CheckRoomOutOfBound(southCoordinate))
        {
            room.availableDirections[2] = -1;
        }

        // Check west
        if (CheckIfCoordinateExist(gridDirectionParentNode, westCoordinate) || CheckRoomOutOfBound(westCoordinate))
        {
            room.availableDirections[3] = -1;
        }
        // Update the available directions list
        room.availableDirections.RemoveAll(item => item == -1);
    }

    bool CheckIfCoordinateExist(GridRoom room, int[] coordinate)
    {
        GridRoom targetRoom = new GridRoom(new int[] {-1, -1});

        CheckIfCoordinateExistLoop(room, targetRoom, coordinate);

        return targetRoom.coordinate.SequenceEqual(coordinate);
    }

    void CheckIfCoordinateExistLoop(GridRoom room, GridRoom targetRoom, int[] coordinate)
    {
        if (coordinate[0] == room.coordinate[0] && coordinate[1] == room.coordinate[1])
        {
            coordinate.CopyTo(targetRoom.coordinate, 0);
            return;
        }

        foreach(GridRoom child in room.children)
        {
            CheckIfCoordinateExistLoop(child, targetRoom, coordinate);
        }
    }

    GridRoom GetRoom(GridRoom currentRoom, int[] targetCoordinate)
    {
        if(currentRoom.coordinate.SequenceEqual(targetCoordinate))
        {
            return currentRoom;
        }

        foreach(GridRoom childRoom in currentRoom.children)
        {
            GridRoom result = GetRoom(childRoom, targetCoordinate);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    bool CheckRoomOutOfBound(int[] coordinate)
    {
        // Check if the room is out of bound
        return coordinate[0] < 0 || coordinate[1] >= grid.EdgeSize || coordinate[0] >= grid.EdgeSize || coordinate[1] < 0;
    }

    void FillEdgeTiles(int idx, int startRoomY, int startRoomX, int endRoomY, int endRoomX)
    {
        // Store the room coordinates on the start to a list
        startSideRooms[idx, 0] = startRoomY;
        startSideRooms[idx, 1] = startRoomX;

        // Store the room coordinates on the end to a list
        endSideRooms[idx, 0] = endRoomY;
        endSideRooms[idx, 1] = endRoomX;
    }

    void OnDrawGizmos()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            return;
        }

        for (int i = 0; i < grid.EdgeSize; i++)
        {
            for (int j = 0; j < grid.EdgeSize; j++)
            {
                if (startRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(DebugScaleFactor * i, 0, DebugScaleFactor * j), new Vector3(1, 1, 1));
                }

                if (endRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(DebugScaleFactor * i, 0, DebugScaleFactor * j), new Vector3(1, 1, 1));
                }
            }
        }
    }

    void TraverseAndDrawRoom(GridRoom room)
    {
        // Render the platform
        GameObject platformInstaObj = Instantiate(PlatformModel, new Vector3(DebugScaleFactor * room.coordinate[0], -0.5f, DebugScaleFactor * room.coordinate[1]), PlatformModel.transform.rotation);
        platformInstaObj.transform.localScale = Vector3.Scale(platformInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
        platformInstaObj.name = $"Platform - [{room.coordinate[0]},{room.coordinate[1]}]";

        // Get the non adjacent rooms. This is where the walls will exist
        List<int> wallDirections = new List<int> {0, 1, 2, 3};
        wallDirections = wallDirections.Except(room.adjacentRoomDirections).ToList();

        // Remove the parent room from the list
        if (room.parent != null)
        {
            wallDirections.Remove(GetParentRoomDirection(room));
        }

        foreach(int direction in wallDirections)
        {
            // Instantiate the wall, scale it, and position it
            GameObject wallInstaObj = Instantiate(WallModel, Vector3.zero, WallModel.transform.rotation);
            wallInstaObj.transform.localScale = Vector3.Scale(wallInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
            Vector3 wallPosition = platformInstaObj.transform.position;
            Vector3 newPos = Vector3.zero;
            string baseName = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}]";
            // Position a wall for each direction where a wall is supposed to be
            switch (direction)
            {
                case 0:
                    // North wall
                    newPos = new Vector3(wallPosition.x - (DebugScaleFactor * 0.45f), -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"{baseName} N";
                    break;
                case 1:
                    // East wall
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z + (DebugScaleFactor * 0.45f));
                    wallInstaObj.name = $"{baseName} E";
                    break;
                case 2:
                    // South wall
                    newPos = new Vector3(wallPosition.x + (DebugScaleFactor * 0.45f), -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"{baseName} S";
                    break;
                case 3:
                    // West wall
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z - (DebugScaleFactor * 0.45f));
                    wallInstaObj.name = $"{baseName} W";
                    break;
            }
            wallInstaObj.transform.position = newPos;
        }

        // Render the corners
        RenderCorner(room, -1, 1, new List<int> {0, 1}, platformInstaObj.transform.position);  // North east
        RenderCorner(room, 1, 1, new List<int> {1, 2}, platformInstaObj.transform.position);   // South east
        RenderCorner(room, 1, -1, new List<int> {3, 2}, platformInstaObj.transform.position);  // South west
        RenderCorner(room, -1, -1, new List<int> {3, 0}, platformInstaObj.transform.position); // North west

        foreach(GridRoom child in room.children)
        {
            // Traverse the children rooms
            TraverseAndDrawRoom(child);
        }
    }

    void RenderCorner(GridRoom room, int xDirection, int zDirection, List<int> adjacentRoomCheck, Vector3 origin)
    {
        // Get the corner room
        GridRoom cornerRoom = GetRoom(gridDirectionParentNode, new int[] {room.coordinate[0] + xDirection, room.coordinate[1] + zDirection });

        List<int> adjacentCornerRoomCheck = new List<int> {0, 1, 2, 3};
        adjacentCornerRoomCheck = adjacentCornerRoomCheck.Except(adjacentRoomCheck).ToList();

        // Check: 
        // - the a corner room doesnt exists OR
        // - A wall exists near the corner room OR
        // - That a wall exists near the current room 
        if (
            cornerRoom == null ||
            (!cornerRoom.adjacentRoomDirections.Contains(adjacentCornerRoomCheck[0]) || !cornerRoom.adjacentRoomDirections.Contains(adjacentCornerRoomCheck[1])) ||
            (!room.adjacentRoomDirections.Contains(adjacentRoomCheck[0]) && !room.adjacentRoomDirections.Contains(adjacentRoomCheck[1]))
            )
        {
            // Render the corner
            Vector3 cornerPosition = new Vector3(origin.x + (xDirection * DebugScaleFactor * 0.45f), -0.5f, origin.z + (zDirection * DebugScaleFactor * 0.45f));
            GameObject cornerInstaObj = Instantiate(CornerModel, cornerPosition, CornerModel.transform.rotation);
            cornerInstaObj.transform.localScale = Vector3.Scale(cornerInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
        }
    }

    bool CheckIfRoomOnEdge(int[,] side, int row, int col)
    {
        // Check if the room lies within the edge list
        for(int i = 0; i < side.GetLength(0); i++)
        {
            if (side[i, 0] == row && side[i, 1] == col)
            {
                return true;
            }
        }
        return false;
    }

    void PruneAvailableDirections(int directionIdx, int direction, int[] availablePaths)
    {
        // Remove available directions depending on the direction
        switch (direction)
        {
            case 0:
                if (currentRoom[0] - 1 < 0)
                {
                    // if its not in the grid then remove it from the available paths
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 1:
                if (currentRoom[1] + 1 >= grid.EdgeSize)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 2:
                if (currentRoom[0] + 1 >= grid.EdgeSize)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 3:
                if (currentRoom[1] - 1 < 0)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            default:
                break;
        }
    }

    int GetParentRoomDirection(GridRoom room)
    {
        // Get direction from the current room to its parent room
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        if (room.parent.coordinate.SequenceEqual(northCoordinate))
        {
            return 0;
        }
        if (room.parent.coordinate.SequenceEqual(eastCoordinate))
        {
            return 1;
        }
        if (room.parent.coordinate.SequenceEqual(southCoordinate))
        {
            return 2;
        }
        if (room.parent.coordinate.SequenceEqual(westCoordinate))
        {
            return 3;
        }
        return -1;
    }

    void UpdateSurroundingAvailableDirections(int[] currentCoordinate)
    {
        // Update the surronding rooms available directions around currently looked at room
        int[] northCoordinate = new int[] {currentCoordinate[0] - 1, currentCoordinate[1] };
        int[] eastCoordinate =  new int[] {currentCoordinate[0], currentCoordinate[1] + 1 };
        int[] southCoordinate = new int[] {currentCoordinate[0] + 1, currentCoordinate[1] };
        int[] westCoordinate =  new int[] {currentCoordinate[0], currentCoordinate[1] - 1 };

        GridRoom northDirectionRoom = GetRoom(gridDirectionParentNode, northCoordinate);
        GridRoom eastDirectionRoom = GetRoom(gridDirectionParentNode, eastCoordinate);
        GridRoom southDirectionRoom = GetRoom(gridDirectionParentNode, southCoordinate);
        GridRoom westDirectionRoom = GetRoom(gridDirectionParentNode, westCoordinate);

        if (northDirectionRoom != null)
        {
            northDirectionRoom.availableDirections.Remove(2);
        }

        if (eastDirectionRoom != null)
        {
            eastDirectionRoom.availableDirections.Remove(3);
        }

        if (southDirectionRoom != null)
        {
            southDirectionRoom.availableDirections.Remove(0);
        }

        if (westDirectionRoom != null)
        {
            westDirectionRoom.availableDirections.Remove(1);
        }
    }

    public Vector3 GetStartRoomPosition()
    {
        // Get the levels starting room
        return new Vector3(DebugScaleFactor * startRoom[0], 0f, DebugScaleFactor * startRoom[1]);
    }
}
