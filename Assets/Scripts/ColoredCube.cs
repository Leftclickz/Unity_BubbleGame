using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ColoredCube : MonoBehaviour
{


    public ColoredCube()
    {
        //ActivatedColor = ColoredCubeGrid.GetColor();
    }

    public void Awake()
    {
        ActivatedColor = ColoredCubeGrid.GetColor();
        PhysicsBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (WasFired == false)
        {
            gameObject.transform.position = LockedPosition;
        }
    }

    public bool Activated
    {
        get
        {
            return m_Activated;
        }

        set
        {
            m_Activated = value;

            //Update the visuals since the activated value changes
            UpdateVisuals();
        }
    }

    public Color ActivatedColor
    {
        get
        {
            return m_ActivatedColor;
        }

        set
        {
            m_ActivatedColor = value;

            //Update the visuals since the color changed
            UpdateVisuals();
        }
    }

    public void ToggleActivated()
    {
        m_Activated = !m_Activated;

        //Update the visuals since the activated value changes
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (GetComponent<Renderer>().material == null)
        {
            return;
        }

        //Set the material color 
        if (m_Activated)
        {
            GetComponent<Renderer>().material.color = m_ActivatedColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.black;
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PhysicsBody.velocity = Vector3.zero;
        PhysicsBody.angularVelocity = Vector3.zero;

        if (WasFired)
        {
            WasFired = false;
            LockedPosition = gameObject.transform.position;

            GameObject.Find("Turret Base").GetComponent<Cannon>().CurrentLiveAmmo = null;
            GameObject.Find("ColoredCubeGrid").GetComponent<ColoredCubeGrid>().AdjustGrid(this);

            ColoredCube cubeObj = collision.collider.gameObject.GetComponent<ColoredCube>();

            if (ActivatedColor == cubeObj.ActivatedColor)
            {

                if (cubeObj.m_Owner != null)
                {
                    cubeObj.m_Owner.DeactivateAll();
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private bool m_Activated = true;
    private Color m_ActivatedColor;
    public ColoredCubeCluster m_Owner;

    public Vector3 LockedPosition = Vector3.zero;

    public Rigidbody PhysicsBody;
    public bool WasFired = false;
}
