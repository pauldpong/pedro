using UnityEngine;

public class DestroyOnHit : MonoBehaviour, IBreakable
{
    public void OnHit()
    {
        Destroy(gameObject);
    }
}
