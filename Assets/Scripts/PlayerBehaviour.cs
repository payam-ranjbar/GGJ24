using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private RootIdentifier _rootIdentifier;
    [SerializeField] 
    private CharacterController _characterController;

    private void OnValidate()
    {
        if (_rootIdentifier == null)
        {
            _rootIdentifier = GetComponentInChildren<RootIdentifier>();
        }

        if (_rootIdentifier != null)
        {
            if (_characterController == null)
            {
                _characterController = _rootIdentifier.GetComponentInChildren<CharacterController>();
            }
        }
    }

    private void Awake()
    {
        OnValidate();
    }

    public void Update()
    {
        var vertical = Input.GetAxis("Vertical");
        var horizontal = -Input.GetAxis("Horizontal");

        _characterController.movementDirection = new Vector2(vertical, horizontal);

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            _characterController.Slap(true);
        }
    }
}
