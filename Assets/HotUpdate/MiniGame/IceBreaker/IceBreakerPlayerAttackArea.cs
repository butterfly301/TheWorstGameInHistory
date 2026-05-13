using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    /// <summary>
    ///     This component should be attached to the player's attack effect prefab (e.g., the shockwave).
    ///     It detects when the attack hits an enemy and calls the enemy's Die() method.
    ///     NOTE: The prefab must have a Collider2D (set as a trigger) and a Rigidbody2D (set to kinematic) for this to work.
    /// </summary>
    public class IceBreakerPlayerAttackArea : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Check if the object we collided with is on the "Enemy" layer
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Try to get the enemy's controller component and call Die()
                var enemy = other.gameObject.GetComponent<IceBreakerEnemyController>();
                if (enemy != null) enemy.TakeDamage();
            }
        }
    }
}