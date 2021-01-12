using UnityEngine;
using XNode;

public abstract class BaseDialogueNode : Node
{
    [Input(backingValue = ShowBackingValue.Never)] public BaseDialogueNode input;
    public const string _INPUT_TAG = "input";
    [Output(backingValue = ShowBackingValue.Never)] public BaseDialogueNode sequel;
    public const string _SEQUEL_TAG = "sequel";

    abstract public void Trigger();

    public void GoToSequel()
    {
        NodePort port = GetOutputPort(_SEQUEL_TAG);

        if (port.IsConnected)
        {
            for (int b = 0; b < port.ConnectionCount; b++)
            {
                NodePort connection = port.GetConnection(b);
                BaseDialogueNode node = connection.node as BaseDialogueNode;
                node.OnEnter();
            }
        }

        (graph as DialogueGraph).currentNodes.Remove(this);

        EventManager.Instance.onGoToSequel -= GoToSequel;

        (graph as DialogueGraph).TriggerAllAvailableNodes();
    }

    public void OnEnter() 
    {
        if (!(graph as DialogueGraph).currentNodes.Contains(this))
        {
            (graph as DialogueGraph).currentNodes.Add(this);
        }
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}