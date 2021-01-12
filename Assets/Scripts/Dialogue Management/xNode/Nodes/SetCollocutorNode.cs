using UnityEngine;

[NodeTint("#004f47")]
public class SetCollocutorNode : BaseDialogueNode
{
    [SerializeField, FrameImagePrefabAndSceneSelection] private GameObject _frame;
    public GameObject Frame => _frame;
    public const string _FRAME_TAG = "_frame";

    [SerializeField, CollocutorInfoSelection] private CollocutorInfo _collocutorInfo;
    public CollocutorInfo CollocutorInfo => _collocutorInfo;
    public const string _COLLOCUTOR_INFO_TAG = "_collocutorInfo";

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;
    public const string _SPRITE_TAG = "_sprite";

    private FrameImage _frameImage;

    public override void Trigger()
    {
        GameObject frameObject = ObjectPool.Instance.Aquire(_frame);
        frameObject.transform.SetParent(parent: ReferenceContainer.Instance.MainCanvasRectTransform, worldPositionStays: false);

        _frameImage = frameObject.GetComponent<FrameImage>();
        _frameImage.SetImageSprite(_sprite);
        _frameImage.ShowAnimation();
        frameObject.transform.localPosition = _frameImage.Position;
    }
}
