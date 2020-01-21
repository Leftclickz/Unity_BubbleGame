using UnityEngine;
using System.Collections.Generic;

public class CubeGrid : MonoBehaviour
{
    public int GridDimX = 15;
    public int GridDimY = 10;
    public float GridSpacing = 1.2f;
    public GameObject CubePrefab;

    public bool CalculatedCluster;

    void Start()
    {
        //Create a 2D array to hold the cubes, then generate the cubes in it
        m_Grid = new ClickableCube[GridDimX, GridDimY];

        //Create a grid of visited cells
        m_VisitedCells = new bool[GridDimX, GridDimY];

        GenerateCubes();

        m_CubeClusters = new List<CubeCluster>();

        CalculatedCluster = false;
    }

    void Update()
    {

    }

    public void RecalcuateClusters()
    {
        //Clear all clusters
        m_CubeClusters.Clear();

        //Clear all visited cells
        ClearVisitedCells();

        //Create a container so we can store all cell that needs to be visited
        List<IntVector2> cellsToCheck = new List<IntVector2>();
        IntVector2 startCoords;

        //Use function that can find non-visited cells
        //The logic below must be executed until all cells are visited
        while (FindNonVisitedCoord(out startCoords))
        {
            //Create cell cluster and add it to the existing list of clusters
            CubeCluster newCluster = new CubeCluster();
            m_CubeClusters.Add(newCluster);

            //Add the same cell to the container which stores a coordinates/cells that needs to be visited
            newCluster.AddCube(m_Grid[startCoords.x, startCoords.y]);

            cellsToCheck.Add(startCoords);

            while (cellsToCheck.Count > 0)
            {
                //remove the initial start value but keep in temp memory
                int indexToRemove = cellsToCheck.Count - 1;
                IntVector2 currentCoord = cellsToCheck[indexToRemove];
                cellsToCheck.RemoveAt(indexToRemove);

                //ignore this cube if its been visited
                if (m_VisitedCells[currentCoord.x, currentCoord.y] == true)
                {
                    continue;
                }
                //flag visible otherwise
                else
                {
                    m_VisitedCells[currentCoord.x, currentCoord.y] = true;
                }

                //List<IntVector2> cardinalCoordsToCheck = new List<IntVector2>();
                //for (int i = 0; i < 4; i++)
                //{
                //    int x = i < 2 ? 1 : 0;
                //    int y = i > 2 ? 1 : 0;

                //    AddCoordsIfNeeded(currentCoord, new IntVector2(x, y), ref cardinalCoordsToCheck);
                //}

                //for (int i = 0; i < cardinalCoordsToCheck.Count; i++)
                //{
                //    ClickableCube cube = m_Grid[cardinalCoordsToCheck[i].x, cardinalCoordsToCheck[i].y];
                //    if (cube.Activated == false)
                //    {
                //        continue;
                //    }

                //    newCluster.AddCube(cube);
                //    cellsToCheck.Add(cardinalCoordsToCheck[i]);
                //}


                //for loop for the cardinal directions
                for (int i = 0; i < 4; i++)
                {
                    //create coords
                    IntVector2 potentialCoords = currentCoord;

                    //check a cardinal direction based on index
                    switch (i)
                    {
                        case 0:
                            potentialCoords.x++;
                            break;
                        case 1:
                            potentialCoords.x--;
                            break;
                        case 2:
                            potentialCoords.y++;
                            break;
                        case 3:
                            potentialCoords.y--;
                            break;
                        default:
                            Debug.LogError("Something really fucked up happened.");
                            break;
                    }

                    //check if coords are legal
                    if (AreCoordsValid(potentialCoords) == false)
                    {
                        continue;
                    }

                    //grab the cube and check if its been activated
                    ClickableCube potential = m_Grid[potentialCoords.x, potentialCoords.y];
                    if (potential.Activated == false)
                    {
                        continue;
                    }

                    //add the cube to the cluster and add it to the need-to-check list
                    newCluster.AddCube(potential);
                    cellsToCheck.Add(potentialCoords);
                }

            }

        }
    }

    //A helper function to add new coordinates to check in our search.
    //It will first create the new coords, then double check if the coordinates are valid before adding 
    //them to the list
    void AddCoordsIfNeeded(IntVector2 coords, IntVector2 checkDir, ref List<IntVector2> coordsToVisit)
    {
        IntVector2 nextCoords = coords + checkDir;

        if (AreCoordsValid(nextCoords))
        {
            coordsToVisit.Add(nextCoords);
        }
    }

    //This is a helper function to check if a set of coordinates are valid.  (out of bounds)
    bool AreCoordsValid(IntVector2 coords)
    {
        return coords.x >= 0 && coords.y >= 0 &&
            coords.x < m_Grid.GetLength(0) && coords.y < m_Grid.GetLength(1);
    }

    //Sets all of the visited cells back to non-visited
    void ClearVisitedCells()
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                m_VisitedCells[x, y] = false;
            }
        }
    }

    //Finds a non-visited coordinate where we can start a search
    //it will return true if a non-vistited coordinate was found where we can start searching
    //
    //Note: This starts looking at the start of the grid every time.  One potential optimization
    //      could be to keep track of where you stopped looking last time, and then pick up from
    //      this location next time the function is executed.
    bool FindNonVisitedCoord(out IntVector2 nonVisitedCoord)
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                if (m_Grid[x, y].Activated && !m_VisitedCells[x, y])
                {
                    nonVisitedCoord = new IntVector2(x, y);

                    return true;
                }
            }
        }

        //No non-visited activated coords found.  Set the value to an invalid coordinate
        //and return false
        nonVisitedCoord = new IntVector2(-1, -1);

        return false;
    }



    //Creates the cubes in the right position and puts them in the grid    
    private void GenerateCubes()
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                Vector3 offset = new Vector3(x * GridSpacing, y * GridSpacing, 0.0f);

                GameObject cubeObj = (GameObject)GameObject.Instantiate(CubePrefab);

                cubeObj.transform.position = offset + transform.position;

                cubeObj.transform.parent = transform;

                m_Grid[x, y] = cubeObj.GetComponent<ClickableCube>();

                DebugUtils.Assert(m_Grid[x, y] != null, "Could not find clickableCube component.");
            }
        }
    }

    //Using a 2D array to represent the grid
    ClickableCube[,] m_Grid;
    bool[,] m_VisitedCells;

    List<CubeCluster> m_CubeClusters;
}
