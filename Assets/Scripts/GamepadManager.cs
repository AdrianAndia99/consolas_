using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GamepadManager : MonoBehaviour
{
    public static GamepadManager Instance;

    [SerializeField] public List<Gamepad> connectedGamepads = new List<Gamepad>();
    [SerializeField] public List<Gamepad> assignedGamepads = new List<Gamepad>();

    public System.Action onGamepadsUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
       
        var gamepads = Gamepad.all;
        bool changed = false;

   
        foreach (var gamepad in gamepads)
        {
            if (!connectedGamepads.Contains(gamepad))
            {
                connectedGamepads.Add(gamepad);
                changed = true;
                Debug.Log($"Gamepad conectado: {gamepad.name} (ID: {gamepad.deviceId})");
            }
        }

     
        for (int i = connectedGamepads.Count - 1; i >= 0; i--)
        {
            if (!connectedGamepads[i].added)
            {
                Debug.Log($"Gamepad desconectado: {connectedGamepads[i].name}");
                connectedGamepads.RemoveAt(i);
                changed = true;
            }
        }

        if (changed && onGamepadsUpdated != null)
        {
            onGamepadsUpdated.Invoke();
        }
    }

    public Gamepad GetGamepadForTankMovement(int tankIndex)
    {
        // Tanque 1: Mando 0, Tanque 2: Mando 2
        int gamepadIndex = tankIndex * 2;
        if (gamepadIndex < connectedGamepads.Count)
        {
            return connectedGamepads[gamepadIndex];
        }
        return null;
    }

    public Gamepad GetGamepadForTankTurret(int tankIndex)
    {
        // Tanque 1: Mando 1, Tanque 2: Mando 3
        int gamepadIndex = (tankIndex * 2) + 1;
        if (gamepadIndex < connectedGamepads.Count)
        {
            return connectedGamepads[gamepadIndex];
        }
        return null;
    }

    public bool HasEnoughGamepads()
    {
        return connectedGamepads.Count >= 4;
    }
}