using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private CharacterState state;
    private Animator anim;

    void Start()
    {
        state = GetComponent<CharacterState>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }
}
