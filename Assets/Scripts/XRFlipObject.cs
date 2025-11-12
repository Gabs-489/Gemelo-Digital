using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class XRFlipObject : MonoBehaviour
{
    [Header("Flip Settings")]
    public float rotationDuration = 0.5f;
    public bool flipped = false;

    [Header("Interaction Type")]
    public bool flipOnGrab = true;        // flip when grabbed
    public bool flipOnActivate = false;   // flip when trigger pressed while holding

    private bool isRotating = false;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable component not found! Please add it to " + gameObject.name);
        }
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            if (flipOnGrab)
                grabInteractable.selectEntered.AddListener(OnSelectEntered);

            if (flipOnActivate)
                grabInteractable.activated.AddListener(OnActivated);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            if (flipOnGrab)
                grabInteractable.selectEntered.RemoveListener(OnSelectEntered);

            if (flipOnActivate)
                grabInteractable.activated.RemoveListener(OnActivated);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isRotating)
            StartCoroutine(Rotate180());
    }

    void OnActivated(ActivateEventArgs args)
    {
        if (!isRotating)
            StartCoroutine(Rotate180());
    }

    IEnumerator Rotate180()
    {
        isRotating = true;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 180, 0);
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
        flipped = !flipped;
        isRotating = false;
    }
}