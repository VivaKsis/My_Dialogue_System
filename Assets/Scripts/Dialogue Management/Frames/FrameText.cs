using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameText : Frame
{
    [SerializeField] private TextMeshProUGUI _text;
    public TextMeshProUGUI Text => _text;

    public bool _sentencesEnd { get; private set; }

    private Queue _sentences = new Queue();

    public void InsertInQueue(List<string> sentencesToEnqueue)
    {
        _sentences.Clear();
        for (int a = 0; a < sentencesToEnqueue.Count; a++)
        {
            _sentences.Enqueue(InsertPlayerName.InsertPlayerNameIntoCharacterSentence(sentencesToEnqueue[a]));
        }
        _sentencesEnd = false;
        SetSentence();
    }

    public void SetSentence()
    {
        if (_sentences.Count == 0)
        {
            EventManager.Instance.GoToSequel();
        }
        else
        {
            _text.text = _sentences.Dequeue() as string;
            Show();
            if (_sentences.Count == 0)
            {
                _sentencesEnd = true;
            }
        }
    }

    private void OnEnable()
    {
        try
        {
            PlayerClicksEvent.Instance.onClick += SetSentence;
        }
        catch
        {
            Debug.LogWarning("PlayerClicksEvent singleton is disabled. Must be on for xNode and off for FR");
        }
    }

    private void OnDisable()
    {
        try
        {
            PlayerClicksEvent.Instance.onClick -= SetSentence;
        }
        catch
        {
            Debug.LogWarning("PlayerClicksEvent singleton is disabled. Must be on for xNode and off for FR");
        }
    }
}
