using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    void Start()
    {
        inputReader.MoveEvent += InputReader_MoveEvent;
    }

    private void InputReader_MoveEvent(Vector2 movement)
    {
        Debug.Log(movement);
    }


    private void OnDestroy()
    {
        inputReader.MoveEvent -= InputReader_MoveEvent;
    }
}
