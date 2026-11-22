using UnityEngine;

namespace FinalProject
{
    public class NPC : InteractableObject
    {
        [SerializeField]
        private string npcName;

        [SerializeField]
        private DialogueNode rootNode;

        public override bool Interact(Player player, Vector2Int dir)
        {
            return false;
        }
    }
}
