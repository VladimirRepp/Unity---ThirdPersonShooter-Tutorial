using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool isViewDebug = false;

    private List<ItemSO> items = new();

    public void AddItem(ItemSO item)
    {
        items.Add(item);

        if (isViewDebug)
            Debug.Log($"Предмет {item.itemName} добавлен в инвентарь.");
    }

    public void RemoveItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);

            if (isViewDebug)
                Debug.Log($"Предмет {item.itemName} удален из инвентаря.");
        }
        else
        {
            if (isViewDebug)
                Debug.LogWarning($"Попытка удалить предмет {item.itemName}, которого нет в инвентаре.");
        }
    }

    /// <summary>
    /// Получить первый доступный для использования предмет указанного типа
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public ItemSO GetUsableItemOfType(ItemType itemType)
    {
        foreach (var item in items)
        {
            if (item.itemType == itemType && item.isUsable)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Получить все доступные для использования предметы указанного типа
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public ItemSO[] GetUsanbleItemsOfType(ItemType itemType)
    {
        return null;
    }

    public void DropItem(ItemSO item, Vector3 position, Quaternion rotation)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            item.TryDrop(position, rotation);

            if (isViewDebug)
                Debug.Log($"Предмет {item.itemName} выброшен из инвентаря.");
        }
        else
        {
            if (isViewDebug)
                Debug.LogWarning($"Попытка выбросить предмет {item.itemName}, которого нет в инвентаре.");
        }
    }

    public void ShowInventoryDebug()
    {
        Debug.Log("Содержимое инвентаря:");
        foreach (var item in items)
        {
            Debug.Log($"- {item.itemName} (ID: {item.itemID})");
        }
    }
}
