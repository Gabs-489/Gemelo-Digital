using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ubiq.Rooms;
using System.Linq;


public class QuestStatusUI : MonoBehaviour
{
    [Header("Referencias UI (asignar en Inspector)")]
    [SerializeField] private TextMeshProUGUI connectionStatus;
    [SerializeField] private TextMeshProUGUI roomInfo;
    [SerializeField] private TextMeshProUGUI userCount;
    [SerializeField] private Image statusIndicator;

    [Header("Colores")]
    [SerializeField] private Color connectedColor = Color.green;
    [SerializeField] private Color disconnectedColor = Color.red;
    [SerializeField] private Color connectingColor = Color.yellow;

    private RoomClient roomClient;
    private float blinkTimer;

    void Start()
    {
        roomClient = FindObjectOfType<RoomClient>();

        if (roomClient != null)
        {
            roomClient.OnJoinedRoom.AddListener(OnRoomJoined);
            roomClient.OnPeerAdded.AddListener(OnPeerChanged);
            roomClient.OnPeerRemoved.AddListener(OnPeerChanged);
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();

        // Parpadeo cuando está conectando
        if (roomClient != null && !roomClient.JoinedRoom)
        {
            blinkTimer += Time.deltaTime * 2f;
            if (statusIndicator != null)
            {
                float alpha = (Mathf.Sin(blinkTimer) + 1f) * 0.5f;
                statusIndicator.color = Color.Lerp(disconnectedColor, connectingColor, alpha);
            }
        }
    }

    private void OnRoomJoined(IRoom room)
    {
        UpdateUI();
    }

    private void OnPeerChanged(IPeer peer)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (roomClient == null) return;

        if (roomClient.JoinedRoom)
        {
            // Conectado
            if (connectionStatus != null)
                connectionStatus.text = "CONECTADO";

            if (roomInfo != null)
            {
                // Mostrar nombre de sala y UUID (primeros 8 caracteres)
                string shortUUID = roomClient.Room.UUID.ToString().Substring(0, 8);
                roomInfo.text = $"Sala: {roomClient.Room.Name}\nID: {shortUUID}...";
            }

            if (userCount != null)
                userCount.text = $"Usuarios: {roomClient.Peers.Count()}";

            if (statusIndicator != null)
                statusIndicator.color = connectedColor;
        }
        else
        {
            // Desconectado/Conectando
            if (connectionStatus != null)
                connectionStatus.text = "CONECTANDO...";

            if (roomInfo != null)
                roomInfo.text = "Esperando conexión...";

            if (userCount != null)
                userCount.text = "Usuarios: 0";

            // El color de parpadeo se maneja en Update()
        }
    }
}