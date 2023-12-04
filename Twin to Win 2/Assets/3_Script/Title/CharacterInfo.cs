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
    public CharacterType Type { get { return type; } }
    public Sprite Image { get { return image; } }
    public Sprite ImagePlay { get { return image_play; } }
    public Sprite QSkillImage { get { return qSkillImage; } }
    public Sprite WSkillImage { get { return wSkillImage; } }
    public Sprite ESkillImage { get { return eSkillImage; } }
    public Sprite RSkillImage { get { return rSkillImage; } }
    public Sprite ROffSkillImage { get { return rOffSkillImage; } }
    public Sprite RChargeSkillImage { get { return rChargeSkillImage; } }
    public string Name { get { return _name; } }
    public string CharacteristicText { get { return characteristicText; } }
    public string QSkillText { get { return qSkillText; } }
    public string WSkillText { get { return wSkillText; } }
    public string ESkillText { get { return eSkillText; } }
    public string RSkillText { get { return rSkillText; } }

    public void Set() 
    {
        switch (type)
        {
            case CharacterType.wtd:
                SetText(
                    "스칼렛",
                    "Q 또는 W 스킬로 적을 처치지 모든 스킬의 재사용 대기시간이 초기화 된다",
                    "전방의 '좁은 범위'에 '강한 공격'을 한다",
                    "전방으로 빠르게 '이동'하며 적을 공격한다",
                    "전방으로 점프하며 모든 공격을 '회피'한다",
                    "주변을 빠르게 이동하며 10회 공격하고, 마무리로 주변의 적에게 '강한 공격'을 한다 "); break;
            case CharacterType.wgs:
                SetText(
                    "앨리스",
                    "스킬 사용중, 받는 피해가 60% 감소한다",
                    "전방의 긴 범위에 '강한 공격'을 한다",
                    "전방으로 빠르게 '이동'하며 적을 최대 8회 공격한다",
                    "주변의 적을 최대 25회 공격한다",
                    "주변의 적에게 '강한 공격'을 하고, 전방으로 검기를 날려 '강한 공격'을 한다 "); break;
            case CharacterType.temp:
                SetText(
                    "",
                    "",
                    "",
                    "",
                    "",
                    ""); break;
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
