using Ubiq.Messaging;
using UnityEngine;

/// <summary>
/// Sincroniza la posición y rotación de un objeto en red.
/// Este script se coloca en el PADRE que contiene todos los hijos.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class NetworkedObjectSync : MonoBehaviour
{
    private NetworkContext context;
    private Rigidbody rb;
    private bool isOwnedByMe = false;

    // Configuración de sincronización
    [SerializeField] private float updateRate = 10f;
    private float nextUpdateTime;

    // Para interpolación suave
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetVelocity;
    private Vector3 targetAngularVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Start()
    {
        // Registrar con Ubiq
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        // Si soy el dueño, envío actualizaciones
        if (isOwnedByMe && Time.time >= nextUpdateTime)
        {
            SendTransformUpdate();
            nextUpdateTime = Time.time + (1f / updateRate);
        }

        // Si NO soy el dueño, interpolo la posición recibida
        if (!isOwnedByMe)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            rb.velocity = targetVelocity;
            rb.angularVelocity = targetAngularVelocity;
        }
    }

    /// <summary>
    /// Llamar cuando este objeto es agarrado
    /// </summary>
    public void OnGrabbed()
    {
        isOwnedByMe = true;
        rb.isKinematic = true;
        SendOwnershipUpdate(true);
    }

    /// <summary>
    /// Llamar cuando este objeto es soltado
    /// </summary>
    public void OnReleased(Vector3 velocity, Vector3 angularVelocity)
    {
        rb.isKinematic = false;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;

        // Enviar actualización final
        SendTransformUpdate();

        // Liberar ownership después de un momento
        Invoke(nameof(ReleaseOwnership), 0.5f);
    }

    private void ReleaseOwnership()
    {
        isOwnedByMe = false;
        SendOwnershipUpdate(false);
    }

    private void SendTransformUpdate()
    {
        context.SendJson(new TransformMessage
        {
            position = transform.position,
            rotation = transform.rotation,
            velocity = rb.velocity,
            angularVelocity = rb.angularVelocity,
            isGrabbed = isOwnedByMe
        });
    }

    private void SendOwnershipUpdate(bool owned)
    {
        context.SendJson(new OwnershipMessage
        {
            isOwned = owned
        });
    }

    // Recibir mensajes de la red
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var transformMsg = message.FromJson<TransformMessage>();

        // Ignorar mis propios mensajes
        if (isOwnedByMe) return;

        targetPosition = transformMsg.position;
        targetRotation = transformMsg.rotation;
        targetVelocity = transformMsg.velocity;
        targetAngularVelocity = transformMsg.angularVelocity;

        // Si alguien más lo está agarrando, deshabilitar física local
        if (transformMsg.isGrabbed)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }
    }

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