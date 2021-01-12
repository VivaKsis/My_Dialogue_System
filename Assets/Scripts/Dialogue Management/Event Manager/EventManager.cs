using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor.Nodes;
using FlowReactor.Nodes.Unity;
using FlowReactor.Utils;
using System;

public class EventManager : MonoBehaviourSingleton <EventManager>
{
    public Action onGoToSequel;
    public Action<int> onGoToAnswer;

    public void GoToSequel()
    {
        onGoToSequel?.Invoke();
    }
    public void GoToAnswer(int index)
    {
        onGoToAnswer?.Invoke(index);
    }
}
