using UnityEngine;

namespace FinalProject
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private DialogueNode currentNode;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void StartDialogue(string npcName, DialogueNode root)
        {
        }

        public void SelectChoice(int index)
        {
        }

        private void CheckOutcome(DialogueNode node)
        {
        }

        private void GameOver()
        {
        }

        private void NextStage()
        {
        }
    }
}
