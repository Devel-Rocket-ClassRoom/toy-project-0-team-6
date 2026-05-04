using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public CharacterState state;
    private Animator anim;
    private Rigidbody rb;
    private CapsuleCollider collider;

    public float moveSpeed = 5;
    public float rotateSpeed = 3f;
    public  bool isAttacking;

    public static readonly string horizontal = "Horizontal";
    public static readonly string vertical = "Vertical";
    public static readonly int Attack = Animator.StringToHash("Attack");

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    void Start()
    {
        state = GetComponent<CharacterState>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();

        isAttacking = false;
    }

    private void Update()
    {
        Horizontal = Input.GetAxis(horizontal);
        Vertical = Input.GetAxis(vertical);
        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed, 0f, Space.World);

        anim.SetBool("IsMoving", Horizontal != 0 || Vertical != 0);
        anim.SetFloat("MoveX", Horizontal);
        anim.SetFloat("MoveZ", Vertical);

        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }

    private void FixedUpdate()
    {
        if (!isAttacking)
        {
            Vector3 direction = transform.right * Horizontal + transform.forward * Vertical;
            direction = Vector3.ClampMagnitude(direction, 1f);
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    private void OnAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        anim.SetTrigger(Attack);
        state.Attacking();
    }
    public void EndAttack()
    {
        isAttacking = false;
    }
}
