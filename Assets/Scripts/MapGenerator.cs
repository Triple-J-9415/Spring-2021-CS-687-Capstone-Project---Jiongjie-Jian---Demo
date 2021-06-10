using System;
using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Vector2 mapSize;
    public Transform mapHolder;
    [Range(0, 1)] public float outlinePercent;

    public GameObject obsPrefab;
    //public float obsCount;
    public List<Coord> allTileCoord = new List<Coord>();

    private Queue<Coord> ShuffledQueue;
    public float minObsHeight, maxObsHeight;

    [Header("Map Fully Accessible")]
    [Range(0, 1)] public float obsPercent;
    private Coord mapCenter; // Must be no obstacles at the center point
    bool[,] mapObstacles; // Determine whether there is an obstacle in the coordinate position

    [Header("NavMesh Agent")]
    public Vector2 mapMaxSize;
    public GameObject navMeshObs;
    public GameObject player;

    private void Start()
    {
        GenerateMap();
        Init();
    }

    // Initialize the player
    private void Init()
    {
        Instantiate(player, new Vector3(-mapSize.x / 2 + 0.5f + mapCenter.x, 0, -mapSize.y / 2 + 0.5f + mapCenter.y), Quaternion.identity);
    }

    private void GenerateMap()
    {
        // Generate tiles
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + i, 0, -mapSize.y / 2 + 0.5f + j);
                GameObject spawnTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0));
                spawnTile.transform.SetParent(mapHolder);
                spawnTile.transform.localScale *= (1 - outlinePercent);

                allTileCoord.Add(new Coord(i, j));
            }
        }

        // Shuffle Algorithm
        ShuffledQueue = new Queue<Coord>(Utillities.ShuffleCoords(allTileCoord.ToArray()));

        int obsCount = (int)(mapSize.x * mapSize.y * obsPercent);
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2); // Set the map center point
        mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];

        int currentObsCount = 0;

        // Generate obstacles
        for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            mapObstacles[randomCoord.x, randomCoord.y] = true;
            currentObsCount++;

            // Determine whether obstacles can be generated
            if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObsCount))
            {
                float obsHeight = Mathf.Lerp(minObsHeight, maxObsHeight, UnityEngine.Random.Range(0f, 0.5f)); // Random height of obstacles

                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + randomCoord.x, obsHeight / 2, -mapSize.y / 2 + 0.5f + randomCoord.y);
                GameObject spawnObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
                spawnObs.transform.SetParent(mapHolder);
                spawnObs.transform.localScale = new Vector3(1 - outlinePercent, obsHeight, 1 - outlinePercent);
            }
            else
            {
                mapObstacles[randomCoord.x, randomCoord.y] = false; // Random coordinate position without obstacles
                currentObsCount--;
            }
        }

        // Generate map boundary, air wall
        GameObject navMeshObsForward = Instantiate(navMeshObs, Vector3.forward * (mapMaxSize.y + mapSize.y) / 4, Quaternion.identity);
        navMeshObsForward.transform.localScale = new Vector3(mapSize.x, 5, (mapMaxSize.y / 2 - mapSize.y / 2));

        GameObject navMeshObsBack = Instantiate(navMeshObs, Vector3.back * (mapMaxSize.y + mapSize.y) / 4, Quaternion.identity);
        navMeshObsBack.transform.localScale = new Vector3(mapSize.x, 5, (mapMaxSize.y / 2 - mapSize.y / 2));

        GameObject navMeshObsLeft = Instantiate(navMeshObs, Vector3.left * (mapMaxSize.x + mapSize.x) / 4, Quaternion.identity);
        navMeshObsLeft.transform.localScale = new Vector3((mapMaxSize.x / 2 - mapSize.x / 2), 5, mapSize.y);

        GameObject navMeshObsRight = Instantiate(navMeshObs, Vector3.right * (mapMaxSize.x + mapSize.x) / 4, Quaternion.identity);
        navMeshObsRight.transform.localScale = new Vector3((mapMaxSize.x / 2 - mapSize.x / 2), 5, mapSize.y);
    }

    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)];

        Queue<Coord> queue = new Queue<Coord>(); // All coordinates are filtered and then stored in the queue
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true; // The center point has been detected

        int accessibleCount = 1;

        while (queue.Count > 0)
        {
            Coord currentTile = queue.Dequeue();

            for (int x = -1; x <= 1; x++) // Detect the X-axis coordinate points of adjacent surroundings
            {
                for (int y = -1; y <= 1; y++) // Detect the Y-axis coordinate points of adjacent surroundings
                {
                    int neighborX = currentTile.x + x;
                    int neighborY = currentTile.y + y;

                    if (x == 0 || y == 0) // Exclude the coordinates of the 45 degree angle
                    {
                        if (neighborX >= 0 && neighborX < _mapObstacles.GetLength(0) && neighborY >= 0 && neighborY < _mapObstacles.GetLength(1)) // Prevent adjacent points from exceeding the map boundary
                        {
                            if (!mapFlags[neighborX, neighborY] && !_mapObstacles[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                accessibleCount++;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                            }
                        }
                    }
                }
            }
        }

        int obsTargetCount = (int)(mapSize.x * mapSize.y - _currentObsCount);
        return accessibleCount == obsTargetCount;
    }

    // Ensure that the elements taken out of the queue are not repeated
    private Coord GetRandomCoord()
    {
        Coord randomCoord = ShuffledQueue.Dequeue(); // First in first out
        ShuffledQueue.Enqueue(randomCoord); // Put the removed element at the last; The queue size does not change
        return randomCoord; // Return the first element
    }
}

[System.Serializable] // Serialization Structure
// Initialize coordinates
public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }

    // Overloaded built-in algorithms
    public static bool operator !=(Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);
    }

    public static bool operator ==(Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.y) && (_c1.y == _c2.y);
    }
}

