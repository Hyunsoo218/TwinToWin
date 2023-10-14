using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.AI.Navigation;

public class StageManager : MonoBehaviour
{
	public static StageManager instance;

	[Header("런타입 맵 만들기")]
	[SerializeField] private bool bMakeMapRuntime = false;
	[SerializeField] private GameObject objMapTile;
	[SerializeField] private float fTileStreets;
	[SerializeField] private Vector2 vTileCount;
	[SerializeField] private int nOneFrameCount;

	[Header("런타입 네브메쉬 업데이트")]
	[SerializeField] private bool bUpdateNavMesh = false;
	[SerializeField] private float fUpdateInterval = 0.5f;
	[SerializeField] private NavMeshSurface cNMS;

	private void Awake() 
	{
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
	private void Start()
	{
		if(bMakeMapRuntime) MakeMap();
		if(bUpdateNavMesh) StartCoroutine(UpdateNavMesh());
	}
	private void MakeMap()
	{
		for (float i = 0; i < vTileCount.x; i++)
		{
			Queue<Action> qActions = new Queue<Action>();
			for (float j = 0; j < vTileCount.y; j++) 
			{
				float nX = i;
				float nZ = j;
				Action cTemp = () => {
					float fX = nX * fTileStreets;
					float fZ = nZ * fTileStreets;
					Instantiate(objMapTile, new Vector3(fX, 0, fZ), Quaternion.identity);
				};
				qActions.Enqueue(cTemp);
			}
			GameManager.instance.AsynchronousExecution(qActions, nOneFrameCount);
		}
	}
	private IEnumerator UpdateNavMesh()
	{
		while (true)
		{
			cNMS.BuildNavMesh();
			yield return new WaitForSeconds(fUpdateInterval);
		}
	}
	public void UpdateNavMeshOne() 
	{
		cNMS.BuildNavMesh();
	}
}
