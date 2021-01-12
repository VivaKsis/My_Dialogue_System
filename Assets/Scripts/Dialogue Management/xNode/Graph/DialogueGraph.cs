using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "xNode/Dialogue Graph")]
public class DialogueGraph : NodeGraph {

    [HideInInspector]
    public List<BaseDialogueNode> currentNodes;

    public void TriggerAllAvailableNodes()
    {
        int maxCount = 0;

        while (currentNodes.Count > 0)
        {
            for (int a = 0; a < currentNodes.Count; a++)
            {
                currentNodes[a].Trigger();

                if (currentNodes[a] is SetConversationTextNode)
                {
                    currentNodes.Remove(currentNodes[a]);
                    continue;
                }

                NodePort port = currentNodes[a].GetOutputPort(BaseDialogueNode._SEQUEL_TAG);
                if (port.IsConnected)
                {
                    for (int b = 0; b < port.ConnectionCount; b++)
                    {
                        NodePort connection = port.GetConnection(b);
                        BaseDialogueNode node = connection.node as BaseDialogueNode;
                        node.OnEnter();
                    }
                }
                currentNodes.Remove(currentNodes[a]);
            }
            maxCount++;
            if(maxCount > 100)
            {
                return;
            }
        }
    }

    public BaseDialogueNode GetRootNode()
    {
        currentNodes = new List<BaseDialogueNode>();
        for (int a = 0; a < nodes.Count; a++)
        {
            if (nodes[a] is BaseDialogueNode node)
            {
                node.OnEnter();
                return node;
            }
        }
        return null;
    }
}