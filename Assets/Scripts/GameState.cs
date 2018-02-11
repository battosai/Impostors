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
	public AudioClip missionInProgressSound;
	public AudioClip interrogationSound;
	public List<AudioClip> gunSounds;
	//public attributes, but only changeable within class
	public int playerScore {get; private set;}
	public int defectorScore {get; private set;}
	public static int gameRound {get; private set;}
	//private attributes
	private AudioSource audioSource;
	private Button proceedButton;
	private Button replayButton;
	private Gridd gridd;
	private string missionText {get{return "select " + selectCount[gameRound].ToString() + " members for a mission.";}}
	private string interrogationText {get{return "interrogate 1 individual";}}
	private string deathText {get{return "an alliance member has been killed!";}}

	void Awake()
	{
		gridd = GameObject.Find("Gridd").GetComponent<Gridd>();
		proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
		replayButton = GameObject.Find("ReplayButton").GetComponent<Button>();
		audioSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start()
	{
		resetRoundText();
		gridd.resetGridd();
		gridd.resetSelectedHeads();
		gameRound = 0;
		playerScore = 0;
		defectorScore = 0;
		proceedButton.onClick.AddListener(proceedToNextRound);
		replayButton.onClick.AddListener(replayGame);
		proceedButton.gameObject.GetComponent<Image>().enabled = true;
		proceedButton.interactable = true;
		replayButton.gameObject.GetComponent<Image>().enabled = false;
		replayButton.interactable = false;
		audioSource.loop = false;
		audioSource.playOnAwake = false;
	}

	public void resetRoundText()
	{
		GameObject instructionText = GameObject.Find("InstructionText");
		instructionText.GetComponent<TextMesh>().text = missionText;
	}

	//called by heads individually
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

	private void replayGame()
	{
		Debug.Log("Resetting the Game");
		gameRound = 0;
		playerScore = 0;
		defectorScore = 0;
		proceedButton.gameObject.GetComponent<Image>().enabled = true;
		proceedButton.interactable = true;
		replayButton.gameObject.GetComponent<Image>().enabled = false;
		replayButton.interactable = false;
		audioSource.loop = false;
		audioSource.playOnAwake = false;
		resetRoundText();
		updateScoreboards();
		gridd.resetGridd();
		gridd.resetSelectedHeads();
		foreach(GameObject head in Gridd.heads)
		{
			head.GetComponent<Head>().reviveHead();
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
		if(gameRound + 1 < gameRoundCount)
			gameRound++;
		//set up ui text for next round
		GameObject instructionText = GameObject.Find("InstructionText");
		if(selectCount[gameRound] == 1)
			instructionText.GetComponent<TextMesh>().text = interrogationText;
		else
			instructionText.GetComponent<TextMesh>().text = missionText;
	}

	private void checkEndGame()
	{
		if(playerScore == winScore || defectorScore == winScore)
		{
			if(playerScore > defectorScore)
				Debug.Log("Allied Victory!");
			else if(defectorScore > playerScore)
				Debug.Log("Defected Victory!");
			//enableReplayButton();
			//disable if testing replay button
			UnityEditor.EditorApplication.isPlaying = false;
			//use Application.Quit();
		}
	}

	private void updateScoreboards()
	{
		GameObject alliedScoreboard = GameObject.Find("AlliedScoreboardText");
		alliedScoreboard.GetComponent<TextMesh>().text = playerScore.ToString();
		GameObject defectedScoreboard = GameObject.Find("DefectedScoreboardText");
		defectedScoreboard.GetComponent<TextMesh>().text = defectorScore.ToString();
	}

	private void playerWinRound()
	{
		Debug.Log("Allies win this round!");
		audioSource.clip = roundVictorySound;
		audioSource.Play();
		//kill head
		int killIndex = Random.Range(0, Gridd.selectedHeads.Count);
		GameObject currentHead = Gridd.selectedHeads[killIndex];
		currentHead.GetComponent<Head>().killHead();
		playerScore++;
		updateScoreboards();
	}

	private void defectorWinRound()
	{
		Debug.Log("Defectors win this round!");
		audioSource.clip = roundLossSound;
		audioSource.Play();
		defectorScore++;
		updateScoreboards();
	}

	//enable replay button over the proceed button
	private void enableReplayButton()
	{
		proceedButton.gameObject.GetComponent<Image>().enabled = false;
		proceedButton.interactable = false;
		replayButton.gameObject.GetComponent<Image>().enabled = true;
		replayButton.interactable = true;
	}
}
