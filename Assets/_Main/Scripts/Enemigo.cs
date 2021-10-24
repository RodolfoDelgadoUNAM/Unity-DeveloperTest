using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    #region Variables

    [Header("Variables de Fisica del Enemigo")]

    //variable para controlar la velocidad del jugador
    [Tooltip("Ajustar de acuerdo con animacion Walk o Run")]
    [Range(1f, 10f)]
    public float VelocidadEnemigo = 3f;

    [Header("Objetos de referencia")]

    //se requiere transform de jugador para mover enemigo y recibir dano
    [Tooltip("arrastra aqui el objeto Jugador")]
    public GameObject Jugador;

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto con el punto A de vigilancia")]
    public Transform PuntoA;

    //se requiere transform de camara para mover al jugador con ella
    [Tooltip("arrastra aqui el objeto con el punto B de vigilancia")]
    public Transform PuntoB;

    [Tooltip("Agrega aqui el efecto de dano")]
    public ParticleSystem EfectoSangre;

    [Tooltip("arrastra aqui el la barra de vida")]
    public Slider BarraVidaUI;

    //Ajustar la vida inicial
    [Tooltip("Vida inicial del Enemigo")]
    public int VidaInicialEnemigo = 100;

    //Poder de Ataque del enemigo
    [Tooltip("Ajustar distancia a la que el enemigo comienza a atacar")]
    [Range(0.5f, 5f)]
    public float SetDistanciaAtaque;

    //Variables privadas
    Animator AnimacionEnemigo;
    private bool JugadorEnZona;
    private bool AtacandoJugador;
    private string Direccion;
    private Vector3 DireccionMovimiento;
    private Vector3 DireccionRotacion;
    private float DistanciaJugadorEnemigo;
    private int VidaEnemigo;

    //Vairables publicas pero ocultas en inspector, se usan para ver el estado del Enemigo
    [HideInInspector]
    public bool JugadorAtacando;
    [HideInInspector]
    public bool EnemigoMuriendo = false;

    


    #endregion

    void Start()
    {
        //Se obtienen los componente del objeto
        AnimacionEnemigo = GetComponent<Animator>();

        //Se activa el character controller ya que durante la muerte se desactivo
        EnemigoMuriendo = false;
        JugadorEnZona = false;

        //se obtiene la vida inicial
        VidaEnemigo = VidaInicialEnemigo;
        BarraVidaUI.value = (float)(VidaEnemigo / (float)VidaInicialEnemigo);

    }

    void Update()
    {
        //El enemigo siempre se esta moviendo con excepcion de cuando ya esta muriendo
        if(!EnemigoMuriendo)
        {
            MovimientoEnemigo();
        }

        //Se ajusta el valor de la barra de vida del enemigo
        BarraVidaUI.value = ((float)VidaEnemigo / (float)VidaInicialEnemigo);
    }


    #region Control Movimiento Enemigo

    public void MovimientoEnemigo()
    {
        //Si el jugador esta en la zona de deteccion el enemigo lo sigue hasta estar cerca y atacarlo
        if (JugadorEnZona)
        {
            //Se obtiene la distancia entre el jugador y el enemigo
            DistanciaJugadorEnemigo = Vector3.Distance(transform.position, Jugador.transform.position);
            // Sin importar si el jugador esta persiguiendo o atacando al enemigo, no deja de verlo
            //definiendo el vector para la rotacion del enemigo
            DireccionRotacion = Jugador.transform.position - transform.position;

            //se revisa si ya esta cerca para atacar
            if (DistanciaJugadorEnemigo <= SetDistanciaAtaque)
            {
                AtacandoJugador = true;
                AnimacionAtacar(AtacandoJugador);
            } 
            else
            {
                //Se mueve al enemigo en direccion del jugador
                DireccionMovimiento = Jugador.transform.position;

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

    //Si se llama a la funcion es por que se estan recibiendo ataques, el enemigo no tiene la posibilidad de aumentar su vida
    public void Vida(int DeltaVida)
    {

        //A vida se descuenta el dano
        VidaEnemigo -= DeltaVida;
        //Se activa el efecto de sangre
        EfectoSangre.Play();

        if (VidaEnemigo <= 0)
        {
            EnemigoMuriendo = true;
            //Se desactivan los constraints del rigidbody
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //Se realiza la animacion de la Muerte
            AnimacionMuerte();
            //Se aumenta el contador de enemigos eliminados en el controlador del juego
            FindObjectOfType<ControlJuego>().EnemigosEliminados += 1;
            //Se destruye el cadaver despues de 20 segundos
            Destroy(gameObject, 20f);
            
        }

    }

    #endregion

    #region Animaciones

    public void AnimacionAtacar(bool Atacando)
    {
        AnimacionEnemigo.SetBool("Atacar", Atacando);
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
        //Detectar si Jugador esta dentro de la zona de vision del Enemigo
        if (other.gameObject.tag == "Jugador")
        {
            JugadorEnZona = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //Detectar si Jugador salio de la zona de vision del Enemigo
        if (other.gameObject.tag == "Jugador")
        {
            JugadorEnZona = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Se detecta si el enemigo esta siendo golpeado por la espada del jugador
        if(collision.gameObject.tag == "EspadaJugador")
        {
            //Se pasa el dano que produce el jugador para restar vida al enemigo solo si el jugador esta realmente atacando
            if(Jugador.GetComponent<Jugador>().JugadorAtacando && !EnemigoMuriendo)
            {
                Vida(Jugador.GetComponent<Jugador>().PoderAtaqueJugador);
            }
        }
    }

    #endregion

}
