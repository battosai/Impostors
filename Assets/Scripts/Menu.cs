using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  //private constants
  private Vector2 titleOffset {get{return new Vector2(0, -400);}}
  //private components
  private Button startButton;
  private Button exitButton;
  private Button optionsButton;
  private Button aboutButton;
  private Transform trans;
  // private MeshRenderer aboutMesh;
  // private string aboutText {get{return "dev::briantsai...thanksforplaying";}}

  void Awake()
  {
    //call load to call constructor for playerdata
    PlayerData.load();
    startButton = GameObject.Find("StartButton").GetComponent<Button>();
    exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    optionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
    aboutButton = GameObject.Find("AboutButton").GetComponent<Button>();
    trans = GetComponent<Transform>();
    // aboutMesh = GameObject.Find("AboutText").GetComponent<MeshRenderer>();
  }

  void Start()
  {
    startButton.onClick.AddListener(startGame);
    exitButton.onClick.AddListener(exitApp);
    optionsButton.onClick.AddListener(customizeOptions);
    aboutButton.onClick.AddListener(displayAbout);
    //transition from title to mainmenu
    StartCoroutine(titleTransition());
    //trans.position = titleOffset;
  }

  //coroutine for sliding title up and revealing main menu
  private IEnumerator titleTransition()
  {
    Debug.Log(Mathf.Abs(titleOffset.y));
    for(int i = 0; i <= Mathf.Abs(titleOffset.y); i++)
    {
      Vector2 pos = trans.localPosition;
      Debug.Log(trans.localPosition);
      pos += new Vector2(0, 1);
      trans.localPosition = pos;
      yield return null;//new WaitForSeconds(transitionTime);
    }
  }

  //starts game when startButton is clicked
  private void startGame()
  {
    //save playerprefs from menu scene
    PlayerData.save();
    //load scene with actual game
    //eventually make the game scene load asynchronously with a load screen
    //https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
    SceneManager.LoadScene("Game");
  }

  //exits app when exitButton is clicked
  private void exitApp()
  {
    //save persistent data
    PlayerData.save();
    Debug.Log("Saved!");
    //for editor, use below for exiting
    UnityEditor.EditorApplication.isPlaying = false;
    //for build, use below for exiting
    // Application.Quit();
  }

  //opens options menu when optionsButton is clicked
  private void customizeOptions()
  {
    //be sure to set these values in playerpref
    //Debug.Log("you aren't anyones' 'option' gurl");
    //temporary for testing saveload
    PlayerData.toggleMusic();
  }

  //display about page when aboutButton is clicked
  private void displayAbout()
  {
    //give the player some brian bio
    Debug.Log("do you have some time to learn about brian");
  }
}
