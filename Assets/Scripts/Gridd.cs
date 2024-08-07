using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//responsible for the layout of heads and types of head lists
public class Gridd : MonoBehaviour
{
	//public read-only constants
	public Vector2 gridScale {get{return transform.localScale;}}
	public Vector2 gridPosition {get{return transform.localPosition;}}
	public float offset {get{return 10.5f;}}
	public float headCount {get{return 9;}}
	public int defectorCount {get{return 2;}}
	//public but only changeable within class
	public static GameObject selectedBorderSprite {get; private set;}
	public static GameObject deadSprite {get; private set;}
	public static List<GameObject> heads {get; private set;}
	public static List<GameObject> selectedHeads {get; private set;}
	public static List<GameObject> defectorHeads {get; private set;}
	//private read-only constants
	private float defaultGridZ {get{return 100f;}}
	private int gridDimensions {get{return 3;}}

	void Awake()
	{
		selectedBorderSprite = GameObject.Find("SelectedBorderSprite");
		deadSprite = GameObject.Find("DeadSprite");
		heads = new List<GameObject>();
		selectedHeads = new List<GameObject>();
		defectorHeads = new List<GameObject>();
	}

	// Use this for initialization
	void Start()
	{
		//match grid scale
		selectedBorderSprite.transform.localScale = new Vector2(gridScale.x, gridScale.y);
		deadSprite.transform.localScale = new Vector2(gridScale.x, gridScale.y);
	}

	// Update is called once per frame
	void Update()
	{
		if(selectedHeads == null)
			Debug.Log("selectedHeads List is null for some reason!");
	}

	//method for gamestate to call
	public void resetGridd()
	{
		// GameState gameState = GameObject.Find("Player").GetComponent<GameState>();
		// gameState.resetRoundText();
		randomizeHeads();
		positionHeads();
		chooseDefectors();
	}

	//gamestate calls this to reset selectedheads between rounds
	public void resetSelectedHeads()
	{
		while(selectedHeads.Count > 0)
		{
			GameObject currentHead = selectedHeads[0];
			currentHead.GetComponent<Head>().toggleSelected();
		}
	}

	//selects defectors, use indexList because can't remove objects from heads
	private void chooseDefectors()
	{
		defectorHeads = new List<GameObject>();
		List<int> indexList = new List<int>();
		for(int i = 0; i < headCount; i++)
		{
			heads[i].GetComponent<Head>().isDefector = false;
			indexList.Add(i);
		}
		for(int i = 0; i < defectorCount; i++)
		{
			int index = indexList[Random.Range(0, indexList.Count)];
			GameObject currentHead = heads[index];
			currentHead.GetComponent<Head>().isDefector = true;
			defectorHeads.Add(currentHead);
			indexList.Remove(index);
		}
	}

	//randomizes the order of heads
	private void randomizeHeads()
	{
		selectedHeads = new List<GameObject>();
		heads = new List<GameObject>();
		List<GameObject> randomizedHeads = new List<GameObject>();
		foreach(Transform child in transform)
		{
			heads.Add(child.gameObject);
		}
		for(int i = 0; i < headCount; i++)
		{
			int index = Random.Range(0, heads.Count);
			GameObject currentHead = heads[index];
			randomizedHeads.Add(currentHead);
			heads.Remove(currentHead);
		}
		heads = randomizedHeads;
	}

	//positions heads in 3x3 grid layout
	private void positionHeads()
	{
		float y = 0f;
		for(int i = 0; i < gridDimensions; i++)
		{
			float x = -offset*gridScale.x + gridPosition.x;
			for(int j = 0; j < gridDimensions; j++)
			{
				int index = i*gridDimensions + j;
				heads[index].transform.position = new Vector3(x, y, defaultGridZ);
				x += offset*gridScale.x + gridPosition.x;
			}
			y += -offset*gridScale.y + gridPosition.y;
		}
	}
}
