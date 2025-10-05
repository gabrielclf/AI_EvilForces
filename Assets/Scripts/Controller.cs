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
    float max_hp;

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
    private bool isGrounded, airborne, isNearWall, doubleJump = true; //checar se está no chão, ar, proximo à parede, ou se ja deu um pulo duplo
    private float defaultGravityScale;

    void Awake()
    {
        max_hp = hp; //vida
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        defaultGravityScale = physics.gravityScale;
    }
    void Update(){
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
        } else {
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
            animator.SetTrigger("dead");
            StartCoroutine(PauseGameAfterDeath());
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Hurt"){
            levarDano(1f);
        }
    }
    IEnumerator PauseGameAfterDeath() {
     yield return new WaitForSeconds(2f);
     Time.timeScale = 0f; 
    }
}
