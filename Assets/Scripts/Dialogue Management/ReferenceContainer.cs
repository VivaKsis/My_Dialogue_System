using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceContainer : MonoBehaviourSingleton<ReferenceContainer>
{
    [SerializeField]  private RectTransform _mainCanvasRectTransform;
    public RectTransform MainCanvasRectTransform => _mainCanvasRectTransform;

    [SerializeField] private PlayerInfo _playerInfo;
    public PlayerInfo _PlayerInfo => _playerInfo;

    private void Start()
    {
        InsertPlayerName.playerName = _playerInfo.playerName;
    }
}
