
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerEffect : EffectOverlap
{
    
    public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
        base.OnAction(tUser, fDamage, nTargetLayer);
    }

    public Collider GetMonsterInOverlap(Transform transform)
	{
        Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
        Collider[] arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, 1 << 7);

        foreach (Collider cItem in arrOverlapObj)
        {
            Character cTarget;
            if (cItem.TryGetComponent<Character>(out cTarget))
            {
                return cItem;
            }
        }

        return null;
    }
}
