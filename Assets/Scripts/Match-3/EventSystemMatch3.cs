using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemMatch3 : MonoBehaviourSingleton <EventSystemMatch3>
{
    public Action onMatchFind;

    public void MatchFind()
    {
        onMatchFind.Invoke();
    }
}

