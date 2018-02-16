using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private Image deathImage;
  private List<Image> missionResultImage;
  private MeshRenderer missionResultMesh;
	private MeshRenderer deathMesh;
	private AudioSource audioSource;
	private Button proceedButton;
	private Button replayButton;
  private string missionText {get{return "select " + selectCount[gameRound].ToString() + " members for a mission.";}}
  private string missionVictoryText {get{return "successful mission!";}}
  private string missionLossText {get{return "failed mission!";}}
  private string interrogationText {get{return "interrogate 1 individual";}}
	private string deathText {get{return "an alliance member has been killed!";}}

  void Awake()
  {
    deathImage = GameObject.Find("DeathImage").GetComponent<Image>();
    deathMesh = GameObject.Find("DeathText").GetComponent<MeshRenderer>();
    missionResultImage = GameObject.Find("MissionResultImage").GetComponent<Image>();
    missionResultMesh = GameObject.Find("MissionResultText").GetComponent<MeshRenderer>();
    proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
    replayButton = GameObject.Find("ReplayButton").GetComponent<Button>();
    audioSource = GetComponent<AudioSource>();
  }

  void Start()
  {
    reset();
    proceedButton.onClick.AddListener(proceed);
		replayButton.onClick.AddListener(replay);
  }

  public static void reset()
  {
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
    deathImage.enabled = false;
    deathMesh.enabled = false;
    proceedButton.gameObject.GetComponent<Image>().enabled = true;
		proceedButton.interactable = true;
		replayButton.gameObject.GetComponent<Image>().enabled = false;
		replayButton.interactable = false;
    audioSource.loop = false;
		audioSource.playOnAwake = false;
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
    }
  }

  private IEnumerator processMission(bool isDefectorPresent)
  {
    audioSource.clip = missionInProgressSound;
    audioSource.Play();
    yield return new WaitForSeconds(2);
    displayMissionNotif();
    yield return new WaitForSeconds(5);
    removeMissionNotif();
    if(!isDefectorPresent)
    {
      displayDeathNotif();
      yield return new WaitForSeconds(5);
      removeDeathNotif();
    }
  }

  private void displayMissionNotif(bool isDefectorPresent)
  {
    missionResultImage.enabled = true;
    missionResultMesh.enabled = true;
    if(isDefectorPresent)
    {
      missionResultImage.image = missionResultSprites[1];
      missionResultMesh.text = missionLossText;
    }
    else
    {
      missionResultImage.image = missionResultSprites[0];
      missionResultMesh.text = missionVictoryText;
    }
  }
  private void removeMissionNotif()
  {
    missionResultImage.enabled = false;
    missionResultMesh.enabled = false;
  }
  private void displayDeathNotif()
  {
    deathImage.enabled = true;
    deathMesh.enabled = true;
    deathMesh.text = deathText;
  }
  private void removeDeathNotif()
  {
    deathImage.enabled = false;
    deathMesh.enabled = false;
  }
}
