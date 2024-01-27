using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Gameplay")]
    public float speed = 10.0f;
    public float maxHealth = 1.0f;
    public float rotationSpeed = 10.0f;

    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField]
    private Rigidbody _rigidbody;
    
    public Vector2 movementDirection = Vector2.zero;
    public bool applyMovement = true;
    public new Transform transform => _rigidbody.transform;
    public Rigidbody rigidbody => _rigidbody;

    private void OnValidate()
    {
        if (_rootIdentifier == null)
        {
            _rootIdentifier = GetComponentInChildren<RootIdentifier>();
        }

        if (_rootIdentifier != null)
        {
            if (_rigidbody == null)
            {
                _rigidbody = _rootIdentifier.GetComponentInChildren<Rigidbody>();
            }
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void Update()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (applyMovement == true)
        {
            var deltaTime = Time.fixedDeltaTime;
            //_rigidbody.rotation = Quaternion.RotateTowards(
            //    _rigidbody.rotation,
            //    Quaternion.LookRotation(new Vector3(movementDirection.x, 0.0f, movementDirection.y)),
            //    deltaTime * rotationSpeed
            //);
            _rigidbody.velocity = new Vector3(movementDirection.x, _rigidbody.velocity.y, movementDirection.y) * speed;
        }
    }

    public void TeleportPosition(Vector3 position)
    {
        var interpolation = _rigidbody.interpolation;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        _rigidbody.MovePosition(position);
        transform.position = position;
        _rigidbody.interpolation = interpolation;
    }

    private void TeleportRotation(Quaternion rotation)
    {
        var interpolation = _rigidbody.interpolation;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        _rigidbody.MoveRotation(rotation);
        transform.rotation = rotation;
        _rigidbody.interpolation = interpolation;
    }

}
