using UnityEngine;
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] float velocidadeX_correr = 3.5f;
    [SerializeField] float velocidadeY_pular = 7f;
    [SerializeField] float velocidadeEscalar = 3f;


    [Header("Verificar Chão")]
    [SerializeField] Transform checarChao;
    [SerializeField] float raycastDistance = 0.7f;
    [SerializeField] LayerMask groundCheck;

    [Header("Verificar Parede")]
    [SerializeField] Transform checkWall;
    [SerializeField] float wallDistanceCheck = 0.4f;
    [SerializeField] LayerMask wallClimbCheck;

    Animator animator;
    Rigidbody2D physics;
    SpriteRenderer sprite;

    enum State { Idle, Running, Airborne, Falling, Climbing }

    State currentState = State.Idle;
}
