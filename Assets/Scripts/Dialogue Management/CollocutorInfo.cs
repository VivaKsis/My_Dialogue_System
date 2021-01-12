using UnityEngine;

[CreateAssetMenu(menuName = "xNode/Collocutor Info")]
public class CollocutorInfo : ScriptableObject
{
    [SerializeField] private string _speakerName;
    public string _CollocutorName => _speakerName;

    [SerializeField] private Sprite [] _speakerSprites;
    public Sprite [] _CollocutorSprites => _speakerSprites;

    [SerializeField] private Color _nodeColorInGraph;
    public Color _NodeColorInGraph => _nodeColorInGraph;
}
