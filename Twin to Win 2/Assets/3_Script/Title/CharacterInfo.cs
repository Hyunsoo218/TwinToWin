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
                    "��Į��",
                    "Q �Ǵ� W ��ų�� ���� óġ��, <color=red>��� ��ų�� ���� ���ð��� �ʱ�ȭ</color> �ȴ�",
                    "������ <color=red>���� ��ǥ</color>���� <color=red>���� ����</color>�� �Ѵ�",
                    "�������� ������ <color=red>�̵�</color>�ϸ� �����Ѵ�",
                    "�������� �����ϸ� <color=red>��� ������ ȸ��</color>�Ѵ�",
                    "�ֺ��� ������ �̵��ϸ� 10ȸ �����ϰ�, �������� �ֺ��� ���� ������ �Ѵ�"); break;
            case CharacterType.wgs:
                SetText(
                    "�ٸ���",
                    "��ų ��� ��, <color=blue>�޴� ���ذ� 60% ����</color>�Ѵ�",
                    "������ �� ������ ���� ������ �Ѵ�",
                    "�������� ������ <color=blue>�̵�</color>�ϸ� ���� �ִ� 8ȸ �����Ѵ�",
                    "ȸ���ϸ� �ֺ��� ��ǥ���� �ִ� 25ȸ �����Ѵ�",
                    "�ֺ��� ��ǥ���� ���� ������ �ϰ�, �������� �˱⸦ ���� �����Ѵ�"); break;
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
