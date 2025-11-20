using UnityEngine;
using Ubiq.Rooms;

public class AutoJoinFixedRoom : MonoBehaviour
{
    [Header("Configuración de Sala")]
    [SerializeField] private string roomName = "GemeloExposicion";

    private static readonly System.Guid fixedRoomId =
        new System.Guid("a11b22c3-d44e-55f6-a777-b888c999d000");

    private RoomClient roomClient;

    void Start()
    {
        // Buscar el RoomClient en la escena
        roomClient = FindObjectOfType<RoomClient>();

        if (roomClient == null)
        {
            Debug.LogError("No se encontró RoomClient en la escena!");
            return;
        }

        // Conectarse automáticamente
        Debug.Log($"Conectando a sala: {roomName}");
        roomClient.Join(
            roomName, true
        );

        // Suscribirse al evento de conexión
        roomClient.OnJoinedRoom.AddListener(OnJoinedRoom);
    }

    private void OnJoinedRoom(IRoom room)
    {
        Debug.Log($"CONECTADO a sala: {room.Name}");
    }
}