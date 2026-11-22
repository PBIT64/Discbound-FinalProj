using UnityEngine;

namespace FinalProject
{
    public class NPC : InteractableObject
    {
        [SerializeField]
        private string npcName;

        private DialogueNode rootNode;

        public override bool Interact(Player player, Vector2Int dir)
        {
            return false;
        }
    }
}
