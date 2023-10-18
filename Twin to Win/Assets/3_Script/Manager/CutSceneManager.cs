using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager instance;
    [SerializeField] private GameObject cutSceneStage1to2Prefab;
    [SerializeField] private GameObject cutSceneStage2to3Prefab;
    [SerializeField] private CutScene cutSceneStage1to2;
    [SerializeField] private CutScene cutSceneStage2to3;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(transform.parent.gameObject);
    }
    public void SetGame()
    {
        cutSceneStage1to2 = Instantiate(cutSceneStage1to2Prefab, transform).GetComponent<CutScene>();
        cutSceneStage2to3 = Instantiate(cutSceneStage2to3Prefab, transform).GetComponent<CutScene>();
    }
    public void SetTitle()
    {
        if (cutSceneStage1to2 != null) Destroy(cutSceneStage1to2.gameObject); 
        if (cutSceneStage2to3 != null) Destroy(cutSceneStage2to3.gameObject); 
    }
    public IEnumerator PlayCutScene(CutSceneType type) 
    {
        UIManager.instance.AcriveAllUI(false);
		switch (type)
		{
			case CutSceneType.Stage1to2: yield return StartCoroutine(cutSceneStage1to2.Play()); break;
			case CutSceneType.Stage2to3: yield return StartCoroutine(cutSceneStage2to3.Play()); break;
		}
        UIManager.instance.AcriveAllUI(true);
    }
}
public enum CutSceneType 
{
    Stage1to2, Stage2to3
}
