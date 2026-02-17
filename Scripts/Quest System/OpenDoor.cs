using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openAnimationTrigger = "Open";
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private HintInteractionItem hintInteractionItem;

    private bool _isOpen = false;

    public bool IsOpen => _isOpen;

    public bool TryOpen(ItemSO keyItem)
    {
        if (keyItem == null)
        {
            return false;
        }

        if (!keyItem.isUsable || keyItem.itemType != ItemType.Key)
        {
            return false;
        }

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(openAnimationTrigger);
        }

        if (doorOpenSound != null)
        {
            AudioSource.PlayClipAtPoint(doorOpenSound, transform.position);
        }

        if (hintInteractionItem != null)
        {
            hintInteractionItem.CloseHint();
            hintInteractionItem.enabled = false;
        }

        _isOpen = true;

        return true;
    }
}
