using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    [Header("Salto")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    [Header("Dash")]
    public float rollSpeed = 8f;
    public float rollHeight = 0.5f;
    public float rollDuration = 0.6f;
    public float rollCooldown = 1f;
    public float lastRollTime = -1f;
    [Header("WallRun")]
    public float wallRunSpeed = 8f;
    public float wallRunDuration = 2f;
    public float wallRunGravity = -2f;
    public float wallDetectionDistance = 1.5f;
    [Header("WallJump")]
    public float wallJumpForce = 8f;
}
