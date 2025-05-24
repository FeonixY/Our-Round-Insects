using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float InteractionDistance = 2.0f;
    private Collider[] collidersBuffer = new Collider[10];

    private HashSet<InteractableObject> currentInteractables = new();

    private void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, InteractionDistance, collidersBuffer);
        if (numColliders == collidersBuffer.Length)
        {
            collidersBuffer = new Collider[collidersBuffer.Length * 2];
            numColliders = Physics.OverlapSphereNonAlloc(transform.position, InteractionDistance, collidersBuffer);
        }

        HashSet<InteractableObject> newInteractables = new();

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = collidersBuffer[i];
            if (collider.TryGetComponent(out InteractableObject interactableObject))
            {
                newInteractables.Add(interactableObject);
                if (!currentInteractables.Contains(interactableObject))
                {
                    interactableObject.ShowUI();
                }
            }
        }

        foreach (InteractableObject interactable in currentInteractables)
        {
            if (!newInteractables.Contains(interactable))
            {
                interactable.HideUI();
            }
        }

        currentInteractables = newInteractables;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }
}
