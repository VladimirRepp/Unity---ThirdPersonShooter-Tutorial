using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class DoorOpener : MonoBehaviour
{
    [Header("References Settings")]
    [SerializeField] private StarterAssetsInputs inputs;
    [SerializeField] private PlayerInventory inventorySystem;

    [Header("Interection Settings")]
    [SerializeField] private string triggerTag = "InerectiveDoor";

    private bool _canOpen = false;
    private OpenDoor _interactiveDoor;

    private void OnEnable()
    {
        inputs.OnPlayerAction += HandleOpen;
    }

    private void OnDisable()
    {
        inputs.OnPlayerAction -= HandleOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _interactiveDoor))
        {
            _canOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            _canOpen = false;
            _interactiveDoor = null;
        }
    }

    private void HandleOpen()
    {
        TryOpen();
    }

    private void TryOpen()
    {
        if (!_canOpen || _interactiveDoor == null)
        {
            //Debug.LogError("Нет активной двери для открытия!");
            return;
        }

        ItemSO keyItem = inventorySystem.GetUsableItemOfType(ItemType.Key);
        if (keyItem == null)
        {
            Debug.Log("У вас нет ключевого предмета для открытия двери.");
            return;
        }

        if (_interactiveDoor.TryOpen(keyItem))
        {
            inventorySystem.RemoveItem(keyItem);
        }
    }

}
