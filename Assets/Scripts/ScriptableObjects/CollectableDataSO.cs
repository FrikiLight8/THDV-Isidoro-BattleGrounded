using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]

public class CollectableDataSO : ScriptableObject
{
    [SerializeField]
    [Header("Collectable Settings")]
    public float increasedSpeed = 300f;
    [SerializeField]
    public float time = 3f;
}