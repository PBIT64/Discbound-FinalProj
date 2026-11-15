using UnityEngine;

namespace FinalProject
{

    public class Door : InteractableObject
    {
        public override bool Interact(Player player, Vector2Int dir)
        {
            return true;
        }
    }
}