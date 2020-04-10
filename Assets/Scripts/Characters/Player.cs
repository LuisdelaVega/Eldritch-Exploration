﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingCharacter
{
  /* Private variables */
  private List<Follower> followers = new List<Follower>();

  /* Weapons */
  public List<Weapon> weapons;
  private Weapon activeWeapon;

  /* Player controls */
  private Controls controls;

  void Awake()
  {
    controls = new Controls();
    Vector3 weaponPosition = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
    activeWeapon = Instantiate(weapons[0], weaponPosition, Quaternion.identity, transform);
  }

  void OnEnable()
  {
    controls.Enable();

    controls.Weapon.Attack.performed += ctx =>
    {
      Attack();
      holdAttack = ctx.ReadValue<float>() >= 0.9f;
    };
    controls.Player.Movement.performed += ctx => ChangeDirection(ctx.ReadValue<Vector2>());
  }

  void OnDisable() => controls.Disable();

  private void ChangeDirection(Vector2 newDirection) => Direction = newDirection;

  void OnTriggerEnter2D(Collider2D other)
  {
    switch (other.tag)
    {
      case "Border":
        // GameManager.instance.GameOver();
        break;
    }
  }

  protected override void Move() => rb.MovePosition(rb.position + Direction * moveSpeed * Time.fixedDeltaTime);

  protected override void Attack() => activeWeapon.Attack();

  public override void RecruitFollower(Follower newFollower)
  {
    if (followers.Count == 0)
      follower = newFollower;

    followers.Insert(followers.Count, newFollower);
  }

  private void OnDestroy()
  {
    // We probably want to do some death animation here, if the object hasn't been removed already
    Destroy(activeWeapon);
  }

  // TODO: Remove everything below
  /* Getters and Setters */
  public int GetFollowerCount() => followers.Count;
  public Follower GetLastFollower() => followers.Count != 0 ? followers[followers.Count - 1] : null;
}
