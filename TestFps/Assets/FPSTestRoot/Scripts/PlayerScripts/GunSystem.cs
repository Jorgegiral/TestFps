using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    #region General Variables
    [Header("General References")]
    [SerializeField] Camera fpsCam; //Ref a la cámara desde la que se dispara
    [SerializeField] Transform shootPoint; //Ref a la posición de la cam para disparar desde ella
    [SerializeField] RaycastHit hit; //Almacén de información de impacto de los Raycast
    [SerializeField] LayerMask impactLayer; //Ref a la layer contra la que SÍ impacta el Raycast

    [Header("Weapon Parameters")]
    public int damage;
    public float range; //Distancia a la que llegan los disparos
    public float spread; //Pequeño rango de dispersión para que el disparo no sea "perfecto"
    public float shootingCooldown; //Tiempo de espera entre disparo y disparo
    public float timeBetweenShoots; //En modo ráfaga: intervalo entre disparos
    public float reloadTime; //Tiempo de recarga
    public bool allowButtonHold; //True = modo ráfaga, false = disparo input a input

    [Header("Bullet Management")]
    public int ammoSize; //Valor del cargador máximo del arma
    public int bulletsPerTap; //Cuantas balas se disparan de un solo disparo
    [SerializeField] int bulletsLeft; //Balas que quedan en el cargador
    [SerializeField] int bulletsShot; //Balas que ya han sido disparadas

    [Header("State Flags")]
    [SerializeField] bool shooting; //Estamos disparando
    [SerializeField] bool canShoot = true; //Podemos disparar
    [SerializeField] bool reloading; //Estamos recargando
    #endregion

    private void Awake()
    {
        bulletsLeft = ammoSize; //Al inicio de la escena, se recarga el arma al completo
    }

    // Update is called once per frame
    void Update()
    {
        InputFlags();
    }

    void InputFlags()
    {
        //Se dispara el Raycast si se cumplen las condiciones: podemos disparar y estamos disparando (input activo)
        if (canShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        canShoot = false; //Estamos en proceso de disparo, no debemos overlapear disparos
        Vector3 direction = fpsCam.transform.forward; //Dirección del raycast, hacia delante desde la cámara

        //RAYCAST DEL DISPARO
        //Physics.Raycast(Origen, Dirección, Almacén de información del rayo, longitud del rayo, a qué puede golpear el rayo)
        //Longitud infinita = Mathf.Infinity
        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range, impactLayer))
        {
            //Si este rayo impacta...
            if (hit.collider.CompareTag("Enemy"))
            {
                //APLICAREMOS EL DAÑO
                //Ejemplo de acceso directo
                EnemyHealth health = hit.collider.gameObject.GetComponent<EnemyHealth>();
                health.TakeDamage(damage); //Accedo a la acción de quitar vida del enemigo y le aplico el daño de mi arma
            }
        }

        bulletsLeft--; //Resta una bala al cargador
        bulletsShot++; //AUmenta en uno el número de balas fuera del cargador

        //Si no estamos en proceso de calcular la cadencia de disparo...
        //Ni podemos disparar...
        if(!IsInvoking(nameof(ResetShoot)) && !canShoot)
        {
            //Devuelve la posibilidad de disparar
            Invoke(nameof(ResetShoot), shootingCooldown);
        }

    }

    void ResetShoot()
    {
        canShoot = true; //nos permite volver a disparar el raycast
    }

    private void Reload()
    {
        if (bulletsLeft < ammoSize && !reloading)
        {
            reloading = true; //Empezamos a recargar
            Invoke(nameof(ReloadFinished), reloadTime); //Se espera a recargar tanto tiempo como dure la anim de recarga
        }
        else Debug.Log("Can't reload now!");
    }

    private void ReloadFinished()
    {
        bulletsLeft = ammoSize; //Se recupera el cargador entero
        bulletsShot = 0; //Se resetea el contador de balas disparadas por cargador
        reloading = false; //Dejamos de recargar
    }

    #region Input Methods
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            shooting = true;
        }
        if (context.canceled)
        {
            shooting = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) Reload();
    }
    #endregion
}
