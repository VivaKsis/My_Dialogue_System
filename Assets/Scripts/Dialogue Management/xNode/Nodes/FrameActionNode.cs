using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#005403")]
public class FrameActionNode : BaseDialogueNode
{
    [SerializeField, AllFramesSelection] private GameObject _frameObject;
    public GameObject FrameObject => _frameObject;
    public const string _FRAME_TAG = "_frameObject";

    [SerializeField] private FrameAction _doAction;
    public FrameAction DoAction => _doAction;
    public const string _DO_ACTION_TAG = "_doAction";

    private Frame _frame;

    public enum FrameAction
    {
        hide, 
        show,
        hideAndDestroy
    }

    public override void Trigger()
    {
        // find clone of the selected prefab in the main canvas
        _frame = ReferenceContainer.Instance.MainCanvasRectTransform.Find(_frameObject.name + "(Clone)").GetComponent<Frame>();
        if (_doAction == FrameAction.hide)
        {
            _frame.HideAnimation();
        }
        else if (_doAction == FrameAction.show)
        {
            _frame.ShowAnimation();
        }
        else if (_doAction == FrameAction.hideAndDestroy)
        {
            _frame.HideAndDestroy();
        }
    }
}
