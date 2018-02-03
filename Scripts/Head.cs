using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Grid
{
	public float defaultSelectedBorderZ {get{return -9.5f;}}
	//public attributes, but only changeable within class
	public bool isSelected {get; private set;}
	public Collider2D coll {get; private set;}
	public GameObject selectedBorderClone {get; private set;}
	public GameObject deadClone {get; private set;}

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
	private void toggleSelected()
	{
		isSelected = !isSelected;
		int index = Grid.heads.IndexOf(gameObject);
		if(isSelected && selectedBorderClone == null)
		{
			Grid.selectedHeads.Add(Grid.heads[index]);
			selectedBorderClone = Instantiate(Grid.selectedBorder);
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
		deadClone = Instantiate(Grid.dead);
		deadClone.GetComponent<SpriteRenderer>().enabled = true;
		deadClone.transform.position = transform.position;
		coll.enabled = false;
	}
}
