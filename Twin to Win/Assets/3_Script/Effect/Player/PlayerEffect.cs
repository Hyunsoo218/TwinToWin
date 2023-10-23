using System.Collections;
using UnityEngine;
public class PlayerEffect : EffectOverlap
{
    [Header("Sphere Area")]
    [SerializeField] bool isSphere = false;
    [SerializeField] float sphereAttackAreaRange = 0f;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (isSphere == true)
        {
            Gizmos.DrawWireSphere(transform.position, sphereAttackAreaRange);
        }
        else
        {
            Gizmos.DrawWireCube(Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position, vAttackAreaSize);
        }

    }
#endif

    public void OnSkillDamage(Transform tUser, float fDamage, int nTargetLayer)
    {
        transform.SetParent(tUser);
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.SetParent(null);

        Vector3 vOverlapPos = Quaternion.LookRotation(transform.forward, Vector3.up) * vAttackAreaCenter + transform.position;
        Collider[] arrOverlapObj;

        if (isSphere == true)
        {
            arrOverlapObj = Physics.OverlapSphere(transform.position, sphereAttackAreaRange, nTargetLayer);
        }
        else
        {
            arrOverlapObj = Physics.OverlapBox(vOverlapPos, vAttackAreaSize, transform.rotation, nTargetLayer);
        }

        foreach (Collider cItem in arrOverlapObj)
        {
            Character cTarget;
            if (cItem.TryGetComponent<Character>(out cTarget))
            {
                DamageCalculator.OnDamage(cTarget, fDamage, criticalHit);
            }
        }
    }

    public void OnSkillEffect(Transform tUser)
    {
        if (Player.instance.cCurrentCharacter.isSkillEffectFollowingPlayer == false)
        {
            transform.SetParent(tUser);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.SetParent(null);
        }
        else
        {
            StartCoroutine(StartSkillEffectFollowingPlayer(tUser));
        }
    }

    private IEnumerator StartSkillEffectFollowingPlayer(Transform tUser)
    {
        while (Player.instance.cCurrentCharacter.isSkillEffectFollowingPlayer == true)
        {
            transform.SetParent(tUser);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            yield return null;
        }
        transform.SetParent(null);
    }

    public void OnSkillContinueDamage(Transform tUser, float fDamage, int nTargetLayer, float totalSkillTime, int totalAttackCountInSkill)
    {
        StartCoroutine(StartContinueDamage(tUser, fDamage, nTargetLayer, totalSkillTime, totalAttackCountInSkill));
    }

    private IEnumerator StartContinueDamage(Transform tUser, float fDamage, int nTargetLayer, float totalSkillTime, int totalAttackCountInSkill)
    {
        for (int i = 0; i < totalAttackCountInSkill; i++)
        {
            OnSkillDamage(tUser, fDamage, nTargetLayer);
            yield return new WaitForSeconds(Mathf.Round((totalSkillTime / totalAttackCountInSkill) * 10) * 0.1f);
        }
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
