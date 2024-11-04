using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehavior : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer model;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entro en contacto");
        if (other.gameObject.layer == 7 && model.enabled)
        {
            CollectableLogic();
            Debug.Log("Entro en contacto layer");
        }
    }

    private void CollectableLogic()
    {
        model.enabled = false;
        Destroy(this, 3f);
    }
}
