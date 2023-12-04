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
                    "��Į��",
                    "Q �Ǵ� W ��ų�� ���� óġ�� ��� ��ų�� ���� ���ð��� �ʱ�ȭ �ȴ�",
                    "������ '���� ����'�� '���� ����'�� �Ѵ�",
                    "�������� ������ '�̵�'�ϸ� ���� �����Ѵ�",
                    "�������� �����ϸ� ��� ������ 'ȸ��'�Ѵ�",
                    "�ֺ��� ������ �̵��ϸ� 10ȸ �����ϰ�, �������� �ֺ��� ������ '���� ����'�� �Ѵ� "); break;
            case CharacterType.wgs:
                SetText(
                    "�ٸ���",
                    "��ų �����, �޴� ���ذ� 60% �����Ѵ�",
                    "������ �� ������ '���� ����'�� �Ѵ�",
                    "�������� ������ '�̵�'�ϸ� ���� �ִ� 8ȸ �����Ѵ�",
                    "�ֺ��� ���� �ִ� 25ȸ �����Ѵ�",
                    "�ֺ��� ������ '���� ����'�� �ϰ�, �������� �˱⸦ ���� '���� ����'�� �Ѵ� "); break;
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
