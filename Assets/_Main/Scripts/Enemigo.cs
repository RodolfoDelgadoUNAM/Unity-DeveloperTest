using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    #region Variables

    [Header("Variables de Fisica del Enemigo")]

    //variable para controlar la velocidad del jugador
    [Tooltip("Ajustar de acuerdo con animacion Walk o Run")]
    [Range(1f, 10f)]
    public float VelocidadEnemigo = 3f;

    [Header("Objetos de referencia")]

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto Jugador")]
    public Transform PosicionJugador;

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto con el punto A de vigilancia")]
    public Transform PuntoA;

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto con el punto B de vigilancia")]
    public Transform PuntoB;

    [Header("Variables de juego")]

    //Ajustar la vida inicial
    [Tooltip("Vida inicial del Enemigo")]
    public int VidaEnemigo = 100;

    //Poder de Ataque del enemigo
    [Tooltip("Poder Ataque")]
    public float PoderAtaqueEnemigo;

    //Poder de Ataque del enemigo
    [Tooltip("Ajustar distancia a la que el enemigo comienza a atacar")]
    [Range(0.5f, 5f)]
    public float SetDistanciaAtaque;

    //Variables privadas
    Rigidbody EnemigoController;
    Animator AnimacionEnemigo;
    private float VelocidadRotacion;
    private bool JugadorEnZona;
    private bool AtacandoJugador;
    private string Direccion;
    private Vector3 DireccionMovimiento;
    private Vector3 DireccionRotacion;
    private float DistanciaJugadorEnemigo;

    private float targetAngle;

    //Vairables publicas pero ocultas en inspector, se usan para avisar estados del Enemigo
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
        EnemigoController = GetComponent<Rigidbody>();
        AnimacionEnemigo = GetComponent<Animator>();

        //Se activa el character controller ya que durante la muerte se desactivo
        JugadorMuriendo = false;
        JugadorEnZona = false;
    }

    void Update()
    {
        //Metodo control movimiento, parametros son los inputs del teclado
        MovimientoEnemigo();
    }


    #region Control Movimiento Enemigo

    public void MovimientoEnemigo()
    {
        //Si el jugador esta en la zona de deteccion el enemigo lo sigue hasta estar cerca y atacarlo
        if (JugadorEnZona)
        {
            //Se obtiene la distancia entre el jugador y el enemigo
            DistanciaJugadorEnemigo = Vector3.Distance(transform.position, PosicionJugador.position);
            Debug.Log("Distancia a Jugador" + DistanciaJugadorEnemigo);
            // Sin importar si el jugador esta persiguiendo o atacando al enemigo, no deja de verlo
            //definiendo el vector para la rotacion del enemigo
            DireccionRotacion = PosicionJugador.position - transform.position;

            //se revisa si ya esta cerca para atacar
            if (DistanciaJugadorEnemigo <= SetDistanciaAtaque)
            {
                AtacandoJugador = true;
                AnimacionAtacar(AtacandoJugador);
            } 
            else
            {
                //Se mueve al enemigo en direccion del jugador
                DireccionMovimiento = PosicionJugador.position;

                AtacandoJugador = false;
                //Se desactiva animacion ataque
                AnimacionAtacar(AtacandoJugador);
            }
        }
        //Si sale de la zona de deteccion camina entre los dos puntos de vigilancia
        else
        {

            //
            if (Direccion == "PosteVigilanciaB")
            {
                //definiendo el vector para la rotacion del enemigo
                DireccionRotacion = PuntoA.position - transform.position;

                //Se mueve al enemigo en direccion del punto A
                DireccionMovimiento = PuntoA.position;
                
            } 
            else
            {
                //definiendo el vector para la rotacion del enemigo
                DireccionRotacion = PuntoB.position - transform.position;

                //Se mueve al enemigo en direccion del punto B
                DireccionMovimiento = PuntoB.position;
            }
        }


        //Se mueve al enemigo en direccion de postes o Jugador solo si no esta atacando
        if (!AtacandoJugador)
        {
            transform.position = Vector3.MoveTowards(transform.position, DireccionMovimiento, VelocidadEnemigo * Time.deltaTime);
        }
        //Se define la rotacion solo en Y
        float rotacionY = Mathf.Atan2(DireccionRotacion.x, DireccionRotacion.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, rotacionY, 0f);
    }

    public void AtaqueEnemigo()
    {
        AnimacionAtacar(true);
    }

    
    #endregion

    #region Control Vida Enemigo

    //La vida puede incrementarse con items o disminuir con danos, por eso la funcion se le pasa parametro delta
    public void Vida(int DeltaVida)
    {
        //Si Deltavida es menor a cero el jugador esta recibiendo ataques si ademas no se esta en modo defensa y no se esta muriendo
        if (DeltaVida < 0 && !JugadorDefendiendo && !JugadorMuriendo)
        {
            //A vida se descuenta el dano
            VidaEnemigo += DeltaVida;

            JugadorDanado = true;

            //Se activa animacion de dano
            AnimacionDano(JugadorDanado);
        }
        else
        {
            JugadorDanado = false;

            //Se activa animacion de dano
            AnimacionDano(JugadorDanado);
        }

        //Si delta vida es igual o menor a cero el jugador muere
        if (VidaEnemigo <= 0)
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

    public void AnimacionAtacar(bool Atacando)
    {
        AnimacionEnemigo.SetBool("Atacar", Atacando);
    }

    public void AnimacionDano(bool Dano)
    {
        AnimacionEnemigo.SetBool("Dano", Dano);
    }

    //La muerte se activa con trigger al ser un evento sin loop
    public void AnimacionMuerte()
    {

        AnimacionEnemigo.SetTrigger("Muerte");
    }

    #endregion

    #region DeteccionColliders


    private void OnTriggerEnter(Collider other)
    {
        //Detectar si Jugador esta dentro de la zona de vision del Enemigo
        if(other.gameObject.tag == "Jugador")
        {
            JugadorEnZona = true;
        }

        //Detecta si el jugador Colision con algun punto de vigilancia
        if (other.gameObject.tag == "PosteVigilanciaA")
        {
            Direccion = "PosteVigilanciaA";
        }

        //Detecta si el jugador Colision con algun punto de vigilancia
        if (other.gameObject.tag == "PosteVigilanciaB")
        {
            Direccion = "PosteVigilanciaB";
        }

    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    //Detectar si Jugador salio de la zona de vision del Enemigo
    private void OnTriggerExit(Collider other)
    {
        //Detectar si Jugador salio de la zona de vision del Enemigo
        if (other.gameObject.tag == "Jugador")
        {
            JugadorEnZona = false;
        }
    }

    #endregion

}
