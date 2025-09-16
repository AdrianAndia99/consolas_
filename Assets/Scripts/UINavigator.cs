using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UINavigator : MonoBehaviour
{
    public Button[] buttons; // Asigna los botones en el inspector
    private int currentIndex = 0;

    void Start()
    {
        if (buttons != null && buttons.Length > 0)
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
    }

    void Update()
    {
        if (Gamepad.current == null || buttons == null || buttons.Length == 0) return;

        if (Gamepad.current.dpad.down.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
        else if (Gamepad.current.dpad.up.wasPressedThisFrame)
        {
            currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
    }
}
