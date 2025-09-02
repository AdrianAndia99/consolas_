using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;
public class GamepadWaitScreen : MonoBehaviour
{
    public GameObject waitScreen;
    public TMP_Text messageText;
  
    void Update()
    {
        if (GamepadManager.Instance != null)
        {
            int connectedCount = GamepadManager.Instance.connectedGamepads.Count;

            if (connectedCount < 4)
            {
                waitScreen.SetActive(true);
                messageText.text = $"Esperando mandos... ({connectedCount}/4 conectados)";
            }
            else
            {
                waitScreen.SetActive(false);
            }
        }
    }
}