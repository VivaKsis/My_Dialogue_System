using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FlowReactor;
using FlowReactor.Nodes;
using FlowReactor.NodeUtilityModules;
using FlowReactor.BlackboardSystem;
using System;
using UnityEngine.SceneManagement;

public class DialogueStart : MonoBehaviour
{
    //For xNode only
    /* [SerializeField] private Dialogue _dialogue;
     public Dialogue Dialogue => _dialogue;

     private void Start()
     {
         _dialogue.GoToRootNode();
         _dialogue._DialogueGraph.TriggerAllAvailableNodes();
     }
     */

    [SerializeField] private FlowReactorComponent _fRComponent;
    public FlowReactorComponent _FRComponent => _fRComponent;
    // called zero
    void Awake()
    {
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    // called third
    void Start()
    {
        Debug.Log("Start");
    }

    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*private void OnEnable()
    {
        Debug.Log("OnEnable");
        List<Node> nodes = _fRComponent.graph.nodes;
        for(int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(nodes[i]);
        }
    }*/
}	
