using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//handles notifications, buttons, sounds in the game scene
//also handles updating from playerdata file into the actual game

public class UIHandler : MonoBehaviour
{
  //public read-only attributes
  public bool isProcessing;
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
  private Image proceedImage;
  private MeshRenderer missionResultMesh;
  private TextMesh missionResultTextMesh;
	private MeshRenderer deathMesh;
  private MeshRenderer instructionMesh;
  private TextMesh instructionTextMesh;
	private AudioSource audioSource;
  private AudioSource musicAudioSource;
	private Button proceedButton;
  private string missionText {get{ return $"pick {GameState.selectCount[GameState.gameRound].ToString()} members\nfor a mission";}}
  private string missionVictoryText = "nice work team!";
  private string missionLossText = "sabotaged!";
  private string interrogationVictoryText = "this is an impostor!";
  private string interrogationLossText = "this is an\nalliance member!";
  private string interrogationText = "interrogate 1\nindividual";
	private string deathText = "an alliance member\nhas been killed!";
  private string processingText = "...";

  void Awake()
  {
    endGameMenu = GameObject.Find("EndGameMenu");
    instructionMesh = GameObject.Find("InstructionText").GetComponent<MeshRenderer>();
    instructionTextMesh = instructionMesh.GetComponent<TextMesh>();
    deathImage = GameObject.Find("DeathImage").GetComponent<Image>();
    deathMesh = GameObject.Find("DeathText").GetComponent<MeshRenderer>();
    missionResultImage = GameObject.Find("MissionResultImage").GetComponent<Image>();
    missionResultMesh = GameObject.Find("MissionResultText").GetComponent<MeshRenderer>();
    missionResultTextMesh = missionResultMesh.GetComponent<TextMesh>();
    proceedImage = GameObject.Find("ProceedButton").GetComponent<Image>();
    proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
    audioSource = GetComponent<AudioSource>();
    musicAudioSource = GameObject.Find("GameCamera").GetComponent<AudioSource>();
    PlayerData.match(musicAudioSource);
  }

  //called in gamestate Start and on reset
  public void reset()
  {
    updateInstructions();
    isProcessing = false;
    instructionMesh.enabled = true;
    instructionTextMesh.text = missionText;
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
    deathImage.enabled = false;
    deathMesh.enabled = false;
    proceedImage.enabled = true;
    proceedButton.gameObject.GetComponent<Image>().enabled = true;
		proceedButton.interactable = false;
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
    Debug.Log("Proceed should not be visible at end game");
    proceedImage.enabled = false;
    proceedButton.interactable = false;
    endGameMenu.SetActive(true);
  }

  public void checkSelectionCount()
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

  //process mission visuals
  public IEnumerator processMission(bool isDefectorPresent)
  {
    isProcessing = true;
    audioSource.clip = missionInProgressSound;
    audioSource.Play();
    instructionTextMesh.text = processingText;
    yield return new WaitForSeconds(GameState.waitTime);
    proceedImage.enabled = false;
    displayMissionNotif(isDefectorPresent);
    yield return new WaitForSeconds(GameState.waitTime);
    removeMissionNotif();
    //waits for gamestate.processinterrogation to update scores
    //prevents death notifs from popping up when player wins
    if(!GameState.isEndGameChecked)
    {
      yield return new WaitForSeconds(0.1f);
    }
    if(!isDefectorPresent && !GameState.isGameOver)
    {
      displayDeathNotif();
      yield return new WaitForSeconds(GameState.waitTime);
      removeDeathNotif();
    }
    isProcessing = false;
    //can insert victory and defeat sections here
    if(!GameState.isGameOver)
      proceedImage.enabled = true;
    updateInstructions();
  }

  public IEnumerator processInterrogation(bool isDefector)
  {
    audioSource.clip = interrogationSound;
    audioSource.Play();
    instructionTextMesh.text = processingText;
    yield return new WaitForSeconds(GameState.waitTime);
    proceedImage.enabled = false;
    displayInterrogationNotif(isDefector);
    yield return new WaitForSeconds(GameState.waitTime);
    removeInterrogationNotif();
    proceedImage.enabled = true;
    updateInstructions();
  }

  private void updateInstructions()
  {
    if(GameState.selectCount[GameState.gameRound] == 1)
      instructionTextMesh.text = interrogationText;
    else
      instructionTextMesh.text = missionText;
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
      missionResultTextMesh.text = missionLossText;
    }
    else
    {
      //success
      audioSource.clip = missionVictorySound;
      audioSource.Play();
      missionResultImage.sprite = missionResultSprites[0];
      missionResultTextMesh.text = missionVictoryText;
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
      audioSource.clip = missionLossSound;
      audioSource.Play();
      missionResultImage.sprite = missionResultSprites[1];
      missionResultTextMesh.text = interrogationVictoryText;
    }
    else
    {
      //found alliance member
      audioSource.clip = missionVictorySound;
      audioSource.Play();
      missionResultImage.sprite = missionResultSprites[0];
      missionResultTextMesh.text = interrogationLossText;
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
