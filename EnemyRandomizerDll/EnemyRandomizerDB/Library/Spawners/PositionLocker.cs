using UnityEngine;

namespace EnemyRandomizerMod
{
    public class PositionLocker : MonoBehaviour
    {
        public Vector2? positionLock;

        protected virtual void Update()
        {
            if(positionLock != null)
                transform.position = positionLock.Value;
        }
    }
}
