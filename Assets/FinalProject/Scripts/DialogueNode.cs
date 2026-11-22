using System.Collections.Generic;
using UnityEngine;

namespace FinalProject
{
    [CreateAssetMenu(fileName = "NewDialogueNode", menuName = "FinalProject/Dialogue Node")]
    public class DialogueNode : ScriptableObject
    {
        [Header("Choice & Dialogue Text")]
        public string choiceText;

        [TextArea(3, 6)]
        public string npcText;

        [Header("Outcome")]
        public bool isGameOver;
        public bool isStageClear;

        [Header("Next Branches (Tree)")]
        public List<DialogueNode> nextNodes = new List<DialogueNode>();
    }
}
