using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	//public but only changeable within class
	public Vector2 mousePosition {get; private set;}
	public SpriteRenderer rend {get; private set;}
	public Transform trans {get; private set;}
	//private read-only constants
	private float defaultMouseZ {get{return -9f;}}
	//completely private
	private Camera sceneCamera;

	void Awake()
	{
		sceneCamera = Camera.main;
		rend = GetComponent<SpriteRenderer>();
		trans = GetComponent<Transform>();
	}

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		takeUserInputs();
	}

	//takes user inputs
	private void takeUserInputs()
	{
		mousePosition = Input.mousePosition;
		mousePosition = sceneCamera.ScreenToWorldPoint(new Vector2(mousePosition.x, mousePosition.y));
		trans.position = new Vector3(mousePosition.x, mousePosition.y, defaultMouseZ);
	}
}
