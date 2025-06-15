using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static bool inputsEnabled = true;

    public static void EnableControls(bool enable)
    {
        inputsEnabled = enable;

        Cursor.visible = !enable;
        Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;

        if (enable) Input.ResetInputAxes();
    }
}