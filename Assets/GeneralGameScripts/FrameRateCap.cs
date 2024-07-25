using UnityEngine;

public class FramerateCap : MonoBehaviour
{
    void Start()
    {
        // Disable VSync (necessary to ensure frame rate is not affected by VSync)
        QualitySettings.vSyncCount = 0;
        // Set target frame rate to 30 FPS
        Application.targetFrameRate = 30;

        
    }
}