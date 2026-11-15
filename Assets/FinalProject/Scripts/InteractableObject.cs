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


        public abstract bool Interact(Player player, Vector2Int dir);

        // Identity
        public override void SetUP()
        {
            base.SetUP();
            positionX = gridPos.x;
            positionY = gridPos.y;
            transform.position = new Vector3(gridPos.x, gridPos.y, 0);
            if (mapGenerator != null && mapGenerator.mapdata != null)
            {
                if (gridPos.x >= 0 && gridPos.x < mapGenerator.X && gridPos.y >= 0 && gridPos.y < mapGenerator.Y)
                {
                    mapGenerator.mapdata[gridPos.x, gridPos.y] = this;
                }
                else
                {
                    Debug.LogWarning($"{name} gridPos {gridPos} is out of map bounds ({mapGenerator.X}, {mapGenerator.Y}).");
                }
            }
        }
    }
}

