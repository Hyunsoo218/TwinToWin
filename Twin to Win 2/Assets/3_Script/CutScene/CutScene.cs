using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutScene : MonoBehaviour
{
	protected bool playing = true;
	public abstract IEnumerator Play();
}
