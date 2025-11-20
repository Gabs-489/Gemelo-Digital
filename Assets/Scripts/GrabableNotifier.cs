using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Componente grabbable que notifica al NetworkedObjectSync cuando es agarrado/soltado.
/// Este script se coloca en el HIJO que quieres que sea agarrable.
/// </summary>
public class GrabbableNotifier : XRGrabInteractable
{
    [Tooltip("Referencia al objeto padre que tiene NetworkedObjectSync")]
    [SerializeField] private NetworkedObjectSync networkedObject;

    protected override void Awake()
    {
        base.Awake();

        // Si no se asignó manualmente, buscar en el padre
        if (networkedObject == null)
        {
            networkedObject = GetComponentInParent<NetworkedObjectSync>();
        }

        if (networkedObject == null)
        {
            Debug.LogError($"GrabbableNotifier en {gameObject.name} no puede encontrar NetworkedObjectSync en el padre!");
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Notificar al NetworkedObjectSync que fue agarrado
        if (networkedObject != null)
        {
            networkedObject.OnGrabbed();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Obtener velocidad para lanzar el objeto
        Vector3 velocity = Vector3.zero;
        Vector3 angularVelocity = Vector3.zero;

        // Obtener del Rigidbody del objeto (más confiable)
        Rigidbody rb = networkedObject?.GetComponent<Rigidbody>();
        if (rb != null)
        {
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
        }

        // Notificar al NetworkedObjectSync que fue soltado
        if (networkedObject != null)
        {
            networkedObject.OnReleased(velocity, angularVelocity);
        }
    }
}