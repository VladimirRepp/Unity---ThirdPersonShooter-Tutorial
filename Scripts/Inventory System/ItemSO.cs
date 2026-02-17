using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Main Settings")]
    public string itemID; // Уникальный идентификатор
    public string itemName = "New Item";
    [TextArea(3, 5)]
    public string description = "Описание предмета";

    [Header("Visual Settings")]
    public Sprite icon;
    public GameObject worldPrefab; // Префаб для спавна в мире
    public Vector3 inventoryRotation = Vector3.zero; // Поворот в UI

    [Header("Characteristics")]
    public ItemType itemType = ItemType.Generic;
    public Rarity rarity = Rarity.Common;
    public int value = 0; // Стоимость

    [Header("Using")]
    public bool isUsable = false;
    public bool isConsumable = false;

    public virtual bool TryUse(GameObject user)
    {
        if (!isUsable)
        {
            Debug.Log($"Предмет не для использования: {itemName}");
            return false;
        }

        Debug.Log($"Используется предмет: {itemName}");
        return true;
    }

    public virtual bool TryDrop(Vector3 position, Quaternion rotation)
    {
        if (worldPrefab != null)
        {
            Instantiate(worldPrefab, position, rotation);
            return true;
        }

        return false;
    }
}

public enum ItemType
{
    Generic,
    Weapon,
    Consumable,
    Material,
    Quest,
    Key,
    Armor,
    Thing
}

public enum Rarity
{
    Common,     // Обычный
    Uncommon,   // Необычный
    Rare,       // Редкий
    Epic,       // Эпический
    Legendary   // Легендарный
}
