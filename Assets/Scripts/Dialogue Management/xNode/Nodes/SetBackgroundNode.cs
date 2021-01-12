using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[NodeTint("#230040")]
public class SetBackgroundNode : BaseDialogueNode
{
    [SerializeField] private Sprite _sprite;
    public Sprite Sprite => _sprite;

    public override void Trigger()
    {
        GameObject canvas = ReferenceContainer.Instance.MainCanvasRectTransform.gameObject;
        Image image = canvas.AddComponent<Image>();
        image.sprite = _sprite;
    }
}
