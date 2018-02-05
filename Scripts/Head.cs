using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Grid
{
	//public attributes
	public bool isDefector;// {get; set;}
	public GameObject selectedBorderClone {get; set;}
	public GameObject deadClone {get; set;}
	//public attributes, but only changeable within class
	public bool isDead {get; private set;}
	public bool isSelected {get; private set;}
	public Collider2D coll {get; private set;}
	//private read-only constants
	private float defaultSelectedBorderZ {get{return -9.5f;}}

	void Awake()
	{
		coll = GetComponent<Collider2D>();
	}

	// Use this for initialization
	void Start()
	{
		isSelected = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	//Note: Can't have other colliders in the way (player object can't have collider)
	void OnMouseDown()
	{
		toggleSelected();
	}

	//when mouse clicks on head, instantiate/destroy selectborderclone
	//when moving to next round, gamestate calls on all selectedheads
	public void toggleSelected()
	{
		if(!isSelected && Grid.selectedHeads.Count == GameState.selectCount[GameState.gameRound])
		{
			Debug.Log("selected: " + Grid.selectedHeads.Count + " Max: " + GameState.selectCount[GameState.gameRound]);
			return;
		}
		isSelected = !isSelected;
		int index = Grid.heads.IndexOf(gameObject);
		if(isSelected && selectedBorderClone == null)
		{
			Grid.selectedHeads.Add(Grid.heads[index]);
			selectedBorderClone = Instantiate(Grid.selectedBorderSprite);
			selectedBorderClone.GetComponent<SpriteRenderer>().enabled = true;
			selectedBorderClone.transform.position = transform.position;
		}
		else if(!isSelected && selectedBorderClone != null)
		{
			Grid.selectedHeads.Remove(Grid.heads[index]);
			Destroy(selectedBorderClone);
		}
		else
		{
			Debug.Log("bool selected and gameObject selectedBorderClone not synced");
		}
	}

	private void killHead()
	{
		//do later, make restart button first
		if(selectedBorderClone != null)
		{
			int index = Grid.heads.IndexOf(gameObject);
			Debug.Log("Heads[" + index + "] is still selected");
			Grid.selectedHeads.Remove(Grid.heads[index]);
			Destroy(selectedBorderClone);
		}
		deadClone = Instantiate(Grid.deadSprite);
		deadClone.GetComponent<SpriteRenderer>().enabled = true;
		deadClone.transform.position = transform.position;
		coll.enabled = false;
		isDead = true;
	}
}
