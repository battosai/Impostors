using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefs2
{
	//The PlayerPrefs class in Unity does not include methods for bool types!

	public static void SetBool(string name, bool booleanValue)
	{
		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
	}

	//gets the bool value, defaults to false if doesn't exist
	public static bool GetBool(string name)
	{
		return PlayerPrefs.GetInt(name) == 1 ? true : false;
	}

	//returns defaultValue if key with 'name' doesn't exist
	//normally would just return false
	public static bool GetBool(string name, bool defaultValue)
	{
	 	if(PlayerPrefs.HasKey(name))
		{
    	return GetBool(name);
  	}
		return defaultValue;
	}
}
