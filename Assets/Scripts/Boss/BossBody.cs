using UnityEngine;

public class BossBody : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == BubbleCollisionLayer.BubbleNormal)
        {
            // todo: 无敌的玩家本身也不能造成伤害
            GetComponentInParent<BossController>().OnDamaged();
        }
    }
}