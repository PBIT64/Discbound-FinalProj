using UnityEngine;
using Solution;

namespace FinalProject
{
    public abstract class InteractableObject : Identity
    {
        [Header("Diagram Data")]
        [SerializeField]
        protected Vector2Int gridPos;

        public Vector2Int GridPos
        {
            get => gridPos;
            set
            {
                gridPos = value;
                positionX = value.x;
                positionY = value.y;
            }
        }

        public override void SetUP()
        {
            base.SetUP();
            gridPos = new Vector2Int(positionX, positionY);
        }

        public abstract bool Interact(Player player, Vector2Int dir);

        public override bool Hit()
        {
            if (mapGenerator != null && mapGenerator.player != null)
            {
                Vector2Int dir = new Vector2Int(positionX - mapGenerator.player.positionX, positionY - mapGenerator.player.positionY);
                Player fpPlayer = mapGenerator.player as Player;
                if (fpPlayer != null)
                {
                    return Interact(fpPlayer, dir);
                }
            }
            return false;
        }
    }
}

