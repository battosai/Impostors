using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//helpful links:
//https://gamedevelopment.tutsplus.com/tutorials/how-to-save-and-load-your-players-progress-in-unity--cms-20934
//https://www.raywenderlich.com/160815/save-load-game-unity

//[System.Serializable]
public static class PlayerData
{
	//public read-only keys to save
	[SerializeField]
	public static bool isMusicOn {get; private set;}
	[SerializeField]
	public static int tokens {get; private set;}

	//constructor, non-monobehaviour classes cannot use Awake, Start, etc
	//static constructor can't have access modifier
	//currently not necessary since does nothing, can use default constructor
	static PlayerData()
	{}

	//if key exists, load value, otherwise default to on
	public static void checkPlayerPrefs()
	{
		if(PlayerPrefs.HasKey("isMusicOn"))
		{
			Debug.Log("Getting Stored Value of " + PlayerPrefs2.GetBool("isMusicOn"));
			isMusicOn = PlayerPrefs2.GetBool("isMusicOn");
		}
		else
		{
			Debug.Log("Defaulting to a value of True");
			PlayerPrefs2.SetBool("isMusicOn", true);
		}
	}

	//temporary for testing saveload
	public static void toggleMusic()
	{
		isMusicOn = !isMusicOn;
		PlayerPrefs2.SetBool("isMusicOn", isMusicOn);
		Debug.Log("Set to " + PlayerPrefs2.GetBool("isMusicOn"));
	}

	//save everything, use when exiting app
	public static void save()
	{

	}

	//load everything, use when starting app
	public static void load()
	{

	}
}
