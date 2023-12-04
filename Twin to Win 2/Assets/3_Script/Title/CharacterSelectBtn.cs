using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CharacterSelectBtn : MonoBehaviour
{
    public Sprite Image { set { img.sprite = value; } }
    [SerializeField] private Image img;

    public string Name { set { text.text = value; } }
    [SerializeField] private TextMeshProUGUI text;

    public Action OnClick { set { onClick = value; } }
    private Action onClick;

    public void Click() => onClick.Invoke();
}
