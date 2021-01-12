using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeTint("#540035")]
public class SetAnswersNode : BaseDialogueNode
{
    [SerializeField, FrameAnswersSelection] private GameObject _frame;
    public GameObject Frame => _frame;
    public const string _FRAME_TAG = "_frame";

    [Output(dynamicPortList = true), SerializeField] private List<string> _answers;
    public List<string> Answers => _answers;
    public const string _ANSWERS_TAG = "_answers";

    private FrameAnswers _frameAnswers;

    public override void Trigger()
    {
        GameObject frameObject = ObjectPool.Instance.Aquire(_frame);
        frameObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);

        _frameAnswers = frameObject.GetComponent<FrameAnswers>();
        _frameAnswers.Show();
        frameObject.transform.localPosition = _frameAnswers.Position;
        _frameAnswers.SetAnswerButtons(_answers);

        EventManager.Instance.onGoToAnswer += GoToAnswer;
    }

    public void GoToAnswer(int index)
    {
        NodePort port = GetOutputPort(_ANSWERS_TAG + " " + index);

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

        EventManager.Instance.onGoToAnswer -= GoToAnswer;

        (graph as DialogueGraph).TriggerAllAvailableNodes();
    }
}
