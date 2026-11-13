using UnityEngine;

namespace FinalProject
{

    public class KeyItem : InteractableObject
    {
        public override bool Interact(Player player, Vector2Int dir)
        {
            return true;
        }
    }
}