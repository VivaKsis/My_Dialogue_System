using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClicksEvent : MonoBehaviourSingleton <PlayerClicksEvent>
{
    public Action onClick;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            onClick?.Invoke();
        }
    }
}
