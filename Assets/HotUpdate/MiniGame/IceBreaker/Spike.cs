using UnityEngine;

namespace HotUpdate.MiniGame.IceBreaker
{
    public class Spike : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 检查与我们碰撞的物体上是否直接挂载了 PlayerBody 脚本
            if (other.TryGetComponent<PlayerBody>(out _))
            {
                // 如果是身体，那么就从这个物体或其父物体上找到控制器并调用 Die
                var player = other.GetComponentInParent<IceBreakerPlayerController>();
                if (player.GetCurrentState() == IceBreakerPlayerController.PlayerState.Playing) player.TakeDamage();
            }
        }
    }
}