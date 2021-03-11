using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook)), DisallowMultipleComponent]
public class CinemachineManualFreeLook : MonoBehaviour
{
    public CinemachineInputAxisDriver xAxis;
    public CinemachineInputAxisDriver yAxis;

    private CinemachineFreeLook freeLook;
    private CinemachineFreeLook.Orbit[] originalOrbits;

    public float minRadius;
    public float maxRadius;
    public float zoomSpeed; //change in zoom multiplier (0.05)
    public float zoomX; //current zoom multiplier
    private Vector2 scroll = Vector2.zero;

    public void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
        xAxis.multiplier = PlayerPrefs.GetFloat("playerSens") * -1;
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            return -UnityEngine.Input.GetAxis("Mouse X");
        }
        else if (axisName == "Mouse Y")
        {
            return -UnityEngine.Input.GetAxis("Mouse Y");
        }
        return UnityEngine.Input.GetAxis(axisName);
    }


    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.m_XAxis.m_MaxSpeed = freeLook.m_XAxis.m_AccelTime = freeLook.m_XAxis.m_DecelTime = 0;
        freeLook.m_XAxis.m_InputAxisName = string.Empty;
        freeLook.m_YAxis.m_MaxSpeed = freeLook.m_YAxis.m_AccelTime = freeLook.m_YAxis.m_DecelTime = 0;
        freeLook.m_YAxis.m_InputAxisName = string.Empty;

        originalOrbits = new CinemachineFreeLook.Orbit[freeLook.m_Orbits.Length]; //array of all rig orbits (3)
        zoomX = 1;
        for (int i = 0; i < freeLook.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = freeLook.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = freeLook.m_Orbits[i].m_Radius;
        }
    }

    private void OnValidate()
    {
        xAxis.Validate();
        yAxis.Validate();
    }

    private void Reset()
    {
        xAxis = new CinemachineInputAxisDriver
        {
            multiplier = -10f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse X",
        };
        yAxis = new CinemachineInputAxisDriver
        {
            multiplier = 0.1f,
            accelTime = 0.1f,
            decelTime = 0.1f,
            name = "Mouse Y",
        };
    }

    private void Update()
    {
        bool changed = yAxis.Update(Time.deltaTime, ref freeLook.m_YAxis);
        changed |= xAxis.Update(Time.deltaTime, ref freeLook.m_XAxis);
        if (changed)
        {
            freeLook.m_RecenterToTargetHeading.CancelRecentering();
            freeLook.m_YAxisRecentering.CancelRecentering();
        }

        scroll = UnityEngine.Input.mouseScrollDelta;
        if (scroll.y > 0f && zoomX - zoomSpeed >= minRadius)
        {
            zoomX -= zoomSpeed;
            for (int i = 0; i < freeLook.m_Orbits.Length; i++)
            {
                freeLook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomX;
            }
        }
        else if (scroll.y < 0f && zoomX + zoomSpeed <= maxRadius)
        {
            zoomX += zoomSpeed;
            for (int i = 0; i < freeLook.m_Orbits.Length; i++)
            {
                freeLook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomX;
            }
        }
    }
}
