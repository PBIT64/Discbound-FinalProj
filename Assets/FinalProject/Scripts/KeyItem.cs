using UnityEngine;

namespace FinalProject
{
    public class KeyItem : InteractableObject
    {
        [SerializeField]
        private int amount = 1;

        public override bool Interact(Player player, Vector2Int dir)
        {
            Inventory inv = player != null ? player.GetInventory() : null;
            if (inv != null)
            {
                inv.AddKey(amount);
            }

            if (mapGenerator != null && mapGenerator.mapdata != null)
            {
                if (GridPos.x >= 0 && GridPos.x < mapGenerator.X && GridPos.y >= 0 && GridPos.y < mapGenerator.Y)
                {
                    if (mapGenerator.mapdata[GridPos.x, GridPos.y] == this)
                    {
                        mapGenerator.mapdata[GridPos.x, GridPos.y] = null;
                    }
                }
            }

            Destroy(gameObject);
            return true;
        }
    }
}
