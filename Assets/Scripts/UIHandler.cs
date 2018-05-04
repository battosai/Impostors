using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//handles notifications, buttons, sounds in the game scene
//also handles updating from playerdata file into the actual game

public class UIHandler : MonoBehaviour
{
  //public attributes
  public List<Sprite> missionResultSprites;
	public AudioClip missionVictorySound;
	public AudioClip missionLossSound;
	public AudioClip selectSound;
	public AudioClip deselectSound;
	public AudioClip missionInProgressSound;
	public AudioClip interrogationSound;
	public List<AudioClip> gunSounds;
  //private attributes
  private GameObject endGameMenu;
	private Image deathImage;
  private Image missionResultImage;
  private MeshRenderer missionResultMesh;
	private MeshRenderer deathMesh;
  private MeshRenderer instructionMesh;
	private AudioSource audioSource;
  private AudioSource musicAudioSource;
	private Button proceedButton;
  private string missionText {get{return "select " + GameState.selectCount[GameState.gameRound].ToString() + " members for a mission.";}}
  private string missionVictoryText {get{return "successful mission!";}}
  private string missionLossText {get{return "failed mission!";}}
  private string interrogationVictoryText {get{return "this is a defector!";}}
  private string interrogationLossText {get{return "this is an\nalliance member!";}}
  private string interrogationText {get{return "interrogate 1 individual";}}
	private string deathText {get{return "an alliance member\nhas been killed!";}}

  void Awake()
  {
    endGameMenu = GameObject.Find("EndGameMenu");
    instructionMesh = GameObject.Find("InstructionText").GetComponent<MeshRenderer>();
    deathImage = GameObject.Find("DeathImage").GetComponent<Image>();
    deathMesh = GameObject.Find("DeathText").GetComponent<MeshRenderer>();
    missionResultImage = GameObject.Find("MissionResultImage").GetComponent<Image>();
    missionResultMesh = GameObject.Find("MissionResultText").GetComponent<MeshRenderer>();
    proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
    audioSource = GetComponent<AudioSource>();
    musicAudioSource = GameObject.Find("GameCamera").GetComponent<AudioSource>();
    PlayerData.match(musicAudioSource);
  }

  void Start()
  {
  }

  void Update()
  {
    checkSelectionCount();
  }

  public void reset()
  {
    instructionMesh.enabled = true;
    instructionMesh.GetComponent<TextMesh>().text = missionText;
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
    deathImage.enabled = false;
    deathMesh.enabled = false;
    proceedButton.gameObject.GetComponent<Image>().enabled = true;
		proceedButton.interactable = true;
    endGameMenu.SetActive(false);
    audioSource.loop = false;
		audioSource.playOnAwake = false;
  }

  public void playSelectionSound(bool isSelected)
  {
    if(isSelected)
		{
			audioSource.clip = selectSound;
			audioSource.Play();
		}
		else
		{
			audioSource.clip = deselectSound;
			audioSource.Play();
		}
  }

  //replace with enable endgamemenu
  public void enableEndGameMenu()
  {
    proceedButton.gameObject.GetComponent<Image>().enabled = false;
    proceedButton.interactable = false;
    endGameMenu.SetActive(true);
  }

  private void checkSelectionCount()
  {
    if(Gridd.selectedHeads.Count == GameState.selectCount[GameState.gameRound])
    {
      proceedButton.interactable = true;
    }
    else
    {
      proceedButton.interactable = false;
    }
  }

  private void proceed()
  {
    if(GameState.selectCount[GameState.gameRound] > 1)
    {
      //mission round
      bool isDefectorPresent = false;
      foreach(GameObject head in Gridd.selectedHeads)
      {
        isDefectorPresent = isDefectorPresent || head.GetComponent<Head>().isDefector;
      }
      StartCoroutine(processMission(isDefectorPresent));
    }
    else
    {
      //interrogation round
			GameObject currentHead = Gridd.selectedHeads[0];
      StartCoroutine(processInterrogation(currentHead.GetComponent<Head>().isDefector));
    }
  }

  //process mission visuals
  public IEnumerator processMission(bool isDefectorPresent)
  {
    audioSource.clip = missionInProgressSound;
    audioSource.Play();
    yield return new WaitForSeconds(GameState.waitTime);
    displayMissionNotif(isDefectorPresent);
    yield return new WaitForSeconds(GameState.waitTime);
    removeMissionNotif();
    if(!isDefectorPresent)
    {
      displayDeathNotif();
      yield return new WaitForSeconds(GameState.waitTime);
      removeDeathNotif();
    }
    updateInstructions();
  }

  public IEnumerator processInterrogation(bool isDefector)
  {
    audioSource.clip = interrogationSound;
    audioSource.Play();
    yield return new WaitForSeconds(GameState.waitTime);
    displayInterrogationNotif(isDefector);
    yield return new WaitForSeconds(GameState.waitTime);
    removeInterrogationNotif();
    updateInstructions();
  }

  private void updateInstructions()
  {
    if(GameState.selectCount[GameState.gameRound] == 1)
      instructionMesh.GetComponent<TextMesh>().text = interrogationText;
    else
      instructionMesh.GetComponent<TextMesh>().text = missionText;
  }

  //shares objects with displayInterrogationNotif
  private void displayMissionNotif(bool isDefectorPresent)
  {
    missionResultImage.enabled = true;
    missionResultMesh.enabled = true;
    if(isDefectorPresent)
    {
      //fail
      audioSource.clip = missionLossSound;
      audioSource.Play();
      missionResultImage.sprite = missionResultSprites[1];
      missionResultMesh.GetComponent<TextMesh>().text = missionLossText;
    }
    else
    {
      //success
      audioSource.clip = missionVictorySound;
      audioSource.Play();
      missionResultImage.sprite = missionResultSprites[0];
      missionResultMesh.GetComponent<TextMesh>().text = missionVictoryText;
    }
  }

  //functionally the same as removeInterrogationNotif, they share objects
  private void removeMissionNotif()
  {
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
  }

  //shares objects with displayMissionNotif
  private void displayInterrogationNotif(bool isDefector)
  {
    missionResultImage.enabled = true;
    missionResultMesh.enabled = true;
    if(isDefector)
    {
      //found defector
      missionResultImage.sprite = missionResultSprites[0];
      missionResultMesh.GetComponent<TextMesh>().text = interrogationVictoryText;
    }
    else
    {
      //found alliance member
      missionResultImage.sprite = missionResultSprites[1];
      missionResultMesh.GetComponent<TextMesh>().text = interrogationLossText;
    }
  }

  //functionally the same as removeMissionNotif, they share objects
  private void removeInterrogationNotif()
  {
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
  }

  private void displayDeathNotif()
  {
    audioSource.clip = gunSounds[Random.Range(0, 2)];
    audioSource.Play();
    deathImage.enabled = true;
    deathMesh.enabled = true;
    deathMesh.GetComponent<TextMesh>().text = deathText;
  }
  private void removeDeathNotif()
  {
    deathImage.enabled = false;
    deathMesh.enabled = false;
  }
}
