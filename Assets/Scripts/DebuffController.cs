using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffController : MonoBehaviour
{

    public Camera MainCamera;
    public Camera DebuffCamera;
    public float timeSinceEat = 0;
    public float cameraDebuffDuration = 0;
    public float cameraDebuffMaxDuration = 10f;
    public float cameraDebuffActivationTime = 5f;
    private bool isCameraDebuffActive = false;

    void Start()
    {
        MainCamera.enabled = true;
        DebuffCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceEat += Time.deltaTime;
        if (timeSinceEat > cameraDebuffActivationTime && !isCameraDebuffActive)
        {
            Debug.Log("Trocando câmeras");
            SwitchCamerasToCameraDebuff();
        }

        if (isCameraDebuffActive)
        {
            cameraDebuffDuration += Time.deltaTime;
            Debug.Log($"cameraDebuffDuration: {cameraDebuffDuration}");

            if (cameraDebuffDuration >= cameraDebuffMaxDuration)
            {
                SwitchCamerasToDefault();
                cameraDebuffDuration = 0;
                timeSinceEat = 0;
            }
        }
    }
    void SwitchCamerasToCameraDebuff()
    {
        MainCamera.enabled = false;
        MainCamera.gameObject.SetActive(false);

        DebuffCamera.enabled = true;
        DebuffCamera.gameObject.SetActive(true);

        isCameraDebuffActive = true;

        Debug.Log("MainCamera disabled, DebuffCamera enabled");
        Debug.Log($"MainCamera.enabled: {MainCamera.enabled}");
        Debug.Log($"DebuffCamera.enabled: {DebuffCamera.enabled}");
    }

    public void SwitchCamerasToDefault()
    {
        MainCamera.enabled = true;
        MainCamera.gameObject.SetActive(true);

        DebuffCamera.enabled = false;
        DebuffCamera.gameObject.SetActive(false);

        isCameraDebuffActive = false;

        Debug.Log("MainCamera enabled, DebuffCamera disabled");
        Debug.Log($"MainCamera.enabled: {MainCamera.enabled}");
        Debug.Log($"DebuffCamera.enabled: {DebuffCamera.enabled}");
    }
}
