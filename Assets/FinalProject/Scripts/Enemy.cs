using UnityEngine;
using DG.Tweening;

namespace FinalProject
{
    public class Enemy : InteractableObject, IPushable
    {
        [SerializeField]
        private int hp = 1;

        public float moveDuration = 0.2f;

        public override bool Interact(Player player, Vector2Int dir)
        {
            if (!Push(dir))
            {
                int dmg = player != null ? player.AttackPoint : 1;
                TakeDamage(dmg);
            }

            return false;
        }

        public bool CanPush(Vector2Int dir)
        {
            if (mapGenerator == null) return false;

            Vector2Int nextPos = GridPos + dir;

            if (!mapGenerator.IsWalkableFloor(nextPos.x, nextPos.y))
            {
                return false;
            }

            if (mapGenerator.GetMapData(nextPos.x, nextPos.y) != null)
            {
                return false;
            }

            return true;
        }

        public bool Push(Vector2Int dir)
        {
            if (!CanPush(dir)) return false;

            Vector2Int nextPos = GridPos + dir;

            mapGenerator.mapdata[GridPos.x, GridPos.y] = null;
            GridPos = nextPos;
            mapGenerator.mapdata[GridPos.x, GridPos.y] = this;

            transform.DOKill();
            transform.DOMove(new Vector3(GridPos.x, GridPos.y, 0), moveDuration);

            return true;
        }

        private void TakeDamage(int dmg)
        {
            hp -= dmg;
            if (hp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            transform.DOKill();
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
