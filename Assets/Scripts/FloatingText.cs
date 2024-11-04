using UnityEngine;

[SelectionBase]
public class FloatingText : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool allowRotation = false;
    [SerializeField] private float renderDistance = 12f;
    [Header("References")]
    [SerializeField] private TextMesh textMesh;

    private Transform mainCamera;
    private void Start()
    {
        mainCamera = Camera.main.transform;
    }
    private void Update()
    {
        if (allowRotation)
        {
            Billboarding();
        }
        if (Vector3.Distance(transform.position, mainCamera.transform.position) < renderDistance && !ViewingFromBehind())
        {
            textMesh.gameObject.SetActive(true);
        }
        else
        {
            textMesh.gameObject.SetActive(false);
        }
    }
    private bool ViewingFromBehind()
    {
        float dotProduct = Vector3.Dot(transform.forward, Camera.main.transform.forward);
        if (dotProduct > 0.3)
        {
            return false;
        }
        return true;
    }
    private void Billboarding()
    {
        Vector3 rotation = Quaternion.LookRotation(transform.position - mainCamera.position).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

}
