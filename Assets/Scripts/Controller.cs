using UnityEngine;
//Script de controle do Personagem e seus estados.
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Controller : MonoBehaviour
{

    [Header("Configurações")]

    //Variaveis, ver se preciso definir os boleanos das transições
    private Animator animator;
    private Rigidbody2D physics;
    private bool isGrounded;
    private bool isNearWall;
    private float defaultGravityScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
