using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera)), DisallowMultipleComponent]
public class CinemachineVirtualCam : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer virtualCamFrameTransposer;
    private CinemachinePOV virtualCamPOV;

    public float minZoom;
    public float maxZoom;
    public float defaultZoom;
    public float currentZoom;

    // Start is called before the first frame update
    void Start()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        virtualCamPOV = virtualCam.GetCinemachineComponent<CinemachinePOV>();
        virtualCamFrameTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        virtualCamFrameTransposer.m_CameraDistance = defaultZoom;
        currentZoom = defaultZoom;
        if (virtualCamPOV) virtualCamPOV.m_HorizontalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("playerSens");
    }

    public void ZoomIn()
    {
        if ((currentZoom -= 0.5f) >= minZoom)
        {
            virtualCamFrameTransposer.m_CameraDistance -= 0.5f;
            currentZoom = virtualCamFrameTransposer.m_CameraDistance;
        }
    }

    public void ZoomOut()
    {
        if ((currentZoom += 0.5f) <= maxZoom)
        {
            virtualCamFrameTransposer.m_CameraDistance += 0.5f;
            currentZoom = virtualCamFrameTransposer.m_CameraDistance;
        }
    }

    public void UpdateSensitivity()
    {
        if (virtualCamPOV) virtualCamPOV.m_HorizontalAxis.m_MaxSpeed = PlayerPrefs.GetFloat("playerSens");
    }

    public void SetTargetLock(GameObject targetLock)
    {
        if (targetLock != null)
        {
            virtualCam.m_LookAt = targetLock.transform.GetChild(0).transform;
            virtualCam.m_Priority = 15;
        }
    }

    public void UnsetTargetLock()
    {
        virtualCam.m_LookAt = null;
        virtualCam.m_Priority = 0;
    }
}
