using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class Grab : MonoBehaviour
    {
        [SerializeField] public LayerMask layer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Compute.ContainsLayer(layer, other.gameObject.layer))
            {
                var grabItem = other.gameObject.GetComponent<GrabItem>();
                if (grabItem != null) grabItem.OnGrab();
            }
        }
    }
}