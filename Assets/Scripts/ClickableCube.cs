using UnityEngine;
using System.Collections;

public class ClickableCube : MonoBehaviour
{

    void Start()
    {
        m_ActivatedColor = Color.white;

        //Choose randomly if the cube should start activated or not
        Activated = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
    }

    void Update()
    {

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
        }
    }

    private bool m_Activated;
    private Color m_ActivatedColor;
}
