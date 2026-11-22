using UnityEngine;

namespace FinalProject
{
    public interface IPushable
    {
        bool CanPush(Vector2Int dir);
        bool Push(Vector2Int dir);
    }
}
