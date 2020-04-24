﻿using UnityEngine;
using System;

public class RangedWeapon : Weapon
{

  /* Projectile */
  public Projectile projectilePrefab;
  [SerializeField] private float projectileForce = 20f;

  /* Fire Point */
  public Transform firePoint;

  /* Sound */
  [SerializeField] private bool silenced = false;
  [SerializeField] private float soundDistance = 30;

  /* Event */
  public static event Action<Vector2, float> OnShotFired;

  public override void Attack()
  {
    if (onCooldown) return;

    Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, transform.parent).GetComponent<Rigidbody2D>()?.AddForce(firePoint.right * projectileForce, ForceMode2D.Impulse);

    AudioManager.instance.PlayWithRandomPitch("Gunshot", 0.8f, 1.2f);
    GetComponent<Recoil>()?.AddRecoil();
    GetComponent<ScreenShake>()?.Shake();

    onCooldown = true;

    Invoke("ShotFired", 0.1f);
  }

  private void ShotFired()
  {
    if (!silenced && transform.parent.TryGetComponent<Player>(out var player))
      OnShotFired?.Invoke(player.transform.position, soundDistance);
  }
}