using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;

    void Start()
    {
        EnableCamera(camera1);
        DisableCamera(camera2);
        DisableCamera(camera3);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCameras();
        }
    }

    void ToggleCameras()
    {
        if (camera1.enabled)
        {
            EnableCamera(camera2);
            DisableCamera(camera1);
            DisableCamera(camera3);
        }
        else if (camera2.enabled)
        {
            EnableCamera(camera3);
            DisableCamera(camera1);
            DisableCamera(camera2); 
        }
        else
        {
            EnableCamera(camera1);
            DisableCamera(camera2);
            DisableCamera(camera3);
        }
    }

    void EnableCamera(Camera cam)
    {
        cam.enabled = true;
    }

    void DisableCamera(Camera cam)
    {
        cam.enabled = false;
    }
}
