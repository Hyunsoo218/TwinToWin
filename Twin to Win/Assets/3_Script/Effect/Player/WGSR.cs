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

    private void Awake()
    {
        skinWGSRSkill = GetComponent<SkinnedMeshRenderer>();
        matWGSRSkill = skinWGSRSkill.material;
    }

    public void StartWGSREffect()
    {
        ResetTransparency();
        StartCoroutine(StartIncreaseTransparency());
        StartCoroutine(StartDecreaseShapesKey());
    }

    private void ResetTransparency()
    {
        matWGSRSkill.SetFloat("_Transparency", 0.5f);
    }

    private void ResetShapeKeys()
    {
        for (int i = 0; i < iTotalKey; i++)
        {
            skinWGSRSkill.SetBlendShapeWeight(i, 100f);
        }
    }

    private IEnumerator StartIncreaseTransparency()
    {
        float minTransparency = -0.5f;
        float transparencyDecreasingSpeed = 0.5f;

        for (float i = matWGSRSkill.GetFloat("_Transparency"); i > minTransparency; i -= Time.deltaTime * transparencyDecreasingSpeed)
        {
            matWGSRSkill.SetFloat("_Transparency", i);
            yield return null;
        }
        ResetTransparency();
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
        float decreasingSpeed = 100f;

        for (float i = skinWGSRSkill.GetBlendShapeWeight(iCurrentKey); i > 0f; i -= Time.deltaTime * decreasingSpeed)
        {
            skinWGSRSkill.SetBlendShapeWeight(iCurrentKey, i);
            yield return null;
        }

        if (iCurrentKey == iTotalKey - 1)
        {
            ResetShapeKeys();
        }

        qShapeKeys.Dequeue();
    }


}
