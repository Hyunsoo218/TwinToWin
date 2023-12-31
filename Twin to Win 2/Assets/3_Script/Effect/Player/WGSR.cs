using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WGSR : MonoBehaviour
{
    private Material matWGSRSkill;
    private SkinnedMeshRenderer skinWGSRSkill;
    private Queue<Coroutine> qShapeKeys = new Queue<Coroutine>();

    private const int iTotalKey = 10;
    private float maxTransparency = 1f;
    private float minTransparency = 0f;

    private void Awake()
    {
        skinWGSRSkill = GetComponent<SkinnedMeshRenderer>();
        matWGSRSkill = skinWGSRSkill.material;
        ResetShapeKeys();
        ResetTransparency();
    }

    public void StartWGSREffect()
    {
        ResetShapeKeys();
        ResetTransparency();
        StartCoroutine(StartDecreaseTransparency());
        StartCoroutine(StartDecreaseShapesKey());
    }

    public void ResetTransparency()
    {
        matWGSRSkill.SetFloat("_Transparency", maxTransparency);
    }

    public void ResetShapeKeys()
    {
        for (int i = 0; i < iTotalKey; i++)
        {
            skinWGSRSkill.SetBlendShapeWeight(i, 100f);
        }
    }

    public IEnumerator StartIncreaseTransparency()
    {
        float transparencyDecreasingSpeed = 10f;

        for (float i = matWGSRSkill.GetFloat("_Transparency"); i < maxTransparency; i += Time.deltaTime * transparencyDecreasingSpeed)
        {
            print(i);
            matWGSRSkill.SetFloat("_Transparency", i);
            yield return null;
        }
    }

    private IEnumerator StartDecreaseTransparency()
    {

        float transparencyDecreasingSpeed = 0.8f;

        for (float i = matWGSRSkill.GetFloat("_Transparency"); i > minTransparency; i -= Time.deltaTime * transparencyDecreasingSpeed)
        {
            matWGSRSkill.SetFloat("_Transparency", i);
            yield return null;
        }
    }

    private IEnumerator StartDecreaseShapesKey()
    {
        float valueOfNextKey = 90f;

        for (int iCurrentKey = 0; iCurrentKey < iTotalKey; iCurrentKey++)
        {
            qShapeKeys.Enqueue(StartCoroutine(StartReduceIndividualShapesKey(iCurrentKey)));
            yield return new WaitUntil(() => skinWGSRSkill.GetBlendShapeWeight(iCurrentKey) <= valueOfNextKey);
        }
    }

    private IEnumerator StartReduceIndividualShapesKey(int iCurrentKey)
    {
        float decreasingSpeed = 120f;

        for (float i = skinWGSRSkill.GetBlendShapeWeight(iCurrentKey); i > 0f; i -= Time.deltaTime * decreasingSpeed)
        {
            skinWGSRSkill.SetBlendShapeWeight(iCurrentKey, i);
            yield return null;
        }

        qShapeKeys.Dequeue();
    }


}
