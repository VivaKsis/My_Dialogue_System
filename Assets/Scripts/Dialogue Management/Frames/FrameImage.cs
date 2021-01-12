using UnityEngine;
using UnityEngine.UI;

public class FrameImage : Frame
{
    [SerializeField] private Image _image;
    public Image Image => _image;

    public void SetImageSprite(Sprite sprite)
    {
        _image.color = new Color(1f, 1f, 1f, 1f);
        _image.sprite = sprite;
    }
}
