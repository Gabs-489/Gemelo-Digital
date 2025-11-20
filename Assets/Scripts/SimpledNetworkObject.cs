using Ubiq.Messaging;
using UnityEngine;

[RequireComponent(typeof(NetworkId))]
public class SimpleNetworkedTransform : MonoBehaviour
{
    private NetworkContext context;
    private float updateInterval = 0.1f;
    private float lastUpdateTime;

    void Start()
    {
        context = NetworkScene.Register(this);
    }

    void Update()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            SendTransform();
            lastUpdateTime = Time.time;
        }
    }

    private void SendTransform()
    {
        context.SendJson(new TransformMessage
        {
            position = transform.position,
            rotation = transform.rotation
        });
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var data = message.FromJson<TransformMessage>();
        transform.position = data.position;
        transform.rotation = data.rotation;
    }

    [System.Serializable]
    struct TransformMessage
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}