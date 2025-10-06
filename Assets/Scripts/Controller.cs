using System.Collections;
using UnityEngine;
//Script de controle do Personagem e seus estados.
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Controller : MonoBehaviour
{
    [Header("Configurações do Personagem")]
    [SerializeField] float velocidadeX_run = 3.5f;
    [SerializeField] float velocidadeY_jump = 7f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float hp = 3f;
    [SerializeField] private bool _active = true;

    [Header("Verificar Chão")]
    [SerializeField] Transform checarChao;
    [SerializeField] float raycastDistance = 0.3f;
    [SerializeField] LayerMask groundCheck;

    [Header("Verificar Parede")]
    [SerializeField] Transform checkWall;
    [SerializeField] float wallDistanceCheck = 0.4f;
    [SerializeField] LayerMask wallClimbCheck;

    private Animator animator;
    private Rigidbody2D physics;
    private Collider2D _colider;
    private bool isGrounded, airborne, isNearWall, doubleJump = true; //checar se está no chão, ar, proximo à parede, ou se ja deu um pulo duplo
    private float defaultGravityScale;
    private Vector2 _respawn;

    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        defaultGravityScale = physics.gravityScale;
    }

    void Start()
    {
        _colider = GetComponent<Collider2D>();
        SetRespawnPoint(transform.position);
    }
    void Update()
    {
        if (!_active) { return; }
        PlayerInput(); //controles do jogador - Li que ao coloca-lós em fixed update, podem ocorrer delays
    }
    void FixedUpdate()
    {

        Movimentacao(); //lidar com movimentação
        UpdateAnimatorInfo(); //atualizar Parameters do animator
    }

    private void PlayerInput()
    {
        /*
        * Comandos:
            Barra de Espaço = Pular, apertar novamente para pulo duplo;
            Ataque com espada = Z;
            Disparo com pistola = X;
            Arremesso = C;
            Deslizar = V;
        */
        if (isGrounded) { doubleJump = true; }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("jump");
            physics.linearVelocity = new Vector2(physics.linearVelocity.x, velocidadeY_jump);
        }

        if (Input.GetKeyDown(KeyCode.Space) && airborne && doubleJump == true)
        {
            animator.SetTrigger("jump");
            physics.linearVelocity = new Vector2(physics.linearVelocity.x, velocidadeY_jump);
            doubleJump = false;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("slash");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("shoot");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetTrigger("throw");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("slide");
        }
    }

    private void Movimentacao()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (!animator.GetBool("estaEscalando"))
        {
            if (horizontalInput > 0f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (horizontalInput < 0f)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        Debug.DrawRay(checarChao.position, Vector3.down * raycastDistance, Color.green);
        isGrounded = Physics2D.Raycast(checarChao.position, Vector2.down, raycastDistance, groundCheck).collider != null;
        airborne = Physics2D.Raycast(checarChao.position, Vector2.down, raycastDistance, groundCheck).collider == null;

        isNearWall = Physics2D.Raycast(checkWall.position, transform.localScale, wallDistanceCheck, wallClimbCheck).collider != null;
        Debug.DrawRay(checkWall.position, transform.localScale * wallDistanceCheck, Color.green);

        if (isNearWall && Mathf.Abs(verticalInput) > 0.1f)
        {
            animator.SetBool("estaEscalando", true);
            physics.linearVelocity = new Vector2(physics.linearVelocity.x, verticalInput * climbSpeed);
            physics.gravityScale = 0f;
        }
        else
        {
            animator.SetBool("estaEscalando", false);
            float currentSpeed = velocidadeX_run;
            physics.gravityScale = defaultGravityScale;
            physics.linearVelocity = new Vector2(horizontalInput * currentSpeed, physics.linearVelocity.y);
        }
    }

    private void UpdateAnimatorInfo()
    {
        animator.SetFloat("velocidadeX", Mathf.Abs(physics.linearVelocity.x));
        animator.SetFloat("velocidadeY", physics.linearVelocity.y);
        animator.SetBool("estaNoChao", isGrounded);
        animator.SetBool("estaNoAr", airborne);
        animator.SetBool("doubleJump", doubleJump);
    }
    public void levarDano(float danoTomado)
    {
        animator.SetTrigger("hurt");
        hp -= danoTomado;
        if (hp <= 0)
        {
            Die();

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Hurt")
        {
            levarDano(1f);
        }
    }
    IEnumerator TimeAfterDeath()
    {
        yield return new WaitForSeconds(2f);
    }
    public void Die()
    {
        animator.SetTrigger("dead");
        StartCoroutine(TimeAfterDeath());
        _active = false; _colider.enabled = false;
        StartCoroutine(Respawning());
    }

    public void SetRespawnPoint(Vector2 position)
    {
        _respawn = position;
    }
    private IEnumerator Respawning(){
        yield return new WaitForSeconds(1f);
        transform.position = _respawn;
        _active = true;
        _colider.enabled = true;
        hp = 3;
    }
}
