using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "[Dialogue]", menuName = "[Dialogue] System/[Dialogue]", order = 0)]
public class Dialogue : ScriptableObject
{
	[SerializeField] private DialogueGraph _dialogueGraph;
	public DialogueGraph _DialogueGraph => this._dialogueGraph;

	public BaseDialogueNode _ActiveChatDialogueNode { get; private set; }

    public class Controller
    {
        public static Controller Instance = new Controller();

        public Dialogue _ActiveDialogue { get; private set; }

        public void ActivateDialogue(Dialogue dialogue)
        {
            this._ActiveDialogue = dialogue;

            this._ActiveDialogue._ActiveChatDialogueNode = this._ActiveDialogue._dialogueGraph.GetRootNode();
        }

        private Controller()
        {
        }
    }

    public void Activate() => Controller.Instance.ActivateDialogue(this);

    public void GoToRootNode()
    {
        _ActiveChatDialogueNode = _dialogueGraph.GetRootNode();
    }

    private void Awake()
    {
        GoToRootNode();
    }

    /* private void RenewCurrentSpeaker()
     {
         //EventManager.Instance.BoxShow(_ActiveChatDialogueNode._CollocutorOrientation);

        // EventManager.Instance.SpriteChange(_ActiveChatDialogueNode._CollocutorInfo._CollocutorSprites[0], _ActiveChatDialogueNode._CollocutorOrientation);

        // EventManager.Instance.NameChange(_ActiveChatDialogueNode._CollocutorInfo._CollocutorName, _ActiveChatDialogueNode._CollocutorOrientation);

         //EventManager.Instance.EnqueueSentences(_ActiveChatDialogueNode._Sentences, _ActiveChatDialogueNode._CollocutorOrientation);
     }

     private void EndSentencesManagement()
     {
         if (_ActiveChatDialogueNode.HasAnswers)
         {
             EventManager.Instance.StartGeneratingAnswers(_ActiveChatDialogueNode._Answers.ToArray());
         }
         else if (_ActiveChatDialogueNode.HasSequel)
         {
             _ActiveChatDialogueNode = _ActiveChatDialogueNode.SetSequelDialogueAsCurrent();
             RenewCurrentSpeaker();
         }
         else
         {
             Debug.Log("In development. For now: the end");
         }
     }

     private void TriggerAnswersNode(int index)
     {
         _ActiveChatDialogueNode = _ActiveChatDialogueNode.SetAnswerDialogueAsCurrent(index);

         EventManager.Instance.ContinueButtonCreateGap();
     }*/

    /*public void AddDelegates()
    {
        EventManager.Instance.onEndSentences += EndSentencesManagement;
        EventManager.Instance.onAnswerChosen += TriggerAnswersNode;
        EventManager.Instance.onRenewSpeaker += RenewCurrentSpeaker;
    }

    public void RemoveDelegates()
    {
        EventManager.Instance.onEndSentences -= EndSentencesManagement;
        EventManager.Instance.onAnswerChosen -= TriggerAnswersNode;
        EventManager.Instance.onRenewSpeaker -= RenewCurrentSpeaker;
    }*/


}
