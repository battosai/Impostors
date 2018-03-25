using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
	//public read-only constants
	public int winScore {get{return 3;}}
	public int gameRoundCount {get{return 7;}}
	public static int waitTime {get{return 2;}}
	public static List<int> selectCount {get{return new List<int> {3, 4, 1, 5, 5, 1, 5};}}
	//public attributes
	//public attributes, but only changeable within class
	public int playerScore {get; private set;}
	public int defectorScore {get; private set;}
	public static bool canRestart {get; private set;}
	public static int gameRound {get; private set;}
	//private attributes
	private UIHandler uiHandler;
	private Gridd gridd;
	private Button proceedButton;
	private Button replayButton;

	void Awake()
	{
		uiHandler = GameObject.Find("Player").GetComponent<UIHandler>();
		gridd = GameObject.Find("Gridd").GetComponent<Gridd>();
		proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
    replayButton = GameObject.Find("ReplayButton").GetComponent<Button>();
	}

	// Use this for initialization
	void Start()
	{
		uiHandler.reset();
		gridd.resetGridd();
		gridd.resetSelectedHeads();
		canRestart = false;
		gameRound = 0;
		playerScore = 0;
		defectorScore = 0;
		proceedButton.onClick.AddListener(proceedToNextRound);
		replayButton.onClick.AddListener(replayGame);
	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log(Gridd.selectedHeads.Count);
	}

	private void replayGame()
	{
		uiHandler.reset();
		gameRound = 0;
		playerScore = 0;
		defectorScore = 0;
		updateScoreboards();
		gridd.resetGridd();
		gridd.resetSelectedHeads();
		foreach(GameObject head in Gridd.heads)
		{
			if(head.GetComponent<Head>().isDead && head.GetComponent<Head>().deadClone != null)
				head.GetComponent<Head>().reviveHead();
		}
	}

	//checks conditions for current round and advances to the next one
	private void proceedToNextRound()
	{
		if(selectCount[gameRound] > 1)
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

	private IEnumerator processMission(bool isDefectorPresent)
	{
		Debug.Log("processing mission...");
		StartCoroutine(uiHandler.processMission(isDefectorPresent));
		yield return new WaitForSeconds(waitTime*2);
		if(isDefectorPresent)
		{
			defectorWinRound();
		}
		else
		{
			playerWinRound();
		}
		checkEndGame();
		gridd.resetSelectedHeads();
		if(gameRound + 1 < gameRoundCount)
			gameRound++;
	}

	private IEnumerator processInterrogation(bool isDefector)
	{
		Debug.Log("processing interrogation...");
		StartCoroutine(uiHandler.processInterrogation(isDefector));
		yield return new WaitForSeconds(waitTime);
		checkEndGame();
		gridd.resetSelectedHeads();
		if(gameRound + 1 < gameRoundCount)
			gameRound++;
	}

	private void checkEndGame()
	{
		if(playerScore == winScore || defectorScore == winScore)
		{
			if(playerScore > defectorScore)
				Debug.Log("Allied Victory!");
			else if(defectorScore > playerScore)
				Debug.Log("Defected Victory!");
			uiHandler.enableReplayButton();
			//disable if testing replay button
			//UnityEditor.EditorApplication.isPlaying = false;
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
		// audioSource.clip = roundLossSound;
		// audioSource.Play();
		defectorScore++;
		updateScoreboards();
	}

	//enable replay button over the proceed button
	// private void enableReplayButton()
	// {
	// 	proceedButton.gameObject.GetComponent<Image>().enabled = false;
	// 	proceedButton.interactable = false;
	// 	replayButton.gameObject.GetComponent<Image>().enabled = true;
	// 	replayButton.interactable = true;
	// }
}
