using UnityEngine;
using DG.Tweening;

namespace FinalProject
{
    public class PushableWall : InteractableObject, IPushable
    {
        public float moveDuration = 0.2f;

        public override bool Interact(Player player, Vector2Int dir)
        {
            return Push(dir);
        }

        public bool CanPush(Vector2Int dir)
        {
            if (mapGenerator == null) return false;

            Vector2Int nextPos = GridPos + dir;

            // ตรวจสอบว่าช่องถัดไปอยู่ในขอบเขตพื้น
            if (!mapGenerator.IsWalkableFloor(nextPos.x, nextPos.y))
            {
                return false;
            }

            // ตรวจสอบว่าช่องถัดไปไม่มีสิ่งกีดขวาง
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

            // ย้ายตำแหน่งใน mapdata
            mapGenerator.mapdata[GridPos.x, GridPos.y] = null;
            GridPos = nextPos;
            mapGenerator.mapdata[GridPos.x, GridPos.y] = this;

            // เลื่อน Visual ด้วย Tween
            transform.DOKill();
            transform.DOMove(new Vector3(GridPos.x, GridPos.y, 0), moveDuration);

            return true;
        }
    }
}