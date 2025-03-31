using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Librer�a para usar clases de NavMesh

public class EnemyAIBasic : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] NavMeshAgent agent; //Ref al componente que permite que el objecto tenga IA
    [SerializeField] Transform target; //Ref al transform del objecto a perseguir
    [SerializeField] LayerMask targetLayer; //Determina cual es la capa de detecci�n del target
    [SerializeField] LayerMask GroundLayer; //Determina cual es la capa de detecci�n del suelo

    [Header("Patroling Stats")]
    public Vector3 walkPoint; //Direcci�n hacia la que se mover� la IA si no detecta al target
    [SerializeField] float walkPointRange; //Rango m�ximo de direcci�n a generar  
    bool walkPointSet; //Determina si la IA ha llegado al objetivo y entonces genera un nuevo objetivo

    [Header("Attack Configuration")]
    public float timeBetweenAttacks; //Tiempo de espera entre ataque y ataque(se suele igualar a la duraci�n de la animaci�n de ataque)
    bool alreadyAttacked; //Determina si ya se ha atacado (evita ataques infinitos segidos)
    //Variables que se usan si el ataque es a distancia
    [SerializeField] GameObject projectile; //Ref al prefab de la bala f�sica
    [SerializeField] Transform shootPoint; //Ref a la posici�n desde donde se genera la bala
    [SerializeField] float shootSpeedZ; //Velocidad de la bala hacia delante
    [SerializeField] float shootSpeedY; //Velocidad de la bala hacia arriba (solo si es catapulta con gravedad)

    [Header("States & Detection")]
    [SerializeField] float sightRange; //Distancia de detecci�n del target de la IA
    [SerializeField] float attackRange; //Distancia a partir de la cual la IA ataca
    [SerializeField] bool targetInSightRange; //Determina si el target est� a distancia de detecci�n
    [SerializeField] bool targetInAttackRange; //Determina si el target est� a distancia de ataque

    private void Awake()
    {
        target = GameObject.Find("Player").transform; //Al inicio del juego le dice que el target es el player
        agent = GetComponent<NavMeshAgent>(); //Al inicio del juego se autoreferencia el componente agente
    }
    void Update()
    {
        
    }

    void EnemyStateUpdater()
    {
        //Chequear si el target esta en los rangos de detecci�n y/o ataque
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, targetLayer);
        targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, targetLayer);

        //Cambios din�micos de estado de la IA
        if (!targetInSightRange && !targetInAttackRange) Patroling(); 
        //Si detecta al target pero no esta en rango de ataque; PERSIGUE
        if (targetInSightRange && !targetInAttackRange) ChaseTarget();
        //Si detecta al target y est� en rango de ataque: ATACA 
        if (targetInSightRange && targetInAttackRange) AttackTarget();
    }

    void Patroling()
    {

    }

    void SearchWalkPoint()
    {

    }

    void ChaseTarget()
    {

    }

    void AttackTarget()
    {

    }

    void ResetAttack()
    {

    }

    //Funci�n para que los gizmos de detecci�n (perseguir/ataque) se dibujen en escena al seleccionar el objeto
    private void OnDrawGizmosSelected()
    {
        //Dibuja una esfera de color rojo que define el rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        //Dibuja una esfera de color amarillo que define el rango de persecuci�n
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
