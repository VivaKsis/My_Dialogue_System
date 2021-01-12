using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeTint("#3d0009")]
public class SetConversationTextNode : BaseDialogueNode
{
    [SerializeField, FrameTextSelection] private GameObject _textFrame;
    public GameObject TextFrame => _textFrame;
    public const string _TEXT_FRAME_TAG = "_textFrame";

    [SerializeField, FrameCollocutorNameSelection] private GameObject _nameFrame;
    public GameObject NameFrame => _nameFrame;
    public const string _NAME_FRAME_TAG = "_nameFrame";

    [SerializeField, CollocutorInfoSelection] private CollocutorInfo _collocutorInfo;
    public CollocutorInfo CollocutorInfo => _collocutorInfo;
    public const string _COLLOCUTOR_INFO_TAG = "_collocutorInfo";

    [SerializeField, TextArea] private List<string> _sentences;
    public List<string> _Sentences => _sentences;
    public const string _SENTENCES_TAG = "_sentences";

    private FrameText _frameText;
    private FrameCollocutorName _collocutorNameInTextFrame, _collocutorNameSeparate;

    public override void Trigger()
    {
        GameObject frameTextObject = ObjectPool.Instance.Aquire(_textFrame);
        frameTextObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);

        _frameText = frameTextObject.GetComponent<FrameText>();
        _frameText.InsertInQueue(_sentences);
        _frameText.ShowAnimation();
        frameTextObject.transform.localPosition = _frameText.Position;

        _collocutorNameInTextFrame = frameTextObject.GetComponentInChildren<FrameCollocutorName>();

        if (_nameFrame != null) // collocutor name will be set in the separate frame
        {
            GameObject frameNameObject = ObjectPool.Instance.Aquire(_nameFrame);
            frameNameObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);

            _collocutorNameSeparate = frameNameObject.GetComponent<FrameCollocutorName>();
            _collocutorNameSeparate.NameText.text = _collocutorInfo.name;
            _collocutorNameSeparate.ShowAnimation();
            frameNameObject.transform.localPosition = _collocutorNameSeparate.Position;

            if (_collocutorNameInTextFrame != null) // must hide name component in the text frame
            {
                _collocutorNameInTextFrame.Background.color = new Color(0f, 0f, 0f, 0f);
                _collocutorNameInTextFrame.NameText.text = "";
            }
        }
        else
        {
             // collocutor name will be set in the text frame
            if (_collocutorNameInTextFrame != null)
            {
                _collocutorNameInTextFrame.NameText.text = _collocutorInfo.name;
            }
            else
            {
                // no collocutor name at all
            }
        }

        EventManager.Instance.onGoToSequel += GoToSequel;
    }
}
