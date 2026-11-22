using UnityEngine;

namespace FinalProject
{
    public class Door : InteractableObject
    {
        [SerializeField]
        private bool isLocked = true;

        [SerializeField]
        private int requiredKeys = 1;

        public override bool Interact(Player player, Vector2Int dir)
        {
            if (!isLocked)
            {
                return true;
            }

            if (requiredKeys <= 0)
            {
                Unlock();
                return true;
            }

            Inventory inv = player != null ? player.GetInventory() : null;
            if (inv != null && inv.UseKey(requiredKeys))
            {
                Unlock();
                return true;
            }

            return false;
        }

        private void Unlock()
        {
            isLocked = false;
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
        }
    }
}
