using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeTileBomb : Effect
{
    [SerializeField] private DamagableSpaceControl cDSC;
    private Animator cAnimator;
    public override void Initialize()
    {
        base.Initialize();
        cAnimator = GetComponent<Animator>();
    }
	public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
        cAnimator.SetTrigger("Action");
        Vector3 pos = Player.Instance.CurrentCharacter.transform.position;
        pos.y = 0;
        transform.position = pos;
        StartCoroutine(LateOverlap());
    }
    private IEnumerator LateOverlap() 
    {
        cDSC.OnAction(3f,FillType.X_Y);
        yield return new WaitForSeconds(3f);
        soundComponent.PlayOneShot(clip);
        Collider[] arrOverlapObj = Physics.OverlapSphere(transform.position, 3f, 1 << 6);
        foreach (Collider cItem in arrOverlapObj)
        {
            Collider cTile;
            if (cItem.TryGetComponent<Collider>(out cTile))
            {
                BoxCollider box = cTile as BoxCollider;
                if (box != null)
                {
                    box.size += Vector3.up * 5f;
                    box.GetComponent<MeshRenderer>().enabled = false;
                    box.gameObject.layer = 0;
                }
            }
        }
        arrOverlapObj = Physics.OverlapSphere(transform.position, 3f, 1 << 8);
        foreach (Collider cItem in arrOverlapObj) 
        { 
            PlayerbleCharacter cPlayerble;
            if (cItem.TryGetComponent<PlayerbleCharacter>(out cPlayerble))
            {
                cPlayerble.Damage(9999999f);
            }
        }
        StageManager.instance.UpdateNavMeshOne();
    }
}
