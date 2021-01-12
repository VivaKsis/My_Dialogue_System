using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrameCollocutorName : Frame
{
    [SerializeField] private TextMeshProUGUI _nameText;
    public TextMeshProUGUI NameText => _nameText;

    [SerializeField] private Image _background;
    public Image Background => _background;
}
