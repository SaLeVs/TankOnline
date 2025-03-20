using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;


[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{

    private Controls controls;

    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        
    }
}
