using UnityEngine;

public class WeaponColliderController : MonoBehaviour
{
    [SerializeField] private CapsuleCollider WeaponCollider;
    [SerializeField] private ParticleSystem VFX;

    private void Start()
    {
        if(VFX)
            VFX.enableEmission = false;
    }

    public void WeaponColliderOn()
    {
        WeaponCollider.enabled = true;

        if (VFX)
            VFX.enableEmission = true;
    }
    public void WeaponColliderOff()
    {
        WeaponCollider.enabled = false;

        if (VFX)
            VFX.enableEmission = false;
    }
}
