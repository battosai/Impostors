using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
	//public read-only constants
	public int winScore {get{return 3;}}
	public int gameRoundCount {get{return 7;}}
	public static int waitTime {get{return 2;}}
	public static List<int> selectCount {get{return new List<int> {3, 4, 1, 5, 5, 1, 5};}}
	//public attributes, but only changeable within class
	public int playerScore {get; private set;}
	public int defectorScore {get; private set;}
	public static bool isGameOver {get; private set;}
	public static bool isEndGameChecked {get; private set;}
	public static int gameRound {get; private set;}
	//private attributes
	private UIHandler uiHandler;
	private Gridd gridd;
	private Button proceedButton;
	private Button replayButton;
	private Button exitButton;
	private Button backButton;

	void Awake()
	{
		uiHandler = GameObject.Find("Player").GetComponent<UIHandler>();
		gridd = GameObject.Find("Gridd").GetComponent<Gridd>();
		proceedButton = GameObject.Find("ProceedButton").GetComponent<Button>();
    replayButton = GameObject.Find("ReplayButton").GetComponent<Button>();
		exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
		backButton = GameObject.Find("BackButton").GetComponent<Button>();
	}

	// Use this for initialization
	void Start()
	{
		isGameOver = false;
		isEndGameChecked = false;
		gameRound = 0;
		playerScore = 0;
		defectorScore = 0;
		uiHandler.reset();
		gridd.resetGridd();
		gridd.resetSelectedHeads();
		proceedButton.onClick.AddListener(proceedToNextRound);
		replayButton.onClick.AddListener(replayGame);
		backButton.onClick.AddListener(backToMainMenu);
		exitButton.onClick.AddListener(exitApp);
	}

	// Update is called once per frame
	void Update()
	{
		//debugging: shows the number of people to select for current round
		// Debug.Log(selectCount[gameRound]);
	}

	//returns to main menu scene from game scene
	private void backToMainMenu()
	{
		//save playerprefs from game scene
		PlayerData.save();
		//load main menu scene
		SceneManager.LoadScene("MainMenu");
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

	//resets game scores, rounds, grid layout, and heads
	private void replayGame()
	{
		isGameOver = false;
		isEndGameChecked = false;
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
		uiHandler.reset();
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
		isEndGameChecked = false;
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
		if(gameRound + 1 < gameRoundCount)
			gameRound++;
		isEndGameChecked = true;
		//wait until uihandler is done processing
		if(uiHandler.isProcessing)
		{
			yield return new WaitForSeconds(0.1f);
		}
		gridd.resetSelectedHeads();
		// if(gameRound + 1 < gameRoundCount)
		// 	gameRound++;
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

	public void checkEndGame()
	{
		if(playerScore == winScore || defectorScore == winScore)
		{
			isGameOver = true;
			if(playerScore > defectorScore)
			{
				Debug.Log("Allied Victory!");
				PlayerData.addToken();
			}
			else if(defectorScore > playerScore)
			{
				Debug.Log("Defected Victory!");
			}
			uiHandler.enableEndGameMenu();
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
		defectorScore++;
		updateScoreboards();
	}
}
