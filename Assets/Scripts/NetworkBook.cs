using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class NetworkedGrabbableBook : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    private NetworkContext context;
    private Rigidbody rb;
    private bool isGrabbed = false;
    private bool isOwnedByMe = false;

    // Configuraci�n de sincronizaci�n
    [SerializeField] private float updateRate = 10f;
    private float nextUpdateTime;

    // Para interpolaci�n suave
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetVelocity;
    private Vector3 targetAngularVelocity;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    protected void Start()
    {
        // Ubiq registra el objeto autom�ticamente bas�ndose en su posici�n en la jerarqu�a
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        // Si soy el due�o, env�o actualizaciones
        if (isOwnedByMe && Time.time >= nextUpdateTime)
        {
            SendTransformUpdate();
            nextUpdateTime = Time.time + (1f / updateRate);
        }

        // Si NO soy el due�o, interpolo la posici�n recibida
        if (!isOwnedByMe && !isGrabbed)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            rb.velocity = targetVelocity;
            rb.angularVelocity = targetAngularVelocity;
        }
    }

    // Cuando alguien agarra el libro
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        isGrabbed = true;
        isOwnedByMe = true;

        // Notificar a otros usuarios que ahora yo controlo este libro
        SendOwnershipUpdate(true);
    }

    // Cuando se suelta el libro
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        isGrabbed = false;

        // Enviar actualizaci�n final con velocidad
        SendTransformUpdate();

        // Despu�s de un momento, liberar ownership
        Invoke(nameof(ReleaseOwnership), 0.5f);
    }

    private void ReleaseOwnership()
    {
        isOwnedByMe = false;
        SendOwnershipUpdate(false);
    }

    // Enviar actualizaci�n de transformaci�n
    private void SendTransformUpdate()
    {
        context.SendJson(new TransformMessage
        {
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity,
            angularVelocity = rb.angularVelocity,
            isGrabbed = isGrabbed
        });
    }

    // Enviar actualizaci�n de ownership
    private void SendOwnershipUpdate(bool owned)
    {
        context.SendJson(new OwnershipMessage
        {
            isOwned = owned
        });
    }

    // Recibir mensajes de la red - IMPORTANTE: este m�todo debe llamarse exactamente as�
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var transformMsg = message.FromJson<TransformMessage>();

        // Ignorar mis propios mensajes
        if (isOwnedByMe) return;

        targetPosition = transformMsg.position;
        targetRotation = transformMsg.rotation;
        targetVelocity = transformMsg.velocity;
        targetAngularVelocity = transformMsg.angularVelocity;

        // Si alguien m�s lo est� agarrando, deshabilitar f�sica local
        if (transformMsg.isGrabbed && !isGrabbed)
        {
            rb.isKinematic = true;
        }
        else if (!transformMsg.isGrabbed)
        {
            rb.isKinematic = false;
        }
    }

    // Estructuras de mensajes
    [System.Serializable]
    private struct TransformMessage
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public bool isGrabbed;
    }

    [System.Serializable]
    private struct OwnershipMessage
    {
        public bool isOwned;
    }
}