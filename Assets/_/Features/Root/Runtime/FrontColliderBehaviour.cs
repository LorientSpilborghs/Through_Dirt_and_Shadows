using UnityEngine;

public class FrontColliderBehaviour : MonoBehaviour
{
    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private Collider _collider;
}
