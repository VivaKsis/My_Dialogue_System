using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour, IPoolable
{
    [SerializeField] private Button _button;
    public Button Button => _button;

    [SerializeField] private TextMeshProUGUI _buttonText;
    public TextMeshProUGUI ButtonText => _buttonText;

    #region Pool
    public GameObject GameObject => this.gameObject;
    public ObjectPool.Pool Pool { get; set; }

    public void OnRelease()
    {
    }

    public void OnAquire()
    {
    }
    #endregion
}
