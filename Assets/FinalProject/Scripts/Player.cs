using UnityEngine;
using UnityEngine.InputSystem;
using Solution;

namespace FinalProject
{
    public class Player : OOPPlayer
    {
        [Header("FinalProject Inventory (Diagram)")]
        [SerializeField]
        private Inventory playerInventory;

        private InputAction moveInputAction;
        private InputAction fireInputAction;

        public override void SetUP()
        {
            base.SetUP();
            if (playerInventory == null)
            {
                playerInventory = GetComponent<Inventory>();
            }
            moveInputAction = InputSystem.actions.FindAction("Move");
            fireInputAction = InputSystem.actions.FindAction("Attack");
        }

        public new void Update()
        {
            if (isAutoMoving || isMoving)
            {
                return;
            }

            if (moveInputAction != null && moveInputAction.triggered)
            {
                Vector2 rawDir = moveInputAction.ReadValue<Vector2>();
                Vector2Int dir = new Vector2Int(Mathf.RoundToInt(rawDir.x), Mathf.RoundToInt(rawDir.y));
                if (dir != Vector2Int.zero)
                {
                    TryMove(dir);
                }
            }

            Keyboard keyboard = Keyboard.current;
            bool firePressed = fireInputAction != null && fireInputAction.triggered;
            if (keyboard != null)
            {
                firePressed |= keyboard.eKey.wasPressedThisFrame;
            }
            if (firePressed)
            {
                UseFireStorm();
            }

            if (keyboard == null || actionHistoryManager == null)
            {
                return;
            }

            if (keyboard.zKey.wasPressedThisFrame)
            {
                actionHistoryManager.UndoLastMove(this);
            }
            if (keyboard.xKey.wasPressedThisFrame)
            {
                actionHistoryManager.RedoLastMove(this);
            }
            if (keyboard.qKey.wasPressedThisFrame)
            {
                actionHistoryManager.StartAutoMoveSequence(this);
            }
        }

        public Inventory GetInventory()
        {
            return playerInventory;
        }

        public bool TryMove(Vector2Int dir)
        {
            if (isMoving)
            {
                return false;
            }

            if (isFreeze)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                isFreeze = false;
                return false;
            }

            Vector2Int targetPos = GridPos + dir;
            InteractableObject targetObj = CheckCell(targetPos);

            if (targetObj != null)
            {
                bool canEnter = targetObj.Interact(this, dir);
                if (canEnter)
                {
                    UpdatePosition(targetPos.x, targetPos.y);
                    mapGenerator.MoveEnemies();
                    return true;
                }
                return false;
            }

            if (HasPlacement(targetPos.x, targetPos.y))
            {
                bool isCanWalkTo = mapGenerator.GetMapData(targetPos.x, targetPos.y).Hit();
                if (isCanWalkTo)
                {
                    UpdatePosition(targetPos.x, targetPos.y);
                    mapGenerator.MoveEnemies();
                    return true;
                }
                return false;
            }
            else
            {
                UpdatePosition(targetPos.x, targetPos.y);
                TakeDamage(1);
                mapGenerator.MoveEnemies();
                return true;
            }
        }

        private InteractableObject CheckCell(Vector2Int targetPos)
        {
            if (mapGenerator == null)
            {
                return null;
            }
            Identity cellData = mapGenerator.GetMapData(targetPos.x, targetPos.y);
            return cellData as InteractableObject;
        }
    }
}
