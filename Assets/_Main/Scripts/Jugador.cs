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

    [Header("Variables de juego")]

    //Ajustar la vida inicial
    [Tooltip("Vida inicial del jugador")]
    public int VidaJugador = 100;

    //Poder de Ataque, se puede ajustar con Items recolectados
    [Tooltip("Poder Ataque")]
    public float PoderAtaqueJugador;

    //Variables privadas
    CharacterController player;
    Animator AnimacionJugador;
    private float VelocidadRotacion;
    private Vector3 Salto;

    //Vairables publicas pero ocultas en inspector, se usan para avisar estados del jugador
    [HideInInspector]
    public bool JugadorAtacando;
    [HideInInspector]
    public bool JugadorDefendiendo;
    [HideInInspector]
    public bool JugadorCorriendo;
    [HideInInspector]
    public bool JugadorDanado;
    [HideInInspector]
    public bool JugadorMuriendo = false;


    #endregion

    void Start()
    {
        //Se obtienen los componente del objeto
        player = GetComponent<CharacterController>();
        AnimacionJugador = GetComponent<Animator>();

        //Se activa el character controller ya que durante la muerte se desactivo
        player.enabled = true;
        JugadorMuriendo = false;
    }

    void Update()
    {
        //Metodo control movimiento, parametros son los inputs del teclado
        MovimientoJugador(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"),JugadorAtacando);

        //Metodo Saltos
        SaltoJugador();

        //Metodo Atacar condicion clic izquierdo presionado
        AtaqueJugador(Input.GetMouseButton(0));

        //Metodo Atacar condicion clic derechopresionado
        DefensaJugador(Input.GetMouseButton(1));
    }


    #region Control Movimiento Jugador

    public void MovimientoJugador(float horizontal, float vertical, bool Atacando)
    {
        //se obtiene el vector de direccion de los controles
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //si se estan presionando los controles aplicar el movimiento y el jugador no esta atacando o defendiendo
        if (direction.magnitude > 0f && !JugadorAtacando && !JugadorDefendiendo && !JugadorMuriendo)
        {
            //Se activan animacion corriendo
            AnimacionCorrer(true);

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

            //Se utiliza esta variable para evitar que el jugador ataque mientras corre
            JugadorCorriendo = true;
        }
        else
        {
            //Se desactiva animacion corriendo
            AnimacionCorrer(false);
            JugadorCorriendo = false;
        }

    }

    public void SaltoJugador()
    {

        if (player.isGrounded && Salto.y<0 && !JugadorMuriendo)
        {
            Salto.y = 0;
            AnimacionSalto(false);
        }
        //Si es presiona boton de salto y el jugador esta en el suelo y no esta atacando
        if (Input.GetButtonDown("Jump") && player.isGrounded && !JugadorAtacando)
        {
            Salto.y += Mathf.Sqrt(PoderSalto *-2 *Gravedad);
            AnimacionSalto(true);
        }
        //vector de salto es igual a la gravedad
        Salto.y += Gravedad * Time.deltaTime;
        player.Move(Salto * Time.deltaTime);
    }

    public void AtaqueJugador(bool ControlAtaque)
    {
        //Si jugador no esta corriendo ni defendiendo puede correr
        if(ControlAtaque && !JugadorCorriendo && !JugadorDefendiendo)
        {
            JugadorAtacando = true;
        }
        else
        {
            JugadorAtacando = false;
        }

        AnimacionAtacar(JugadorAtacando);

    }

    public void DefensaJugador(bool ControlDefensa)
    {
        //Si jugador no esta corriendo ni Atacando puede correr
        if (ControlDefensa && !JugadorCorriendo &&!JugadorAtacando)
        {
            JugadorDefendiendo = true;
        }
        else
        {
            JugadorDefendiendo = false;
        }

        AnimacionDefender(JugadorDefendiendo);

    }
    #endregion

    #region Control Vida Jugador

    //La vida puede incrementarse con items o disminuir con danos, por eso la funcion se le pasa parametro delta
    public void Vida(int DeltaVida)
    {
        //Si Deltavida es menor a cero el jugador esta recibiendo ataques si ademas no se esta en modo defensa y no se esta muriendo
        if (DeltaVida < 0 && !JugadorDefendiendo && !JugadorMuriendo)
        {
            //A vida se descuenta el dano
            VidaJugador += DeltaVida;

            JugadorDanado = true;

            //Se activa animacion de dano
            AnimacionDano(JugadorDanado);
        } else
        {
            JugadorDanado = false;

            //Se activa animacion de dano
            AnimacionDano(JugadorDanado);
        }


        //Si deltaVida es mayor a cero el jugador esta recibiendo un Item de salud
        if (DeltaVida > 0 && !JugadorDefendiendo && !JugadorMuriendo && !JugadorCorriendo && !JugadorAtacando)
        {
            //A vida se aumenta la salud
            VidaJugador += DeltaVida;

            //Se activa animacion de mas salud
            AnimacionSalud();
        }


        //Si delta vida es igual o menor a cero el jugador muere
        if (VidaJugador <= 0)
        {
            JugadorMuriendo = true;
            //Se realiza la animacion de la Muerte
            AnimacionMuerte();
            Invoke("Muerte", 5f);
        }

    }

    //Se quizo llamar con animation event pero existia un error al invocar (se puede mejorar esto con mas tiempo)
    public void Muerte()
    {

    }

    #endregion

    #region Animaciones
    public void AnimacionSalto(bool Saltando)
    {
        AnimacionJugador.SetBool("Salto", Saltando);
    }

    public void AnimacionCorrer(bool Corriendo)
    {
        AnimacionJugador.SetBool("Correr", Corriendo);
    }

    public void AnimacionAtacar(bool Atacando)
    {
        AnimacionJugador.SetBool("Atacar", Atacando);
    }

    public void AnimacionDefender(bool Defendiendo)
    {
        AnimacionJugador.SetBool("Defender", Defendiendo);
    }

    public void AnimacionDano(bool Dano)
    {
        AnimacionJugador.SetBool("Dano", Dano);
    }

    //La muerte se activa con trigger al ser un evento sin loop
    public void AnimacionMuerte()
    {
        //se desactiva el collider del character para que pueda caer al piso el jugador
        player.enabled = false;
        AnimacionJugador.SetTrigger("Muerte");
    }

    //Aumento a la salud se activa con un trigger
    public void AnimacionSalud()
    {
        AnimacionJugador.SetTrigger("Salud");
    }

    #endregion

    #region DeteccionColliders


    #endregion

}



