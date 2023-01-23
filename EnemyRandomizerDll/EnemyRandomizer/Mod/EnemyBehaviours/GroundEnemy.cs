using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyRandomizerMod.Behaviours
{
    public class GroundEnemy : EnemyBehaviour
    {
        bool needsMoving = true;
        private void Update()
        {
            //BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if(needsMoving)
                transform.Translate(new Vector3(Vector2.down.x, Vector2.down.y, 0f) * Time.deltaTime);
            //transform.position  = Vector3.MoveTowards(transform.position, (new Vector3(Vector2.down.x, Vector2.down.y, 0f) * Time.deltaTime), 10f);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            needsMoving = false;
        }

    }
}