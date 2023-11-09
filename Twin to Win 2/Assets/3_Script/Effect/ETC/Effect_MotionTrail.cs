using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_MotionTrail : Effect {
    [SerializeField] private ParticleSystem particle;

    public override void OnAction(Transform tUser, float fDamage, int nTargetLayer)
    {
        transform.position = tUser.position;
        transform.rotation = tUser.rotation;
        GenerateTrail(tUser.transform.GetComponentsInChildren<SkinnedMeshRenderer>());
    }
    public void GenerateTrail(SkinnedMeshRenderer[] skinnedMeshes) {

        for (int i = 0; i < skinnedMeshes.Length; i++)
        {
            print(skinnedMeshes[i].gameObject.name);
        }

        for(int i=0; i<skinnedMeshes.Length; i++) {
            GameObject effect = EffectManager.instance.GetEffect(particle.gameObject);
            effect.transform.eulerAngles = new Vector3(-90f, 0, 0);
            effect.transform.SetParent(transform);
            effect.transform.localPosition = Vector3.zero;
            if (effect.TryGetComponent<ParticleSystem>(out var p)
            && effect.TryGetComponent<ParticleSystemRenderer>(out var pr)) {
                Mesh mesh = new Mesh();
                skinnedMeshes[i].BakeMesh(mesh, true); // 인자로 받은 메쉬를 베이크하여,
                List<Mesh> meshes = new List<Mesh>();
                meshes.Add(mesh);
                print("count" + i + ", smn" + mesh.subMeshCount);
                pr.SetMeshes(meshes.ToArray());     // 파티클 시스템이 생성하는 메쉬로 Set
                p.Play();
            }
        }
    }
}
