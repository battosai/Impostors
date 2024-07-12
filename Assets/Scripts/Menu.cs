using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
  /// <summary>
  /// Object for the title image.
  /// </summary>
  private GameObject title;

  /// <summary>
  /// Parent object for the main menu buttons.
  /// </summary>
  private GameObject mainMenu;
  
  /// <summary>
  /// Button for start game.
  /// </summary>
  private Button startButton;
  
  /// <summary>
  /// Button for quit game.
  /// </summary>
  private Button quitButton;
  
  /// <summary>
  /// Button for mute/unmute music.
  /// </summary>
  private Button musicButton;
  
  /// <summary>
  /// Image component of music button.
  /// </summary>
  private Image musicImage;
  
  /// <summary>
  /// Sprite state of music button.
  /// </summary>
  private SpriteState musicSpriteState;

  /// <summary>
  /// Pressed/unpressed states for music when off/on.
  /// </summary>
  public List<Sprite> musicToggleSprites;

  /// <summary>
  /// Button for tutorial info.
  /// </summary>
  private Button tutorialButton;

  /// <summary>
  /// Parent object for tutorial page.
  /// </summary>
  private GameObject tutorial;

  /// <summary>
  /// Button to go back from tutorial to main menu.
  /// </summary>
  private Button backButton;

  /// <summary>
  /// Music sound source.
  /// </summary>
  private AudioSource audioSource;

  /// <summary>
  /// Reference to click sound for buttons.
  /// </summary>
  public AudioClip click;

  /// <summary>
  /// Cached position of title at start. 
  /// </summary>
  private Vector3 titleStartPos;  

  /// <summary>
  /// Cached position of main menu at start.
  /// </summary>
  private Vector3 mainMenuStartPos;

  /// <summary>
  /// Initialization Pt I.
  /// </summary>
  private void Awake()
  {
    title = transform.Find("Title").gameObject;

    // Main menu references
    mainMenu = transform.Find("Main").gameObject;
    startButton = mainMenu.transform.Find("StartButton").GetComponent<Button>();
    quitButton = mainMenu.transform.Find("QuitButton").GetComponent<Button>();
    musicButton = mainMenu.transform.Find("MusicButton").GetComponent<Button>();
    musicImage = musicButton.GetComponent<Image>();
    musicSpriteState = musicButton.spriteState;
    tutorialButton = mainMenu.transform.Find("TutorialButton").GetComponent<Button>();

    // Tutorial references
    tutorial = transform.Find("Tutorial").gameObject;
    backButton = tutorial.transform.Find("BackButton").GetComponent<Button>();

    audioSource = Camera.main.GetComponent<AudioSource>();
  }

  /// <summary>
  /// Initialization Pt II.
  /// </summary>
  private void Start()
  {
    startButton.onClick.AddListener(StartGame);
    quitButton.onClick.AddListener(ExitApp);
    musicButton.onClick.AddListener(ToggleMusic);
    tutorialButton.onClick.AddListener(DisplayTutorial);
    backButton.onClick.AddListener(() =>
    {
      MainMenu(true);
    });

    // Toggle music based on player data
    PlayerData.load();
    audioSource.enabled = PlayerData.isMusicOn;
    int i = PlayerData.isMusicOn ? 0 : 1;
    musicImage.sprite = musicToggleSprites[2 * i];
    musicSpriteState.pressedSprite = musicToggleSprites[2 * i + 1];

    // Save starting positions
    titleStartPos = title.transform.position;
    mainMenuStartPos = mainMenu.transform.position;

    mainMenu.SetActive(false);
    tutorial.SetActive(false);
    title.SetActive(true);

    // Blink animation = 1.5s
    // Move animation = 1.5s
    StartCoroutine(IntroSequence(3f));
  }

  /// <summary>
  /// Wrapper for displaying main menu after a delay.
  /// </summary>
  private IEnumerator IntroSequence(float delay_s)
  {
    if (delay_s < 1.5f)
    {
      delay_s += 1.5f;
    }

    yield return new WaitForSeconds(1.5f);

    float moveDuration_s = delay_s - 1.5f;

    Coroutine titleUp = StartCoroutine(Move(
      title.transform,
      titleStartPos,
      titleStartPos + Vector3.up * 10f,
      moveDuration_s));

    Coroutine mainMenuUp = StartCoroutine(Move(
      mainMenu.transform,
      mainMenuStartPos + Vector3.down * 10f,
      mainMenuStartPos,
      moveDuration_s));

    mainMenu.SetActive(true);

    yield return titleUp;
    yield return mainMenuUp;
    
    MainMenu(false);
  }

  /// <summary>
  /// Displays the main menu.
  /// </summary>
  private void MainMenu(bool clicked)
  {
    if (clicked == true)
    {
      audioSource.PlayOneShot(click);
    }

    title.SetActive(false);
    tutorial.SetActive(false);
    mainMenu.SetActive(true);
  }

  /// <summary>
  /// Starts the game.
  /// </summary>
  private void StartGame()
  {
    audioSource.PlayOneShot(click);

    // Save playerprefs from menu scene
    PlayerData.save();
    SceneManager.LoadScene("Game");
  }

  /// <summary>
  /// Exits game.
  /// </summary>
  private void ExitApp()
  {
    audioSource.PlayOneShot(click);

    // Save persistent data
    PlayerData.save();

    #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
    #else
      Application.Quit();
    #endif
  }

  /// <summary>
  /// Mute/unmute music.
  /// </summary>
  private void ToggleMusic()
  {
    // Update playerdata
    PlayerData.toggleMusic();

    // Swap sprites
    int i = PlayerData.isMusicOn ? 0 : 1;
    musicImage.sprite = musicToggleSprites[2*i];
    musicSpriteState.pressedSprite = musicToggleSprites[(2*i)+1];

    audioSource.enabled = PlayerData.isMusicOn;
    audioSource.PlayOneShot(click);
  }

  /// <summary>
  /// Show the tutorial page.
  /// </summary>
  private void DisplayTutorial()
  {
    audioSource.PlayOneShot(click);

    title.SetActive(false);
    mainMenu.SetActive(false);
    tutorial.SetActive(true);
  }

  /// <summary>
  /// Helper for moving windows.
  /// </summary>
  private IEnumerator Move(
    Transform xform,
    Vector3 start,
    Vector3 end,
    float duration_s)
  {
    float start_s = Time.time;
    float elapsed_s = 0f;

    while (elapsed_s <= duration_s)
    {
      elapsed_s = Mathf.Pow(
        (Time.time - start_s),
        2f) * 5f;

      float ratio = elapsed_s / duration_s;
      ratio = Mathf.Min(
        ratio,
        1f);

      Vector3 pos = Vector3.Lerp(
        start,
        end,
        ratio);
      
      xform.position = pos;
      yield return null;
    }

    xform.position = end;
  }
}
