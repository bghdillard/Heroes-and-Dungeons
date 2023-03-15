using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public static class DungeonBuilder
{
    public static IEnumerator BuildGrid(Cell[,,] grid, int activeLayer, GameObject worldGeography, GameObject cellHolder, float xOffset, float yOffset, float seed) //In the future, might use this to randomize the placement of, say, for example, gold and metal ores, and other special resources. Hey, past me, guess what... You're Right!
    {
        GameObject temp;
        int goldCount = 0;
        int ironCount = 0;
        int stoneCount = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    float toUse = Mathf.PerlinNoise((x + y + xOffset) * seed, (z + y + yOffset) * seed);
                    if (0.74 < toUse && toUse < 0.81 || 0.86 < toUse && toUse < 0.96) //generate gold
                    {
                        goldCount++;
                        temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Gold Ore"), cellHolder.transform);
                        Cell cell = temp.GetComponent<Cell>();
                        grid[x, y, z] = cell;
                        temp.transform.position = new Vector3(x, y, z);
                        if (y == activeLayer)
                        {
                            temp.layer = 6;
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 6;
                            if (x == 0) cell.ShowSide(2, true);
                            else if (x == grid.GetLength(0) - 1) cell.ShowSide(1, true);
                            if (z == 0) cell.ShowSide(3, true);
                            else if (z == grid.GetLength(2) - 1) cell.ShowSide(4, true);                     
                            /*
                            if (z == 0) cell.ShowSide(1);
                            if (z == grid.GetLength(0) - 1) cell.ShowSide(2);
                            if (x == 0) cell.ShowSide(3);
                            if (x == grid.GetLength(2) - 1) cell.ShowSide(4);
                            */
                        }
                        else
                        {
                            temp.layer = 8;
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 8;
                            cell.ShowSide(0, true);
                        }
                        
                    }
                    else if (0.56 < toUse && toUse < 0.59 || 0.61 < toUse && toUse < 0.66) //generate iron
                    {
                        ironCount++;
                        temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Iron Ore"), cellHolder.transform);
                        Cell cell = temp.GetComponent<Cell>();
                        grid[x, y, z] = cell;
                        temp.transform.position = new Vector3(x, y, z);
                        if (y == activeLayer)
                        {
                            temp.layer = 6;
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 6;
                            if (x == 0) cell.ShowSide(2, true);
                            else if (x == grid.GetLength(0) - 1) cell.ShowSide(1, true);
                            if (z == 0) cell.ShowSide(3, true);
                            else if (z == grid.GetLength(2) - 1) cell.ShowSide(4, true);
                        }
                        else
                        {
                            temp.layer = 8;
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 8;
                            cell.ShowSide(0, true);
                        }
                    }
                    else
                    {
                        stoneCount++;
                        if (y == activeLayer) //If at the top of the dungeon, use a stone that prevents a navmesh from generating on top
                        {
                            temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Impassable Stone"), cellHolder.transform);
                            Cell cell = temp.GetComponent<Cell>();
                            grid[x, y, z] = cell;
                            temp.transform.position = new Vector3(x, y, z);
                            temp.layer = 6;
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 6;
                            if (x == 0) cell.ShowSide(2, true);
                            else if (x == grid.GetLength(0) - 1) cell.ShowSide(1, true);
                            if (z == 0) cell.ShowSide(3, true);
                            else if (z == grid.GetLength(2) - 1) cell.ShowSide(4, true);
                        }
                        else
                        {
                            temp = Object.Instantiate(Resources.Load<GameObject>("Cells/Stone"), cellHolder.transform); //On game start, set all possible slots to plain stone.
                            Cell cell = temp.GetComponent<Cell>();
                            grid[x, y, z] = cell;
                            temp.transform.position = new Vector3(x, y, z);
                            temp.layer = 8; //set to be invisible and non interactive
                            for (int i = 0; i < temp.transform.childCount; i++) temp.transform.GetChild(i).gameObject.layer = 8;
                            cell.ShowSide(0, true);
                        }
                    }
                }
            }
            yield return null;
        }
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(-53, activeLayer * 4, 53);
        camera.transform.LookAt(new Vector3(-49, activeLayer * 2, 49));
        worldGeography.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        BuilderController builderController = GameObject.Find("GameController").GetComponent<BuilderController>();
        for (int i = 0; i < 4; i++)
        {
            temp = Object.Instantiate(Resources.Load<GameObject>("Monster/Undead/Builder"));
            builderController.AddBuilder(temp.GetComponent<Builder>());
            if (i % 2 == 0) temp.GetComponent<NavMeshAgent>().Warp(new Vector3(-49 + i * 2 - 2, activeLayer, +49));
            else temp.GetComponent<NavMeshAgent>().Warp(new Vector3(-49, activeLayer, 49 + (i - 1) * 2 - 2));
        }
        Debug.Log("Gold Count: " + goldCount);
        Debug.Log("Iron Count: " + ironCount);
        Debug.Log("Stone Count: " + stoneCount);
    }
}
