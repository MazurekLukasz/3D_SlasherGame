using UnityEngine;

public class WeaponColliderController : MonoBehaviour
{
    [SerializeField] private CapsuleCollider WeaponCollider;

    public void WeaponColliderOn()
    {
        WeaponCollider.enabled = true;
    }
    public void WeaponColliderOff()
    {
        WeaponCollider.enabled = false;
    }
}
