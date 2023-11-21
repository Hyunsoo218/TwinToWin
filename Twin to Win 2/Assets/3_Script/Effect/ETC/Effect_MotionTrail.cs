using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public class Solution
    {
        public class Data
        {
            public Data(string name, int num)
            {
                this.name = name;
                this.num = num;
            }
            public Data next;
            public Data prev;
            public string name;
            public int num;
        }
        public class DataList
        {
            public void Add(Data add)
            {
                if (seed == null) seed = add;
                else
                {
                    Data temp = seed;
                    while (temp.prev != null)
                        temp = temp.prev;
                    temp.prev = add;
                    add.next = temp;
                }
            }
            public Data Get(string name)
            {
                Data temp = seed;
                while (temp != null)
                {
                    if (temp.name == name)
                        break;
                    temp = temp.prev;
                }
                return temp;
            }
            public string[] GetAll()
            {
                List<string> names = new List<string>();
                Data temp = seed;
                while (temp != null)
                {
                    names.Add(temp.name);
                    temp = temp.prev;
                }
                return names.ToArray();
            }
            private Data seed;
        }
        public string[] solution(string[] players, string[] callings)
        {
            DataList dataList = new DataList();
            Data currentData;
            Data nextData;
            Data prevData;
            for (int i = 0; i < players.Length; i++)
                dataList.Add(new Data(players[i], i));
            foreach (var name in callings)
            {
                currentData = dataList.Get(name);
                nextData = currentData.next;
                prevData = currentData.prev;
                currentData.num--;
                nextData.num++;
                currentData.next = nextData.next;
                currentData.prev = nextData;
                nextData.next = currentData;
                nextData.prev = prevData;
                if(prevData != null)
                    prevData.next = nextData;
            }
            return dataList.GetAll();
        }
    }
}
