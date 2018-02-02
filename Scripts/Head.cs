using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Grid
{
	public float defaultSelectedBorderZ {get{return -9.5f;}}
	//public attributes, but only changeable within class
	public bool selected {get; private set;}
	public GameObject selectedBorderClone {get; private set;}

	// Use this for initialization
	void Start()
	{
		selected = false;
	}

	// Update is called once per frame
	void Update()
	{
	}

	//Note: Can't have other colliders in the way (player object can't have collider)
	//when mouse clicks on head, (de)select it
	void OnMouseDown()
	{
		selected = !selected;
		int index = Grid.heads.IndexOf(gameObject);
		Debug.Log(index);
		if(selected && selectedBorderClone == null)
		{
			Grid.selectedHeads.Add(Grid.heads[index]);
			selectedBorderClone = Instantiate(Grid.selectedBorder);
			selectedBorderClone.GetComponent<SpriteRenderer>().enabled = true;
			selectedBorderClone.transform.position = transform.position;
		}
		else if(!selected && selectedBorderClone != null)
		{
			Grid.selectedHeads.Remove(Grid.heads[index]);
			Destroy(selectedBorderClone);
		}
		else
		{
			Debug.Log("bool selected and gameObject selectedBorderClone not synced");
		}
	}
}
