using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private CharacterState state;
    private Camera camera;
    private Animator anim;
    private Rigidbody rb;
    private CapsuleCollider collider;

    public float moveSpeed = 5;
    public float rotateSpeed = 3f;

    public static readonly string horizontal = "Horizontal";
    public static readonly string vertical = "Vertical";

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    void Start()
    {
        state = GetComponent<CharacterState>();
        camera = GetComponentInChildren<Camera>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Horizontal = Input.GetAxis(horizontal);
        Vertical = Input.GetAxis(vertical);
        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f, Space.World);

        anim.SetBool("IsMoving", Horizontal != 0 || Vertical != 0);
        anim.SetFloat("MoveX", Horizontal);
        anim.SetFloat("MoveZ", Vertical);

    }

    private void FixedUpdate()
    {
        Vector3 direction = transform.right * Horizontal + transform.forward * Vertical;
        direction = Vector3.ClampMagnitude(direction, 1f);
        rb.linearVelocity = direction * moveSpeed;
    }
}
