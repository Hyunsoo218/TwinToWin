using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HpbarControl : MonoBehaviour
{
    [SerializeField] private GameObject hpbar;
    private Dictionary<Character, CharacterWithHpbarInfo> hpbarInfos = new Dictionary<Character, CharacterWithHpbarInfo>();
    private List<Character> characters = new List<Character>();
    void Update()
    {
        foreach (var info in hpbarInfos)
        {
            if (info.Value.hpbar.gameObject.activeSelf && info.Value.target != null)
            {
                Vector3 hpbarPos = info.Value.target.transform.position + info.Value.hpbarPosOffset;
                info.Value.hpbar.transform.position = Camera.main.WorldToScreenPoint(hpbarPos);
            }
            else characters.Add(info.Key); 
        }
        if (characters.Count > 0)
        {
            foreach (var character in characters)
            {
                hpbarInfos.Remove(character);
            }
            characters.Clear();
        }
    }
    public void InsertHpbar(Character target, Vector3 hpbarPosOffset) 
    {
        if (!hpbarInfos.ContainsKey(target))
        {
            GameObject hpbarClone = Instantiate(hpbar, transform);
            CharacterWithHpbarInfo info = new CharacterWithHpbarInfo(
                target,
                hpbarClone.GetComponent<Hpbar>(),
                hpbarPosOffset
            );
            info.hpbar.Initialize(target.GetMaxHP(), 0, target.GetHP());

            hpbarInfos.Add(target, info);
        }
        else  print("이미 등록된 캐릭터입니다"); 
    }
    public void SetHp(Character target) 
    {
        if (hpbarInfos.ContainsKey(target))
        {
            hpbarInfos[target].hpbar.Set(target.GetHP());
        }
        else print("등록되지 않은 캐릭터입니다"); 
    }
    public void RemoveHpbar(Character target) 
    {
        if (hpbarInfos.ContainsKey(target))
        {
            hpbarInfos[target].hpbar.Disable();
        }
        else print("등록되지 않은 캐릭터입니다"); 
    }
    private struct CharacterWithHpbarInfo
    {
        public Character target;
        public Hpbar hpbar;
        public Vector3 hpbarPosOffset;
        public CharacterWithHpbarInfo(Character target, Hpbar hpbar, Vector3 hpbarPosOffset) 
        {
            this.target = target;
            this.hpbar = hpbar;
            this.hpbarPosOffset = hpbarPosOffset;
        }
    }
}