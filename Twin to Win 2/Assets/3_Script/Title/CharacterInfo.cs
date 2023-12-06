using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private CharacterType type;

    [SerializeField] private Sprite image;    
    [SerializeField] private Sprite image_play;    
    [SerializeField] private Sprite qSkillImage;    
    [SerializeField] private Sprite wSkillImage;
    [SerializeField] private Sprite eSkillImage;
    [SerializeField] private Sprite rSkillImage;
    [SerializeField] private Sprite rOffSkillImage;
    [SerializeField] private Sprite rChargeSkillImage;

    private string _name;
    private string characteristicText;
    private string qSkillText;
    private string wSkillText;
    private string eSkillText;
    private string rSkillText;

    public CharacterType Type => type; 

    public Sprite Image => image;
    public Sprite ImagePlay => image_play; 
    public Sprite QSkillImage => qSkillImage; 
    public Sprite WSkillImage => wSkillImage; 
    public Sprite ESkillImage => eSkillImage; 
    public Sprite RSkillImage => rSkillImage; 
    public Sprite ROffSkillImage => rOffSkillImage; 
    public Sprite RChargeSkillImage => rChargeSkillImage; 

    public string Name => _name; 
    public string CharacteristicText => characteristicText; 
    public string QSkillText => qSkillText; 
    public string WSkillText => wSkillText; 
    public string ESkillText => eSkillText; 
    public string RSkillText => rSkillText; 

    public void Set() 
    {
        switch (type)
        {
            case CharacterType.wtd:
                SetText(
                    "스칼렛",
                    "Q 또는 W 스킬로 적을 처치지, <color=red>모든 스킬의 재사용 대기시간이 초기화</color> 된다",
                    "전방의 <color=red>단일 목표</color>에게 <color=red>강한 공격</color>을 한다",
                    "전방으로 빠르게 <color=red>이동</color>하며 공격한다",
                    "전방으로 점프하며 <color=red>모든 공격을 회피</color>한다",
                    "주변을 빠르게 이동하며 10회 공격하고, 마무리로 주변에 강한 공격을 한다"); break;
            case CharacterType.wgs:
                SetText(
                    "앨리스",
                    "스킬 사용 중, <color=blue>받는 피해가 60% 감소</color>한다",
                    "전방의 긴 범위에 강한 공격을 한다",
                    "전방으로 빠르게 <color=blue>이동</color>하며 적을 최대 8회 공격한다",
                    "회전하며 주변의 목표에게 최대 25회 공격한다",
                    "주변의 목표에게 강한 공격을 하고, 전방으로 검기를 날려 공격한다"); break;
            case CharacterType.temp: SetText("", "", "", "", "", ""); break;
        }
    }
    private void SetText(string name, string cText, string qText, string wText, string eText, string rText ) 
    {
        _name = name;
        characteristicText = cText;
        qSkillText = qText;
        wSkillText = wText;
        eSkillText = eText;
        rSkillText = rText;
    }
}
