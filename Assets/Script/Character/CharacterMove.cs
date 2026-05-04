using UnityEngine;
using static CharacterState;

public class CharacterMove : MonoBehaviour
{
    private CharacterState state;
    private Animator anim;
    private Rigidbody rb;
    private CapsuleCollider collider;

    public float moveSpeed = 5;
    public float rotateSpeed = 3f;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible= false;
    }

    private void Update()
    {
        Horizontal = Input.GetAxis(horizontal);
        Vertical = Input.GetAxis(vertical);
        transform.Rotate(0f, Input.GetAxis("Mouse X") * rotateSpeed * Time.timeScale, 0f, Space.World);

        if (state.currentState != StateType.Attack &&
            state.currentState != StateType.Dodge &&
            state.currentState != StateType.Damaged)
        {
            bool isMoving = Horizontal != 0 || Vertical != 0;

            state.currentState = isMoving ? StateType.Move : StateType.Idle;
        }

        if (state.currentState == StateType.Move)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("MoveX", Horizontal);
            anim.SetFloat("MoveZ", Vertical);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

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
        if (state.currentState == CharacterState.StateType.Attack ||
            state.currentState == CharacterState.StateType.Dodge ||
            state.currentState == CharacterState.StateType.Damaged)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 direction = transform.right * Horizontal + transform.forward * Vertical;
        direction = Vector3.ClampMagnitude(direction, 1f);
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnAttack()
    {
        if (state.currentState != StateType.Idle &&
            state.currentState != StateType.Move)
            return;

        state.currentState = CharacterState.StateType.Attack;

        anim.SetTrigger(Attack);
        state.Attacking();
    }
    public void EndAttack()
    {
        state.currentState = CharacterState.StateType.Idle;
    }
}
