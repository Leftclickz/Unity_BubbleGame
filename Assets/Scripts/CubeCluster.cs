using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeCluster
{

    public CubeCluster()
    {

        m_Cubes = new List<ClickableCube>();

        if (GameObject.Find("CubeGrid").GetComponent<CubeGrid>())
        {
            //Choosing a random color for the cluster
            m_ClusterColor = new Color(
                UnityEngine.Random.Range(0.25f, 1.0f),
                UnityEngine.Random.Range(0.25f, 1.0f),
                UnityEngine.Random.Range(0.25f, 1.0f)
                );

            GameObject.Find("CubeGrid").GetComponent<CubeGrid>().CalculatedCluster = true;
        }

    }

    public void AddCube(ClickableCube cube)
    {
        m_Cubes.Add(cube);

        cube.ActivatedColor = m_ClusterColor;
    }

    List<ClickableCube> m_Cubes;

    Color m_ClusterColor;
}

public class ColoredCubeCluster
{

    public ColoredCubeCluster(Color ClusterColor)
    {
        m_Cubes = new List<ColoredCube>();

        if (GameObject.Find("ColoredCubeGrid").GetComponent<ColoredCubeGrid>())
        {
            GameObject.Find("ColoredCubeGrid").GetComponent<ColoredCubeGrid>().CalculatedCluster = true;
        }

        ActivatedColor = ClusterColor;
    }

    public void AddCube(ColoredCube cube)
    {
        m_Cubes.Add(cube);
        cube.m_Owner = this;
    }

    public void AddCube(ColoredCube cube, ColoredCubeGrid grid, IntVector2 coords)
    {
        grid.FlagSet(coords);
        AddCube(cube);
    }

    public void DeactivateAll()
    {
        foreach (var cube in m_Cubes)
        {
            cube.Activated = false;
        }
    }

    List<ColoredCube> m_Cubes;
    public Color ActivatedColor;
}
