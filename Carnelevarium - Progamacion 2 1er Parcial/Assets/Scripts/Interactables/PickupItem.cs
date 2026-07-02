using UnityEngine;

public class PickupItem : MonoBehaviour, Interactables
{
    [Header("Prefab para inspección")]
    public GameObject itemPrefab;

    [Header("Identificador único para SaveSystem")]
    public string objectID;   // ? este campo faltaba

    public void Interact()
    {
        Debug.Log("Interact ejecutado en " + gameObject.name);
        InspectItem.Instance.Inspect(itemPrefab);
    }

}
