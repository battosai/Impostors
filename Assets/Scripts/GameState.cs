using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
	//public read-only constants
	public int winScore {get{return 3;}}
	public int gameRoundCount {get{return 7;}}
	public static List<int> selectCount {get{return new List<int> {3, 4, 1, 5, 5, 1, 5};}}
	//public attributes
	public AudioClip roundVictorySound;
	public AudioClip roundLossSound;
	public AudioClip selectSound;
	public AudioClip deselectSound;
	//public attributes, but only changeable within class
	public int playerScore {get; private set;}
	public int defectorScore {get; private set;}
	public static int gameRound {get; private set;}
	//private attributes
	private AudioSource audioSource;
	private Button proceedButton;
	private Gridd gridd;
	private string missionText {get{return "Select ";}}
	private string interrogationText {get{return "Interrogate 1 Individual";}}

	void Awake()
	{
		gridd = GameObject.Find("Gridd").GetComponent<Gridd>();
		proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
		audioSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start()
	{
		resetRoundText();
		gridd.resetGridd();
		gameRound = 0;
		proceedButton.onClick.AddListener(proceedToNextRound);
		audioSource.loop = false;
		audioSource.playOnAwake = false;
	}

	public void resetRoundText()
	{
		GameObject textMesh = GameObject.Find("Textmesh");
		textMesh.GetComponent<TextMesh>().text = missionText + selectCount[gameRound].ToString();
	}

	public void playSelectionSound(bool isSelected)
	{
		Debug.Log("ey");
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

	// Update is called once per frame
	void Update()
	{
		checkSelectionCount();
	}

	//disables/enables proceed button if enough selections are made
	private void checkSelectionCount()
	{
		if(Gridd.selectedHeads.Count == selectCount[gameRound])
		{
			proceedButton.interactable = true;
		}
		else
		{
			proceedButton.interactable = false;
		}
	}

	//checks conditions for current round and advances to the next one
	private void proceedToNextRound()
{
		if(selectCount[gameRound] > 1)
		{
			//mission round currently
			bool isDefectorPresent = false;
			foreach(GameObject head in Gridd.selectedHeads)
			{
				isDefectorPresent = isDefectorPresent || head.GetComponent<Head>().isDefector;
			}
			if(isDefectorPresent)
				defectorWinRound();
			else
				playerWinRound();
		}
		else
		{
			//interrogation round currently
			GameObject currentHead = Gridd.selectedHeads[0];
			if(currentHead.GetComponent<Head>().isDefector)
				Debug.Log("THIS IS A DEFECTOR");
			else
				Debug.Log("THIS IS AN ALLY");
		}
		checkEndGame();
		gridd.resetSelectedHeads();
		gameRound++;
		//set up ui text for next round
		GameObject textMesh = GameObject.Find("Textmesh");
		if(selectCount[gameRound] == 1)
			textMesh.GetComponent<TextMesh>().text = interrogationText;
		else
			textMesh.GetComponent<TextMesh>().text = missionText + selectCount[gameRound].ToString();
	}

	private void checkEndGame()
	{
		if(playerScore == winScore || defectorScore == winScore)
		{
			if(playerScore > defectorScore)
				Debug.Log("Allied Victory!");
			else if(defectorScore > playerScore)
				Debug.Log("Defected Victory!");
			UnityEditor.EditorApplication.isPlaying = false;
			 //use Application.Quit();
		}
	}

	private void playerWinRound()
	{
		Debug.Log("Allies win this round!");
		audioSource.clip = roundVictorySound;
		audioSource.Play();
		int killIndex = Random.Range(0, Gridd.selectedHeads.Count);
		GameObject currentHead = Gridd.selectedHeads[killIndex];
		currentHead.GetComponent<Head>().killHead();
		playerScore++;
		//kill a head
	}

	private void defectorWinRound()
	{
		Debug.Log("Defectors win this round!");
		audioSource.clip = roundLossSound;
		audioSource.Play();
		defectorScore++;
	}
}
