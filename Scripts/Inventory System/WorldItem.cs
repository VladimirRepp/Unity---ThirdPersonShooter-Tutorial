using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemSO itemData;

    [Header("Effects Settings")]
    [SerializeField] private ParticleSystem pickupParticles;
    [SerializeField] private AudioClip pickupSound;

    public ItemSO ItemData => itemData;

    private void Start()
    {
        // Если префаб не назначен, используем текущий объект
        if (itemData != null && itemData.worldPrefab == null)
        {
            itemData.worldPrefab = gameObject;
        }
    }

    private void PlayPickupEffects()
    {
        if (pickupParticles != null)
        {
            Instantiate(pickupParticles, transform.position, Quaternion.identity);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }

    public void Pickup()
    {
        PlayPickupEffects();
        Destroy(gameObject);
    }

    public void Setup(ItemSO newItemData = null)
    {
        if (newItemData != null)
            itemData = newItemData;

        if (itemData != null && itemData.worldPrefab == null)
        {
            // Можно добавить логику изменения меша/материалов
        }
    }
}
