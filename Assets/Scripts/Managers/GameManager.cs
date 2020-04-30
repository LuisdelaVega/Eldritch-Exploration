﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
  public static GameManager instance = null;

  /* Player */
  public GameObject playerPrefab;
  [HideInInspector] public GameObject player;

  /* Cinemachine */
  public CinemachineVirtualCamera vcam1;

  /* Controls */
  private Controls controls;

  /* Level */
  public int level = 1;

  /* UI */
  public GameObject resetText;

  /* Resources */
  string[] firstNames;
  string[] lastNames;

  [SerializeField] private bool promo = false;

  // Start is called before the first frame update
  void Awake()
  {
    if (instance == null)
      instance = this;
    else if (instance != this)
    {
      Destroy(gameObject);
      return;
    }

    DontDestroyOnLoad(gameObject);

    // Initialize Controls
    controls = new Controls();

    // Get first and last names
    firstNames = Resources.Load<TextAsset>("FirstNames").text.Split("\n"[0]);
    lastNames = Resources.Load<TextAsset>("LastNames").text.Split("\n"[0]);

    //Call the InitGame function to initialize the first level
    InitGame();
  }

  // private void Start() => AudioManager.instance.PlayWithRandomPitch("Door Open", 0.9f, 1.1f);

  void OnEnable()
  {
    controls.Enable();
    controls.GameManager.Restart.performed += _ => Restart();
  }

  void OnDisable() => controls.Disable();

  void InitGame()
  {
    resetText.SetActive(false);
    if (!promo)
      InstantiatePlayer();
  }

  public void InstantiatePlayer()
  {
    player = Instantiate(playerPrefab, new Vector2(-8, 0), Quaternion.identity);
    vcam1.LookAt = player.transform;
    vcam1.Follow = player.transform;
    string name = $"{firstNames[Random.Range(0, firstNames.Length)]} {lastNames[Random.Range(0, lastNames.Length)]}";
    player.GetComponent<Player>().SayName(name);
  }

  public void GameOver() => resetText.SetActive(true);

  private void Restart()
  {
    RoomTemplates.instance.seedTextSet = false;
    RoomTemplates.instance.NewSeed();
    RoomTemplates.instance.timer = RoomTemplates.instance.waitTime;
    RoomTemplates.instance.spawedBoss = false;
    // Coroutines
    StopAllCoroutines();

    // // Player Character
    if (player != null)
      Destroy(player.gameObject);

    Destroy(gameObject);

    // Init game
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
