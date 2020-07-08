﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectRedColision : MonoBehaviour
{
    private GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {

        Destroy(gameObject);

        gm.addAtCount(1);
        
    }
}
