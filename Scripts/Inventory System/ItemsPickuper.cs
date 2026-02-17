using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemsPickuper : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private bool autoPickup = false;
    [SerializeField] private string worldItemTag = "WorldItem";
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private StarterAssetsInputs inputs;

    private bool _canTryPickup = false;
    private WorldItem _pickupWorldItem = null;

    private void OnEnable()
    {
        inputs.OnPlayerAction += HandlePickup;
    }

    private void OnDisable()
    {
        inputs.OnPlayerAction -= HandlePickup;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _pickupWorldItem))
        {
            _canTryPickup = true;

            if (autoPickup)
                TryPickupItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(worldItemTag))
        {
            _canTryPickup = false;
            _pickupWorldItem = null;
        }
    }

    private void HandlePickup()
    {
        TryPickupItem();
    }

    private void TryPickupItem()
    {
        if (!_canTryPickup)
            return;

        if (_pickupWorldItem == null)
        {
            Debug.LogError("Нет активного объекта для добавления в инвентарь!");
            return;
        }

        playerInventory.AddItem(_pickupWorldItem.ItemData);
        _pickupWorldItem.Pickup();
        _canTryPickup = false;
    }
}
