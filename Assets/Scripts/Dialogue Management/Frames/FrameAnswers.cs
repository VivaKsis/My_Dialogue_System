using System.Collections.Generic;
using UnityEngine;

public class FrameAnswers : Frame
{
    [SerializeField] private GameObject _answerButtonPrefab;
    public GameObject AnswerButtonPrefab => _answerButtonPrefab;

    public void SetAnswerButtons(List<string> answers)
    {
        for (int a = 0; a < answers.Count; a++)
        {
            AnswerButton answerButton = ObjectPool.Instance.Aquire<AnswerButton>(_answerButtonPrefab);

            answerButton.transform.SetParent(
                parent: this.transform,
                worldPositionStays: false
            );

            answerButton.ButtonText.text = answers[a];

            answerButton.Button.onClick.RemoveAllListeners();

            int answerIndex = a;      // for passing proper index into TypeLongAnswer
            answerButton.Button.onClick.AddListener( () => 
            {
                  EventManager.Instance.GoToAnswer(answerIndex);
                  //ClearFrame();
            });
        }
    }
}
