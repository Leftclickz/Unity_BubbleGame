using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        m_Rotation = gameObject.transform.rotation.eulerAngles;
        m_SnapshotRotation = gameObject.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (InvertControls == true)
            m_Rotation -= GetMovementAxis();
        else
            m_Rotation += GetMovementAxis();

        if (m_Rotation.z > (m_SnapshotRotation.x + m_RotationLimit))
        {
            m_Rotation.z = m_SnapshotRotation.x + m_RotationLimit;
        }
        else if (m_Rotation.z < (m_SnapshotRotation.x - m_RotationLimit))
        {
            m_Rotation.z = m_SnapshotRotation.x - m_RotationLimit;
        }

        gameObject.transform.rotation = Quaternion.Euler(m_Rotation);

        if (CurrentTip == null && CurrentLiveAmmo == null)
        {
            CurrentTip = Instantiate(CannonShotPrefab);
            CurrentTip.GetComponent<ColoredCube>().WasFired = true;
        }

        if (CurrentTip != null)
        {
            CurrentTip.transform.position = ShotPoint.transform.position;
            CurrentTip.transform.rotation = ShotPoint.transform.rotation;
        }

        if (IsShooting() && CurrentLiveAmmo == null)
        {
            CurrentLiveAmmo = CurrentTip;
            CurrentTip = null;

            CurrentLiveAmmo.transform.position = ShotPoint.transform.position;
            CurrentLiveAmmo.transform.rotation = ShotPoint.transform.rotation;

            CurrentLiveAmmo.GetComponent<ColoredCube>().PhysicsBody.AddForce(CurrentLiveAmmo.transform.up * CannonPower);
        }
    }


    public Vector3 GetMovementAxis()
    {
        return new Vector3(0.0f, 0.0f, Input.GetAxis("Horizontal")) * m_Sensitity * Time.fixedDeltaTime;
    }

    public bool IsShooting()
    {
        return Input.GetKey(KeyCode.Space);
    }

    #region Rotation Vars

    [SerializeField]
    private Vector3 m_Rotation;

    private Vector3 m_SnapshotRotation;

    [SerializeField]
    private float m_RotationLimit = 60.0f;
    [SerializeField]
    private float m_Sensitity = 15.0f;
    [SerializeField]
    private bool InvertControls = true;

    #endregion

    public float CannonPower = 2.5f;
    public GameObject CannonShotPrefab = null;
    public Transform ShotPoint = null;

    public GameObject CurrentLiveAmmo = null;
    private GameObject CurrentTip = null;
}
