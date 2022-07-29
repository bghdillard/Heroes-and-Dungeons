using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    private Cell[,  ,] grid;
    private int activeLayer;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Cell[100, 10, 100];
        activeLayer = 3;
        int centerX = Random.Range(20, 71);
        int centerZ = Random.Range(20, 71); //Select a random x and z coordinate to be the center of the initial dungeon
        GameObject temp;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    //If at the square forming the initial dungeon...
                    if ((x == centerX || x == centerX - 1 || x == centerX + 1) && (z == centerZ || z == centerZ - 1 || z == centerZ + 1))
                    {
                        Debug.Log(centerX);
                        Debug.Log(centerZ);
                        if (y == activeLayer)//...and at the right layer, set those cells to be the initial room...
                        {
                            temp = Instantiate(Resources.Load<GameObject>("Cells/Empty"));
                            grid[x, y, z] = temp.GetComponent<Cell>();
                            temp.transform.position = new Vector3(x, y * 2, z);
                            temp.layer = 6;
                        }
                        else
                        {
                            temp = Instantiate(Resources.Load<GameObject>("Cells/Stone")); //On game start, set all possible slots to plain stone.
                            grid[x, y, z] = temp.GetComponent<Cell>();
                            temp.transform.position = new Vector3(x, y * 2, z);
                            if (y == activeLayer - 1) temp.layer = 7; //if directly below the initial active layer, set to be visible but non interactive
                            else temp.layer = 8; //otherwise, set to be invisible and non interactive
                        }
                    }
                    else
                    {
                        temp = Instantiate(Resources.Load<GameObject>("Cells/Stone")); //On game start, set all possible slots to plain stone.
                        grid[x, y, z] = temp.GetComponent<Cell>();
                        temp.transform.position = new Vector3(x, y * 2, z);
                        if (y == activeLayer) temp.layer = 6; //If at the initial active layer, set to be visible and interactive
                        else temp.layer = 8; //otherwise, set to be invisible and non interactive
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
