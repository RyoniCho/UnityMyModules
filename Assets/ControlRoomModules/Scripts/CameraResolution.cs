using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    float currentRatio;
    UnityEngine.U2D.PixelPerfectCamera pixelPerfectCamera;

    private void Awake()
    {
        this.pixelPerfectCamera = this.GetComponent<UnityEngine.U2D.PixelPerfectCamera>();

        SetResolution();

    }

    private void SetResolution()
    {
        currentRatio = (float)Screen.height / (float)Screen.width;

        float y = currentRatio * this.pixelPerfectCamera.refResolutionX;

        int fixedY = (int)y - ((int)y % 2);
        this.pixelPerfectCamera.refResolutionY = fixedY;
    }
}
