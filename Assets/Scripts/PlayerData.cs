using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TO DO:
//currently no way of 'exiting' from game scene, thus does not save playerprefs. only saves if exit button is used from mainmenu

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
	//not necessary right now since no different than default constructor
	static PlayerData()
	{}

	//called by UIHandler for Game scene and Menu for MainMenu scene
	//matches scene to settings
	public static void match(AudioSource audioSource)
	{
		audioSource.enabled = isMusicOn;
	}

	//toggles the music, doesn't save to playerprefs
	public static void toggleMusic()
	{
		isMusicOn = !isMusicOn;
		Debug.Log("Music is set to " + isMusicOn);
	}

	//adds a token to the current count, doesn't save to playerprefs
	public static void addToken()
	{
		tokens++;
	}

	//save everything to playerprefs, use when exiting app
	public static void save()
	{
		PlayerPrefs2.SetBool("isMusicOn", isMusicOn);
		PlayerPrefs.SetInt("tokens", tokens);
		PlayerPrefs.Save();
	}

	//load everything, use when starting app
	//if key exists, load value, otherwise default
	public static void load()
	{
		//music
		if(PlayerPrefs.HasKey("isMusicOn"))
		{
			Debug.Log("Music was " + isMusicOn + " last time");
			isMusicOn = PlayerPrefs2.GetBool("isMusicOn");
		}
		else
		{
			Debug.Log("Just gonna turn some music on");
			PlayerPrefs2.SetBool("isMusicOn", true);
			isMusicOn = true;
		}
		//tokens
		if(PlayerPrefs.HasKey("tokens"))
		{
			Debug.Log("Player had " + tokens + " tokens last time");
			tokens = PlayerPrefs.GetInt("tokens");
		}
		else
		{
			Debug.Log("Ya got nada tokens");
			PlayerPrefs.SetInt("tokens", 0);
			tokens = 0;
		}
	}
}
