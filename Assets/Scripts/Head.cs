using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Gridd
{
	//public attributes
	public bool isDefector {get; set;}
	public GameObject selectedBorderClone {get; set;}
	public GameObject deadClone {get; set;}
	//public attributes, but only changeable within class
	public bool isDead {get; private set;}
	public bool isSelected {get; private set;}
	public Collider2D coll {get; private set;}
	//private read-only constants
	private float defaultCloneZ {get{return -1f;}}
	private GameState gameState;

	void Awake()
	{
		coll = GetComponent<Collider2D>();
		gameState = GameObject.Find("Player").GetComponent<GameState>();
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
		//when selectCount is reached, can only deselect
		if(Gridd.selectedHeads.Count == GameState.selectCount[GameState.gameRound] && !isSelected)
			return;
		toggleSelected();
		gameState.playSelectionSound(isSelected);
	}

	//gamestate calls when replaying game
	public void reviveHead()
	{
		if(!isDead)
		{
			Debug.Log("Reviving a non-dead head");
			return;
		}
		if(isDead && deadClone != null)
		{
			isDead = false;
			Destroy(deadClone);
			coll.enabled = true;
		}
		else
		{
			Debug.Log("isDead and gameObject dead clone not synced");
		}
	}

	//when mouse clicks on head, instantiate/destroy selectborderclone
	//when moving to next round, gamestate calls on all selectedheads
	public void toggleSelected()
	{
		if(!isSelected && Gridd.selectedHeads.Count == GameState.selectCount[GameState.gameRound])
		{
			Debug.Log("selected: " + Gridd.selectedHeads.Count + " Max: " + GameState.selectCount[GameState.gameRound]);
			return;
		}
		isSelected = !isSelected;
		int index = Gridd.heads.IndexOf(gameObject);
		if(isSelected && selectedBorderClone == null)
		{
			Gridd.selectedHeads.Add(Gridd.heads[index]);
			selectedBorderClone = Instantiate(Gridd.selectedBorderSprite);
			selectedBorderClone.GetComponent<SpriteRenderer>().enabled = true;
			selectedBorderClone.transform.position = new Vector3(transform.position.x, transform.position.y, defaultCloneZ);
		}
		else if(!isSelected && selectedBorderClone != null)
		{
			Gridd.selectedHeads.Remove(Gridd.heads[index]);
			Destroy(selectedBorderClone);
		}
		else
		{
			Debug.Log("bool selected and gameObject selectedBorderClone not synced");
		}
	}

	public void killHead()
	{
		//do later, make restart button first
		if(selectedBorderClone != null)
		{
			int index = Gridd.heads.IndexOf(gameObject);
			Debug.Log("Heads[" + index + "] is still selected");
			Gridd.selectedHeads.Remove(Gridd.heads[index]);
			Destroy(selectedBorderClone);
		}
		deadClone = Instantiate(Gridd.deadSprite);
		deadClone.GetComponent<SpriteRenderer>().enabled = true;
		deadClone.transform.position = new Vector3(transform.position.x, transform.position.y, defaultCloneZ);
		coll.enabled = false;
		isDead = true;
	}
}
