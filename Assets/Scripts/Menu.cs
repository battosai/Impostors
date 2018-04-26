using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : UIHandler
{
  private Button startButton;
  private Button exitButton;
  private Button optionsButton;
  private Button aboutButton;
  private MeshRenderer aboutMesh;
  private string aboutText {get{return "dev::briantsai...thanksforplaying";}}

  void Awake()
  {
    startButton = GameObject.Find("StartButton").GetComponent<Button>();
    exitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    optionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
    aboutButton = GameObject.Find("AboutButton").GetComponent<Button>();
    aboutMesh = GameObject.Find("AboutText").GetComponent<MeshRenderer>();
  }

  void Start()
  {
    startButton.onClick.AddListener(startGame);
    exitButton.onClick.AddListener(exitApp);
    optionsButton.onClick.AddListener(customizeOptions);
    aboutButton.onClick.AddListener(displayAbout);
    //should also add loadsavedata call
  }

  void Update()
  {
  }

  //starts game when startButton is clicked
  void startGame()
  {
    //load scene with actual game
  }

  //exits app when exitButton is clicked
  void exitApp()
  {
    //save persistent data, exit app
  }

  //opens options menu when optionsButton is clicked
  void customizeOptions()
  {
    //be sure to set these values in playerpref
  }

  //display about page when aboutButton is clicked
  void displayAbout()
  {
    //give the player some brian bio
  }

  //make a new class for handling persistent data? yes pls
  void loadSaveData()
  {}
}
