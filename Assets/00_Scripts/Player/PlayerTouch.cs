using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTouch : MonoBehaviour
{
    [SerializeField] private float _horizontalThresold;
    [SerializeField] private float _verticalThresold;


    private GameObject _levelHolder;
    private GridManager _gridManager;

    private InputAction _touchPress;   // dokunma var mı/yok mu (basılı/bırakıldı)
    private InputAction _touchPoint;   // dokunma pozisyonu

    private Vector2 _startPos;
    private bool _isTouching;

    void Awake()
    {
        _levelHolder = GameObject.FindGameObjectWithTag("Level");
        _gridManager = _levelHolder.GetComponent<GridManager>();

        _touchPress = InputSystem.actions.FindAction("Press");   // veya "TouchPress" gibi bir action ismi
        _touchPoint = InputSystem.actions.FindAction("Point"); // senin pozisyon action'ın

        _touchPress.Enable();
        _touchPoint.Enable();

        _touchPress.started += OnTouchStart;
        _touchPress.canceled += OnTouchEnd;
    }

    void OnTouchStart(InputAction.CallbackContext ctx)
    {
        _startPos = _touchPoint.ReadValue<Vector2>();
        _isTouching = true;
    }

    void OnTouchEnd(InputAction.CallbackContext ctx)
    {
        if (!_isTouching) return;
        _isTouching = false;

        Vector2 endPos = _touchPoint.ReadValue<Vector2>();
        Vector2 swipe = endPos - _startPos;

        DecideDirection(swipe);
    }

    void DecideDirection(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > _horizontalThresold) _gridManager.MoveGrid(GridManager.Direction.Right);
            else if (swipe.x < -_horizontalThresold) _gridManager.MoveGrid(GridManager.Direction.Left);
        }
        else
        {
            if (swipe.y > _verticalThresold) _gridManager.MoveGrid(GridManager.Direction.Up);
            else if (swipe.y < -_verticalThresold) _gridManager.MoveGrid(GridManager.Direction.Down);
        }
    }

    void OnDestroy()
    {
        _touchPress.started -= OnTouchStart;
        _touchPress.canceled -= OnTouchEnd;
    }
}
