using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
    }
}
