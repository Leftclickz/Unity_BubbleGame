using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredCubeGrid : MonoBehaviour
{

    static Color[] m_Colors = new Color[4];

    void Awake()
    {
        m_Colors[0] = Color.red;
        m_Colors[1] = Color.blue;
        m_Colors[2] = Color.green;
        m_Colors[3] = Color.yellow;
    }

    public static Color GetColor()
    {
        int choice = Random.Range(0, 4);
        return m_Colors[choice];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GridDimX = 15;
    public int GridDimY = 10;
    public float GridSpacing = 1.2f;
    public GameObject CubePrefab;

    public bool CalculatedCluster;

    void Start()
    {
        //Create a 2D array to hold the cubes, then generate the cubes in it
        m_Grid = new ColoredCube[GridDimX, GridDimY];

        //Create a grid of visited cells
        m_SetCells = new bool[GridDimX, GridDimY];

        GenerateCubes();

        m_CubeClusters = new List<ColoredCubeCluster>();

        CalculatedCluster = false;
        RecalcuateClusters();
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
            ColoredCubeCluster newCluster = new ColoredCubeCluster(m_Grid[startCoords.x, startCoords.y].ActivatedColor);
            m_CubeClusters.Add(newCluster);

            //Add the same cell to the container which stores a coordinates/cells that needs to be visited
            newCluster.AddCube(m_Grid[startCoords.x, startCoords.y], this, startCoords);

            cellsToCheck.Add(startCoords);

            while (cellsToCheck.Count > 0)
            {
                //remove the initial start value but keep in temp memory
                int indexToRemove = cellsToCheck.Count - 1;
                IntVector2 currentCoord = cellsToCheck[indexToRemove];
                cellsToCheck.RemoveAt(indexToRemove);

                if (m_Grid[currentCoord.x, currentCoord.y].m_Owner == null)
                {
                    continue;
                }

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

                    //check if this has been set already
                    if (m_SetCells[potentialCoords.x, potentialCoords.y] == true)
                    {
                        continue;
                    }

                    //grab the cube and check if it matches our color
                    ColoredCube potential = m_Grid[potentialCoords.x, potentialCoords.y];

                    if (potential)
                    {
                        if (potential.ActivatedColor != newCluster.ActivatedColor)
                        {
                            continue;
                        }


                        //add the cube to the cluster and add it to the need-to-check list
                        newCluster.AddCube(potential, this, potentialCoords);
                        cellsToCheck.Add(potentialCoords);
                    }

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

    public void FlagSet(IntVector2 coords)
    {
        m_SetCells[coords.x, coords.y] = !m_SetCells[coords.x, coords.y];
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
                m_SetCells[x, y] = false;
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
                if (m_Grid[x, y] && m_Grid[x, y].Activated && !m_SetCells[x, y])
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
                ColoredCube color = cubeObj.GetComponent<ColoredCube>();

                color.LockedPosition = offset + transform.position;
                cubeObj.transform.parent = transform;

                m_Grid[x, y] = cubeObj.GetComponent<ColoredCube>();

                DebugUtils.Assert(m_Grid[x, y] != null, "Could not find ColoredCube component.");
            }
        }
    }

    public void AdjustGrid(ColoredCube NewCube)
    {
        float x = NewCube.gameObject.transform.position.x - gameObject.transform.position.x;
        float y = NewCube.gameObject.transform.position.y - gameObject.transform.position.y;

        int xArrayVal = (int)Mathf.Round(x / GridSpacing);
        int yArrayVal = (int)Mathf.Round(y / GridSpacing);

        NewCube.transform.parent = gameObject.transform;
        NewCube.transform.position = new Vector3(xArrayVal * GridSpacing, yArrayVal * GridSpacing);
        NewCube.LockedPosition = NewCube.transform.position + transform.position;
        NewCube.WasFired = false;

        bool gridChanged = false;

        if (xArrayVal > GridDimX)
        {
        }
        else if (xArrayVal < gridResizeX)
        {
            ColoredCube[,] newGrid = new ColoredCube[GridDimX - xArrayVal, GridDimY];
            m_SetCells = new bool[GridDimX - xArrayVal, GridDimY];

            CopyOverGridDetailsToCopy(ref newGrid, new IntVector2(-xArrayVal, 0));

            newGrid[0, yArrayVal] = NewCube;
            gridResizeX--;

            m_Grid = newGrid;
            gridChanged = true;
        }

        if (yArrayVal < gridResizeY)
        {
            ColoredCube[,] newGrid = new ColoredCube[GridDimX, GridDimY - yArrayVal];
            m_SetCells = new bool[GridDimX, GridDimY - yArrayVal];

            CopyOverGridDetailsToCopy(ref newGrid, new IntVector2(0, -yArrayVal));

            newGrid[xArrayVal, 0] = NewCube;

            
            gridResizeY--;

            m_Grid = newGrid;
            gridChanged = true;
        }


        if (yArrayVal >= gridResizeY && xArrayVal >= gridResizeX)
        {
            m_Grid[xArrayVal -gridResizeX, yArrayVal -gridResizeY] = NewCube;
            gridChanged = true;
        }

        if (gridChanged)
        {
            RecalcuateClusters();
        }
    }

    private void CopyOverGridDetailsToCopy(ref ColoredCube[,] NewGrid, IntVector2 Offset)
    {
        for (int x = 0; x < GridDimX; x++)
            for (int y = 0; y < GridDimY; y++)
                NewGrid[x + Offset.x, y + Offset.y] = m_Grid[x, y];

        GridDimX += Offset.x;
        GridDimY += Offset.y;
    }

    //Using a 2D array to represent the grid
    ColoredCube[,] m_Grid;
    bool[,] m_SetCells;

    public List<ColoredCubeCluster> m_CubeClusters;

    private int gridResizeX = 0;
    private int gridResizeY = 0;
}
