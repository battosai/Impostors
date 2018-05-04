using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  //public attributes
  public List<Sprite> musicToggleSprites;
  //private constants
  private Vector2 titleOffset {get{return new Vector2(0, -400);}}
  private int titleDisplayTime {get{return 3;}}
  //private components
  private Button startButton;
  private Button quitButton;
  private Button musicButton;
  private Button aboutButton;
  private Button backButton;
  private Image musicImage;
  private SpriteState musicSpriteState;
  private Transform trans;
  // private MeshRenderer aboutMesh;
  // private string aboutText {get{return "dev::briantsai...thanksforplaying";}}

  void Awake()
  {
    //call load to call constructor for playerdata
    PlayerData.load();
    startButton = GameObject.Find("StartButton").GetComponent<Button>();
    quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
    musicButton = GameObject.Find("MusicButton").GetComponent<Button>();
    aboutButton = GameObject.Find("AboutButton").GetComponent<Button>();
    backButton = GameObject.Find("BackButton").GetComponent<Button>();
    musicImage = GameObject.Find("MusicButton").GetComponent<Image>();
    musicSpriteState= GameObject.Find("MusicButton").GetComponent<Button>().spriteState;
    trans = GetComponent<Transform>();
    // aboutMesh = GameObject.Find("AboutText").GetComponent<MeshRenderer>();
    initializeMusicSprites();
  }

  void Start()
  {
    startButton.onClick.AddListener(startGame);
    quitButton.onClick.AddListener(exitApp);
    musicButton.onClick.AddListener(toggleMusic);
    aboutButton.onClick.AddListener(displayAbout);
    backButton.onClick.AddListener(backToMainWrapper);
    //transition from title to mainmenu
    StartCoroutine(transition(false, titleDisplayTime));
  }

  //acts as the onclick listener for backbutton
  private void backToMainWrapper()
  {
    StartCoroutine(transition(true, 0));
  }
  //coroutine for sliding from main menu to about
  private IEnumerator transition(bool up, int delay)
  {
    yield return new WaitForSeconds(delay);
    Vector2 startPos = trans.localPosition;
    //speeds up as i increases each loop
    int i = 1;
    //going back to mainmenu
    if(up)
    {
      while(trans.localPosition.y - startPos.y > titleOffset.y)
      {
        Vector2 pos = trans.localPosition;
        pos -= new Vector2(0, i);
        trans.localPosition = pos;
        i++;
        yield return null;
      }
    }
    //going to about
    else
    {
      while(startPos.y - trans.localPosition.y > titleOffset.y)
      {
        Vector2 pos = trans.localPosition;
        pos += new Vector2(0, i);
        trans.localPosition = pos;
        i++;
        yield return null;
      }
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

  //exits app when quitButton is clicked
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

  //only called at start of app
  private void initializeMusicSprites()
  {
    int i = 0;
    if(!PlayerData.isMusicOn)
      i++;
    musicImage.sprite = musicToggleSprites[2*i];
    musicSpriteState.pressedSprite = musicToggleSprites[2*i+1];
  }

  //toggles music when musicButton is clicked
  //NOT SURE WHY BUT PRESSED SPRITE DOESNT CHANGE
  private void toggleMusic()
  {
    //update playerdata
    PlayerData.toggleMusic();
    //swap sprites
    int i = 0;
    if(!PlayerData.isMusicOn)
      i++;
    musicImage.sprite = musicToggleSprites[2*i];
    musicSpriteState.pressedSprite = musicToggleSprites[(2*i)+1];
    Debug.Log("Pressed Sprite should be element " + (2*i+1));
  }

  //display about page when aboutButton is clicked
  private void displayAbout()
  {
    //give the player some brian bio
    Debug.Log("do you have some time to learn about brian");
    StartCoroutine(transition(false, 0));
  }
}
