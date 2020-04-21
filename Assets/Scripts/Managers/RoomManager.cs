﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; //Tells Random to use the Unity Engine random number generator.

public class RoomManager : MonoBehaviour
{
  [SerializeField] private bool arcadeMode = false;
  /* Room */
  // [SerializeField] private bool roomIsActive = false;
  public CompositeCollider2D walls;
  private bool roomIsActive = false;

  /* Lights */
  public RoomLightsManager roomLightsManager;
  private Coroutine toggleDim;

  /* Enemies */
  public List<Enemy> enemyPrefabList;
  private int enemiesToSpawn = 0;
  private bool waitingToSpawnEnemy = true;
  private int enemiesSpawnedInRoom = 0;
  private int enemiesInRoom = 0;
  [SerializeField] private float spawnEnemyCooldown = 2f;

  private void Start()
  {
    if (arcadeMode) SetUpRoom();
  }

  private void UpdateEnemyCount()
  {
    if (--enemiesInRoom == 0 && enemiesSpawnedInRoom == enemiesToSpawn) // TODO: Open the doors
    {
      roomIsActive = false;
      Debug.Log("Cleared room");
      StopCoroutine(toggleDim);
      roomLightsManager.TurnOnLights();
      Enemy.OnEnemyKilled -= UpdateEnemyCount;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (roomIsActive && !waitingToSpawnEnemy && (enemiesSpawnedInRoom < enemiesToSpawn || arcadeMode))
      StartCoroutine(SpawnEnemies());
  }

  public void SetUpRoom()
  {
    enemiesToSpawn = GetAmountOfEnemiesToSpawn();
    toggleDim = StartCoroutine(roomLightsManager.ToggleDim(true));
    waitingToSpawnEnemy = false;
    roomIsActive = true;
    if (!arcadeMode)
      Enemy.OnEnemyKilled += UpdateEnemyCount;
  }

  public IEnumerator SpawnEnemies()
  {
    waitingToSpawnEnemy = true;
    yield return new WaitForSeconds(spawnEnemyCooldown);
    if (GameManager.instance.player != null)
    {
      Vector2 distanceToPlayer;
      Vector2 spawnLocation;

      do
      {
        spawnLocation = new Vector2(
          Random.Range(walls.bounds.min.x + 5, walls.bounds.max.x - 5),
          Random.Range(walls.bounds.min.y + 5, walls.bounds.max.y - 5)
        );
        distanceToPlayer = spawnLocation - (Vector2)GameManager.instance.player.transform.position;
      } while (distanceToPlayer.sqrMagnitude < 50); // TODO: Revisit this number

      int index = Random.Range(0, enemyPrefabList.Count);
      var enemy = Instantiate(enemyPrefabList[index], spawnLocation, Quaternion.identity);

      if (!arcadeMode)
      {
        ++enemiesSpawnedInRoom;
        ++enemiesInRoom;
      }
    }

    waitingToSpawnEnemy = false;
  }

  private int GetAmountOfEnemiesToSpawn() => Random.Range(
        Mathf.FloorToInt(Mathf.Log(GameManager.instance.level + 2, 2)),
        Mathf.CeilToInt(Mathf.Log(GameManager.instance.level + 2, 2) * 2)
    );
}