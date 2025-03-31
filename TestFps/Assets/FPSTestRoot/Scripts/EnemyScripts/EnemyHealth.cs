using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health System Configuration")]
    [SerializeField] int maxHealth; //Vida m�xima del enemigo
    [SerializeField] int currentHealth; //Vida actual del enemigo

    [Header("Feedback Configuration")]
    [SerializeField] Material baseMat; //Material base del enemigo (aspecto normal)
    [SerializeField] Material damagedMat; //Material feedback de recibir da�o
    [SerializeField] GameObject deathEffect; //VFX que se activa al morir

    //Autoreferencias privadas
    MeshRenderer enemyRend; //Ref a la mesh del enemigo, nos permite acceder a su material y modificarlo 

    private void Awake()
    {
        enemyRend = GetComponent<MeshRenderer>();
        baseMat = enemyRend.material; //Al inicio del juego se almacena el material base del enemigo
        currentHealth = maxHealth;
    }
    void Update()
    {
        //Coindici�n de muerte
        if (currentHealth <= 0)
        {
            currentHealth = 0; //La vida no puede bajar de cero
            deathEffect.SetActive(true); //Al morir se activa el VFX de muerte
            deathEffect.transform.position = transform.position; //Se mueve el VFX de muerte a la posici�n correcta
            gameObject.SetActive(false); //Se apaga el modelo visual del enemigo
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; //Se aplica el da�o recibido desde afuera
        enemyRend.material = damagedMat; //Se aplica el feedback visual del da�o
        Invoke(nameof(ResetDamageMaterial), 0.1f);
    }

    void ResetDamageMaterial()
    {
        //Devuelve al enemigo su aspecto original despues de ser da�ado
        enemyRend.material = baseMat;
    }
}
