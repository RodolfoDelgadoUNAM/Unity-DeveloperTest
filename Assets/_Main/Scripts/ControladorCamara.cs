using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCamara : MonoBehaviour
{
    #region Variables

    [Tooltip("arrastra aqui al objeto jugador")]
    public Transform PosicionJugador;

    [Tooltip("Suavidad del movimiento de la camara")]
    [Range(0.1f, 1.0f)]
    public float SuavidadMovimiento = 0.5f;

    [Tooltip("Velocidad del movimiento de la camara")]
    [Range(1.0f, 10.0f)]
    public float VelocidadRotacion = 5.0f;

    private Vector3 OffsetCamara;

    

 

    #endregion

    #region Metodos

    // Start is called before the first frame update
    void Start()
    {
        OffsetCamara = transform.position - PosicionJugador.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update
    void LateUpdate()
    {
        //Obtenermos el angulo de gito del mouse en el eje X
        Quaternion AnguloDeGiroCamaraX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * VelocidadRotacion, Vector3.up);
        
        //Actualizamos el offset de la camara
        OffsetCamara = AnguloDeGiroCamaraX * OffsetCamara;

        //La nueva posicion de la camara es la resultante de la posicion del jugador + el offset si el jugador se mueve la nueva pos se mantiene actualizada
        Vector3 NuevaPosicion = PosicionJugador.position + OffsetCamara;

        //la posicion es la interpolacion de el vector de la pos actual de la camara y la nueva posicion
        transform.position = Vector3.Slerp(transform.position, NuevaPosicion, SuavidadMovimiento);

        //hacemos que la camara siempre mire al jugador
        transform.LookAt(PosicionJugador);
    }

    #endregion
}
