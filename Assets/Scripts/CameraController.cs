using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera redCam;
    public Camera blackCam;
    public bool isBlack = false;

    // Start is called before the first frame update
    void Start()
    {
        if (FindGameButton.PlayerID == FindGameButton.blackPlayer)
        {
            isBlack = true;
        }
        
        if (isBlack)
        {
            redCam.enabled = false;
            blackCam.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
