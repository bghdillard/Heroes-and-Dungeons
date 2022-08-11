using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class DungeonBuilder
{
    public static IEnumerator BuildGrid(Cell[,,] grid, int activeLayer, GameObject worldGeography) //In the future, might use this to randomize the placement of, say, for example, gold and metal ores, and other special resources
    {
        GameObject temp;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (y == activeLayer) //If at the top of the dungeon, use a stone that prevents a navmesh from generating on top
                    {
                        temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Impassable Stone"), worldGeography.transform);
                        grid[x, y, z] = temp.GetComponent<Cell>();
                        temp.transform.position = new Vector3(x, y, z);
                        temp.layer = 6;
                    }
                    else
                    {
                        temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Stone"), worldGeography.transform); //On game start, set all possible slots to plain stone.
                        grid[x, y, z] = temp.GetComponent<Cell>();
                        temp.transform.position = new Vector3(x, y, z);
                        temp.layer = 8; //set to be invisible and non interactive
                    }
                }
            }
            yield return null;
        }
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(-53, activeLayer * 4, 53);
        camera.transform.LookAt(new Vector3(-49, activeLayer * 2, 49));
        worldGeography.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        for (int i = 0; i < 4; i++)
        {
            temp = Object.Instantiate(Resources.Load<GameObject>("Undead/Builder"));
            if (i % 2 == 0) temp.GetComponent<NavMeshAgent>().Warp(new Vector3(-49 + i * 2 - 2, activeLayer, +49));
            else temp.GetComponent<NavMeshAgent>().Warp(new Vector3(-49, activeLayer, 49 + (i - 1) * 2 - 2));
        }
    }
}
