using UnityEngine;
using Ubiq.Rooms;
using Ubiq.Messaging;

/// <summary>
/// Gestor simple de salas para Ubiq
/// Coloca este script en un GameObject de tu escena
/// </summary>
public class UbiqRoomManager : MonoBehaviour
{
    [Header("Configuración de Sala")]
    [SerializeField] private string defaultRoomName = "GemeloExposicion";
    [SerializeField] private bool autoJoin = true;

    private RoomClient roomClient;

    void Start()
    {
        roomClient = FindObjectOfType<RoomClient>();

        if (roomClient == null)
        {
            Debug.LogError(" No se encontró RoomClient!");
            return;
        }

        roomClient.OnJoinedRoom.AddListener(OnJoinedRoom);
        roomClient.OnPeerAdded.AddListener(OnPeerAdded);
        roomClient.OnPeerRemoved.AddListener(OnPeerRemoved);

        if (autoJoin)
        {
            JoinRoom(defaultRoomName);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (roomClient == null) return;
        Debug.Log($" Intentando unirse a sala: {roomName}");
        roomClient.Join(name: roomName,true);
    }

    private void OnJoinedRoom(IRoom room)
    {
        Debug.Log($" CONECTADO a sala: {room.Name}");
        Debug.Log($"   Mi UUID: {roomClient.Me.uuid}");
    }

    private void OnPeerAdded(IPeer peer)
    {
        Debug.Log($" Usuario CONECTADO");
    }

    private void OnPeerRemoved(IPeer peer)
    {
        Debug.Log($"Usuario DESCONECTADO");
    }
}