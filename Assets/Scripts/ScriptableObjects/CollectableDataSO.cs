using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableData", menuName = "ScriptableObjects/CollectableData")]

public class CollectableDataSO : ScriptableObject
{
    [SerializeField]
    [Header("Collectable Settings")]
    public float increasedSpeed = 300f;
    [SerializeField]
    public float time = 3f;
}