using UnityEngine;

/// <summary>
///     This script is writed in 5 minutes only for the showcase and contains REALLY BAD practices in terms of coding.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [SerializeField]
    private float movementForce;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private Color jumpColor;

    private Color _defaultColor;
    private bool _jumpFlag;

    private float _jumpTimer;
    private Rigidbody _rb;

    private RaymarchShape _rmShape;

    private Vector2 _userInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rmShape = GetComponent<RaymarchShape>();
    }

    private void Start()
    {
        _defaultColor = _rmShape.colour;
    }

    private void Update()
    {
        _userInput.x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && !_jumpFlag)
        {
            _jumpFlag = true;
            _jumpTimer = 1f;
        }

        _rmShape.colour = Color.Lerp(_defaultColor, jumpColor, _jumpTimer);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (_jumpFlag)
        {
            _rb.AddForce(Vector3.up * jumpForce);
            _jumpFlag = false;
        }

        _jumpTimer -= Time.fixedDeltaTime;

        if (Mathf.Abs(_userInput.x) > 0f) _rb.velocity += Vector3.right * (_userInput.x * movementForce * Time.fixedDeltaTime);
    }
}