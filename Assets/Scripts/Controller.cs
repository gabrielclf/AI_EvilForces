using UnityEngine;
//Script de controle do Personagem e seus estados.
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Controller : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float velocidadeX_run = 3.5f;
    [SerializeField] float velocidadeY_jump = 7f;


    [Header("Verificar Chão")]
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask groundCheck;

    [Header("Verificar Parede")]
    [SerializeField] Transform checkWall;
    [SerializeField] float wallDistanceCheck = 0.4f;
    [SerializeField] LayerMask wallClimbCheck;

    //Variaveis, ver se preciso definir os boleanos das transições
    private Animator animator;
    private Rigidbody2D physics;
    private bool isGrounded, airborne, isNearWall, doubleJump = true; //checar se está no chão, ar, proximo à parede, ou se ja deu um pulo duplo
    private float defaultGravityScale;

    void Awake()
    {
        animator = GetComponent<Animator>();
        physics = GetComponent<Rigidbody2D>();
        defaultGravityScale = physics.gravityScale;
    }

    void Update()
    {
        PlayerInput(); //controlar comandos
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
            Barra de Espaço = Pular
            Ataque com espada = Z
            Disparo com pistola = X
            Arremesso = C
            Deslizar = V
        */
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("jump");
            physics.linearVelocity = new Vector2(physics.linearVelocity.x, velocidadeY_jump);
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

    private void Movimentacao(){
        
    }

    private void UpdateAnimatorInfo()
    {
        animator.SetFloat("velocidadeX", Mathf.Abs(physics.linearVelocity.x));
        animator.SetFloat("velocidadeY", physics.linearVelocity.y);
        animator.SetBool("estaNoChao", isGrounded);
        animator.SetBool("estaNoAr", airborne);
        animator.SetBool("doubleJump", doubleJump);
    }
}
