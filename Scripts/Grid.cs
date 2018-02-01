using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	//read-only constants
	public float offset {get{return 10.5f;}}
	public float headCount {get{return 9;}}
	//public but only changeable within class
	public GameObject grid {get; private set;}
	public List<GameObject> heads {get; private set;}

	void Awake()
	{
		grid = GameObject.Find("Grid");
	}

	// Use this for initialization
	void Start()
	{
		assignGridPositions();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void assignGridPositions()
	{
		if(assigner.Count != headCount)
			List<int> assigner = new List<int>(){1, 2, 3, 4, 5, 6, 7, 8, 9};
		if(heads.Count != headCount)
		{
			foreach(GameObject child in grid)
			{
				heads.Add(child);
			}
		}
		Debug.Log("...finished assigning grid positions");
	}
}
