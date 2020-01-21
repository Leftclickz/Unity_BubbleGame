using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    private const float PointerRayDist = 1000.0f;

    void Start()
    {
        m_CubeGrid = GameObject.Find("ColoredCubeGrid").GetComponent<ColoredCubeGrid>();
    }

    void Update()
    {
        //Handling input
        if (Input.GetMouseButton(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        //Use a ray cast to figure what cube we are clicking
        Ray pointerRay = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (!Physics.Raycast(pointerRay, out hitInfo, PointerRayDist))
        {
            return;
        }

        //Get the cube object or ignore the object if it isn't one.
        ColoredCube clickableCube = hitInfo.collider.gameObject.GetComponent<ColoredCube>();
        if (clickableCube == null)
        {
            return;
        }

        if (clickableCube.m_Owner != null)
        {
            clickableCube.m_Owner.DeactivateAll();
        }

    }

    ColoredCubeGrid m_CubeGrid;
    bool m_ActivateCubes;
}
