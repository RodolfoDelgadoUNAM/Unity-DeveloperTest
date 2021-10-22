using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    #region Variables

    [Header("Variables de Fisica del jugador")]

    //variable para controlar la velocidad del jugador
    [Tooltip("Ajustar de acuerdo con animacion Walk o Run")]
    [Range(1f, 10f)]
    public float VelocidadJugador = 5f;

    //variable para controlar la gravedad y caida de jugador
    [Tooltip("Ajustar la gravedad aplicada solo al jugador")]
    [Range(0f, -9.81f)]
    public float Gravedad = -9.81f;

    [Tooltip("Ajustar el poder del salto")]
    [Range(0f, 5f)]
    public float PoderSalto = 1f;

    //variable para suavizado de rotacion de jugador
    [Tooltip("Mayo valor suaviza la velocidad de rotacion")]
    [Range(0.1f, 1.0f)]
    public float SuavizadoRotacion = 0.1f;

    [Header("Objetos de referencia")]

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto camara")]
    public Transform PosicionCamara;

    CharacterController player;
    float VelocidadRotacion;

    private Vector3 Salto;

    #endregion

    void Start()
    {
        player = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Metodo control movimiento, parametros son los inputs del teclado
        MovimientoJugador(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Metodo control gravedad
        SaltoJugador();

    }


    #region Control Movimiento Jugador

    public void MovimientoJugador(float horizontal, float vertical)
    {
        //se obtiene el vector de direccion de los controles
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        //si se estan presionando los controles aplicar el movimiento
        if (direction.magnitude > 0f)
        {


            //Angulo de rotacion del jugador de acuerdo con los controles y la camara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + PosicionCamara.eulerAngles.y;

            //aplicando suavizado a la rotacion del jugador
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref VelocidadRotacion, SuavizadoRotacion);

            //definiendo direccion a la que debe ir el jugador
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            //solo se rota en el eje Y de acuerdo al angulo definido con suavizado
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //mueve al jugador de acuerdo con la direccion y la velocidad
            player.Move(moveDir.normalized * VelocidadJugador * Time.deltaTime);
        }
    }

    public void SaltoJugador()
    {
        if (player.isGrounded && Salto.y < 0)
        {
            Salto.y = 0f;
        }
        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            Salto.y += Mathf.Sqrt(PoderSalto * -3.0f * Gravedad);
        }

        Salto.y += Gravedad * Time.deltaTime;
        player.Move(Salto * Time.deltaTime);
    }

    #endregion


}



