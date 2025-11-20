using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRFlipObject : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 0.5f;
    public bool flipped = false;

    private bool isFlipping = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    [Header("Target Position & Rotation (Local)")]
    public Vector3 localPosA = Vector3.zero;
    public Vector3 localPosB = new Vector3(0.4661576f, 0.4086528f, 0.00001392604f);

    private Quaternion rotA = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion rotB = Quaternion.Euler(0f, 0f, 180f);

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        // Si quieres que la posición inicial se tome del objeto al inicio
        localPosA = transform.localPosition;
    }

    void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isFlipping)
            StartCoroutine(FlipCoroutine());
    }

    IEnumerator FlipCoroutine()
    {
        if (isFlipping) yield break;

        isFlipping = true;

        // Desactivar grab interactable temporalmente
        if (grabInteractable != null)
            grabInteractable.enabled = false;

        Vector3 startPos = flipped ? localPosB : localPosA;
        Vector3 endPos = flipped ? localPosA : localPosB;

        Quaternion startRot = flipped ? rotB : rotA;
        Quaternion endRot = flipped ? rotA : rotB;

        float t = 0f;

        while (t < duration)
        {
            float f = t / duration;

            // Interpolar posición local y rotación
            transform.localPosition = Vector3.Lerp(startPos, endPos, f);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, f);

            t += Time.deltaTime;
            yield return null;
        }

        // Forzar valores exactos al final
        transform.localPosition = endPos;
        transform.localRotation = endRot;

        flipped = !flipped;

        // Reactivar grab interactable
        if (grabInteractable != null)
            grabInteractable.enabled = true;

        isFlipping = false;
    }
}
