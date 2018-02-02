using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	//public read-only constants
	public int gridDimensions {get{return 3;}}
	public float offset {get{return 10.5f;}}
	public float headCount {get{return 9;}}
	//public but only changeable within class
	public GameObject grid {get; private set;}
	public static GameObject selectedBorder {get; private set;}
	public static List<GameObject> heads {get; private set;}
	public static List<GameObject> selectedHeads {get; private set;}
	//private read-only constants
	private float defaultGridZ {get{return 0f;}}

	void Awake()
	{
		grid = GameObject.Find("Grid");
		selectedBorder = GameObject.Find("SelectedBorder");
		heads = new List<GameObject>();
		selectedHeads = new List<GameObject>();
	}

	// Use this for initialization
	void Start()
	{
		randomizeHeads();
		positionHeads();
	}

	// Update is called once per frame
	void Update()
	{
	}

	//randomizes the order of heads
	private void randomizeHeads()
	{
		heads = new List<GameObject>();
		List<GameObject> randomizedHeads = new List<GameObject>();
		foreach(Transform child in grid.transform)
		{
			heads.Add(child.gameObject);
		}
		for(int i = 0; i < headCount; i++)
		{
			int index = Random.Range(0, heads.Count-1);
			GameObject selectedHead = heads[index];
			randomizedHeads.Add(selectedHead);
			heads.Remove(selectedHead);
		}
		heads = randomizedHeads;
	}

	//positions heads in 3x3 grid layout
	private void positionHeads()
	{
		float y = 0f;
		for(int i = 0; i < gridDimensions; i++)
		{
			float x = -offset;
			for(int j = 0; j < gridDimensions; j++)
			{
				int index = i*gridDimensions + j;
				heads[index].transform.position = new Vector3(x, y, defaultGridZ);
				x += offset;
			}
			y += -offset;
		}
	}
}
