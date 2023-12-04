using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image qSkillImg;
    [SerializeField] private Image wSkillImg;
    [SerializeField] private Image eSkillImg;
    [SerializeField] private Image rSkillImg;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI cSkillText;
    [SerializeField] private TextMeshProUGUI qSkillText;
    [SerializeField] private TextMeshProUGUI wSkillText;
    [SerializeField] private TextMeshProUGUI eSkillText;
    [SerializeField] private TextMeshProUGUI rSkillText;
    public CharacterInfo Info { 
        set {
            image.sprite = value.Image;
            qSkillImg.sprite = value.QSkillImage;
            wSkillImg.sprite = value.WSkillImage;
            eSkillImg.sprite = value.ESkillImage;
            rSkillImg.sprite = value.RSkillImage;
            nameText.text = value.Name;
            cSkillText.text = value.CharacteristicText;
            qSkillText.text = value.QSkillText;
            wSkillText.text = value.WSkillText;
            eSkillText.text = value.ESkillText;
            rSkillText.text = value.RSkillText;
        }
    }

}
