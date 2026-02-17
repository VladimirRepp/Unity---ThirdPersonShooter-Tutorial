using UnityEngine;

public class HintInteractionItem : MonoBehaviour
{
    [Header("Hint Interaction Item Settings")]
    [SerializeField] protected float interactionRange = 2f;
    [SerializeField] protected GameObject hintInteractionUI;

    protected SphereCollider _interactionTrigger;

    private void Start()
    {
        // Отключаем UI взаимодействия
        if (hintInteractionUI != null)
            hintInteractionUI.SetActive(false);
        else
            Debug.LogError("Hint Interaction UI не назначен!");

        _interactionTrigger = GetComponent<SphereCollider>();
        if (_interactionTrigger != null)
            _interactionTrigger.radius = interactionRange;
        else
            Debug.LogError("Interaction Trigger не найден на объекте!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hintInteractionUI != null)
            {
                hintInteractionUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hintInteractionUI != null)
            {
                hintInteractionUI.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса взаимодействия
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void CloseHint()
    {
        if (hintInteractionUI != null)
        {
            hintInteractionUI.SetActive(false);
        }
    }
}
