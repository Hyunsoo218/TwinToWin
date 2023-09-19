using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackEffectBrakeTile : Effect
{
    [SerializeField] private GameObject objEffect;
    private Transform tUser;
    private float fDamage;
    private int nTargetLayer;
    private bool bRun = true;
    public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
        this.tUser = tUser;
        this.fDamage = fDamage;
        this.nTargetLayer = nTargetLayer;

        StartCoroutine(ChasePlayer());
        StartCoroutine(OnAction());
    }
    private IEnumerator OnAction() 
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2.5f);
            GameObject effect = EffectManager.instance.GetEffect(objEffect);
            effect.GetComponent<Effect>().OnAction(tUser, fDamage, nTargetLayer);
        }
        bRun = false;
    }
    private IEnumerator ChasePlayer() 
    {
        while (bRun) 
        {
            transform.position = Player.instance.cCurrentCharacter.transform.position;
            yield return null;
        }
    }
}
