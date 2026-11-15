using UnityEngine;

namespace FinalProject
{

    public class Enemy : InteractableObject
    {
        public override bool Interact(Player player, Vector2Int dir)
        {
            return true;
        }
    }
}