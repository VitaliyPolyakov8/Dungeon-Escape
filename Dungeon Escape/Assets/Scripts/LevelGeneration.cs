using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class LevelGeneration : MonoBehaviour
{
    private NavMeshSurface Surfaces;

    [Header("Lvl Generation")]
    public int columns = 10; 
    public int rows = 10; 
    public int wallCount = 22; 
    public int enemyCount = 2; 
    public List<Vector3> wallPositions = new List<Vector3>(); 

    [Header("Prefabs")]
    public GameObject exit; 
    public GameObject floorPrefabs; 
    public GameObject wallPrefabs; 
    public GameObject enemyPrefabs;
    public GameObject outerWallPrefab; 

    private Transform _outerWalls; 
    private Transform _floor;
    private Transform _walls;
    private Transform _enemies;



    public void Start()
    {
        SceneSetup();
    }
    void InitialiseList()
    {
        wallPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int z = 1; z < rows - 1; z++)
            {
                wallPositions.Add(new Vector3(x, 0, z));
            }
        }
    }

    void BoardGeneration()
    {
        _floor = new GameObject("Board").transform;
        _outerWalls = new GameObject("OuterWalls").transform;

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                GameObject toInstantiate = floorPrefabs;
                if (x == 0 || x == columns - 1  || z == 0 || z == rows - 1 )
                {
                    if(z == 0)
                        Instantiate(outerWallPrefab, new Vector3(x + 1 , 0, z ), Quaternion.AngleAxis(180, new Vector3(0,1,0))).transform.SetParent(_outerWalls);
                    if (z == rows - 1 && z != 9)
                        Instantiate(outerWallPrefab, new Vector3(x , 0, z ), Quaternion.AngleAxis(180, new Vector3(0, 1, 0))).transform.SetParent(_outerWalls);
                    if(z == 9)
                        Instantiate(outerWallPrefab, new Vector3(x + 1, 0, z + 1), Quaternion.AngleAxis(180, new Vector3(0, 1, 0))).transform.SetParent(_outerWalls);
                    if (x == 0)
                        Instantiate(outerWallPrefab, new Vector3(x , 0, z + 1 ), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))).transform.SetParent(_outerWalls);
                    if (x == columns - 1 && x != 9)
                        Instantiate(outerWallPrefab, new Vector3(x , 0, z ), Quaternion.AngleAxis(-90, new Vector3(0, 1, 0))).transform.SetParent(_outerWalls);
                    if (x == 9)
                        Instantiate(outerWallPrefab, new Vector3(x + 1, 0, z + 1), Quaternion.AngleAxis(90, new Vector3(0, 1, 0))).transform.SetParent(_outerWalls);


                }

                Instantiate(toInstantiate, new Vector3(x,0,z), Quaternion.identity).transform.SetParent(_floor);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, wallPositions.Count);

        Vector3 randomPosition = wallPositions[randomIndex];

        wallPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject Prefab, int count, Transform parentTransform, string parentName)
    {
        parentTransform = new GameObject(parentName).transform;
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = RandomPosition();

            GameObject tileChoice = Prefab;

            Instantiate(tileChoice, randomPosition, Quaternion.identity).transform.SetParent(parentTransform);
        }
    }
    public void SceneSetup()
    {
        BoardGeneration();

        InitialiseList();

        LayoutObjectAtRandom(wallPrefabs, wallCount , _walls, "Walls");
        Surfaces = GameObject.FindGameObjectWithTag("Navigation").GetComponent<NavMeshSurface>();
        Surfaces.BuildNavMesh();
        LayoutObjectAtRandom(enemyPrefabs, enemyCount, _enemies, "Enemies");

        Instantiate(exit, new Vector3(columns - 1, 0.05f, rows - 1), Quaternion.identity);
    }
}
