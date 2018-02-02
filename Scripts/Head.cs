using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Grid
{
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	//Note: Can't have other colliders in the way (player object can't have collider)
	//when mouse clicks on head, (de)select it
	void OnMouseDown()
	{
		int index = Grid.heads.IndexOf(gameObject);
		Debug.Log(index);
	}
}
