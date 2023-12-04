using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    [SerializeField] private List<GameObject> character_play;
    [SerializeField] private List<GameObject> character_title;

    private Dictionary<CharacterType, CharacterInfo> typeToTitle =
        new Dictionary<CharacterType, CharacterInfo>();
    private Dictionary<CharacterType, GameObject> typeToObj =
       new Dictionary<CharacterType, GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        foreach (var prefab in character_title)
        {
            CharacterInfo ci = prefab.GetComponent<CharacterInfo>();
            typeToTitle.Add(ci.Type, ci);
            ci.Set();
        }
        foreach (var prefab in character_play)
        {
            PlayerbleCharacter pc = prefab.GetComponent<PlayerbleCharacter>();
            typeToObj.Add(pc.Type, prefab);
        }
    }
    public CharacterInfo GetTypeToInfo(CharacterType type) => typeToTitle[type];
    public bool ContainsKeyTypeToInfo(CharacterType type) => typeToTitle.ContainsKey(type);
    public GameObject GetTypeToObj(CharacterType type) => typeToObj[type];
}
