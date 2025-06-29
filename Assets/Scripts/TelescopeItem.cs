using UnityEngine;
using Cinemachine;

public class TelescopeItem : MonoBehaviour
{
    private bool collected = false;

    public CinemachineVirtualCamera virtualCamera;
    public float zoomAmount = 5f; 

    public bool GetCollected() => collected;
    public void SetCollected(bool value) => collected = value;

    public float GetCameraDistance()
    {
        var framing = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        return framing.m_CameraDistance;
    }

    public void SetCameraDistance(float value)
    {
        var framing = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framing.m_CameraDistance = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Ant")) return;

        collected = true;
        gameObject.SetActive(false);

        var framing = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        framing.m_CameraDistance += zoomAmount;  // ✅ 增加而不是设置

    }
}
