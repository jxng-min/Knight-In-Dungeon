using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{ 
    private Rigidbody2D m_rigid;
    private SpriteRenderer m_sprite_renderer;

    public JoyStickCtrl joyStick { get; private set; }
    public Animator Animator { get; private set; }

    private float m_move_speed = 3f;

    void Start()
    {
        joyStick = GameObject.Find("TouchPanel").GetComponent<JoyStickCtrl>();
        m_rigid = GetComponent<Rigidbody2D>();
        m_sprite_renderer= GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        Vector2 input_vector = joyStick.GetInputVector();

        m_rigid.linearVelocity = new Vector2(input_vector.x * m_move_speed, input_vector.y * m_move_speed);
        if (input_vector.x > 0)
        {
            m_sprite_renderer.flipX = false;
        }
        else if (input_vector.x < 0)
        {
            m_sprite_renderer.flipX = true;

        }
        Animator.SetBool("IsMove", input_vector.sqrMagnitude > 0 );        
    }
}
