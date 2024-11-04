using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer model;
    [SerializeField]
    private CollectableDataSO collectableDataSO;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && model.enabled)
        {
            CollectableLogic();
            other.TryGetComponent(out PlayerMovement player);
            player.IncreaseSpeed(collectableDataSO.increasedSpeed, collectableDataSO.time);
        }
    }

    private void CollectableLogic()
    {
        model.enabled = false;
        Destroy(this, 3f);
    }
}
