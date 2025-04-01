using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Librería para usar clases de NavMesh

public class EnemyAIBasic : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] NavMeshAgent agent; //Ref al componente que permite que el objecto tenga IA
    [SerializeField] Transform target; //Ref al transform del objecto a perseguir
    [SerializeField] LayerMask targetLayer; //Determina cual es la capa de detección del target
    [SerializeField] LayerMask GroundLayer; //Determina cual es la capa de detección del suelo

    [Header("Patroling Stats")]
    public Vector3 walkPoint; //Dirección hacia la que se moverà la IA si no detecta al target
    [SerializeField] float walkPointRange; //Rango máximo de dirección a generar  
    bool walkPointSet; //Determina si la IA ha llegado al objetivo y entonces genera un nuevo objetivo

    [Header("Attack Configuration")]
    public float timeBetweenAttacks; //Tiempo de espera entre ataque y ataque(se suele igualar a la duración de la animación de ataque)
    bool alreadyAttacked; //Determina si ya se ha atacado (evita ataques infinitos segidos)
    //Variables que se usan si el ataque es a distancia
    [SerializeField] GameObject projectile; //Ref al prefab de la bala física
    [SerializeField] Transform shootPoint; //Ref a la posición desde donde se genera la bala
    [SerializeField] float shootSpeedZ; //Velocidad de la bala hacia delante
    [SerializeField] float shootSpeedY; //Velocidad de la bala hacia arriba (solo si es catapulta con gravedad)

    [Header("States & Detection")]
    [SerializeField] float sightRange; //Distancia de detección del target de la IA
    [SerializeField] float attackRange; //Distancia a partir de la cual la IA ataca
    [SerializeField] bool targetInSightRange; //Determina si el target está a distancia de detección
    [SerializeField] bool targetInAttackRange; //Determina si el target está a distancia de ataque

    private void Awake()
    {
        target = GameObject.Find("Player").transform; //Al inicio del juego le dice que el target es el player
        agent = GetComponent<NavMeshAgent>(); //Al inicio del juego se autoreferencia el componente agente
    }
    void Update()
    {
        EnemyStateUpdater();
    }

    void EnemyStateUpdater()
    {
        //Chequear si el target esta en los rangos de detección y/o ataque
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, targetLayer);
        targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, targetLayer);

        //Cambios dinámicos de estado de la IA
        if (!targetInSightRange && !targetInAttackRange) Patroling(); 
        //Si detecta al target pero no esta en rango de ataque; PERSIGUE
        if (targetInSightRange && !targetInAttackRange) ChaseTarget();
        //Si detecta al target y está en rango de ataque: ATACA 
        if (targetInSightRange && targetInAttackRange) AttackTarget();
    }

    void Patroling()
    {
        //Sistema de patrullaje
        if(!walkPointSet)
        {
            //si no hay punto al que dirigirse, se genera un nuevo punto
            SearchWalkPoint();
        }
        else
        {
            //si el punto generado es caminable el agente lo perseguirá
            agent.SetDestination(walkPoint);
        }

        //Sistema para que el agente que busque un nuevo destino en caso de que haya llegado al punto actual
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1) walkPointSet = false;

    }

    void SearchWalkPoint()
    {
        //Este método es un sistema de generación de puntos a perseguir por el agente

        //Sistema de generación de puntos a patrullar random
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        //Determinamos el nuevo punto random a perseguir 
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        //Detección: si no hay suelo debajo, para evitar bucles infinitos
        if (Physics.Raycast(walkPoint, -transform.up, 2f, GroundLayer))
        {
            walkPointSet = true; //confirmamos que el punto es caminable, por lo que empezará el movimiento
        }
    }

    void ChaseTarget()
    {
        //Una vez detecta al target, el agente lo persigue
        agent.SetDestination(target.position);
    }

    void AttackTarget()
    {
        //Cuando se comienza a atacar, el agente se queda quieto (se persigue a sí mismo)
        agent.SetDestination(transform.position);
        //El agente siempre observa directamente al target
        transform.LookAt(target);

        if (!alreadyAttacked)
        {
            //Si no estamos atacando, se comienza a atacar
            //Aquì iria el código de ataque a personalizar

            //En este ejemplo, vamos a generar una bala, referencia su rigibody y empujarla por fuerzas
            Rigidbody rb = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * shootSpeedZ, ForceMode.Impulse);
            //Si es modo catapulta, se añade la siguiente linea
            //rb.AddForce(transform.up * shootSpeedY, ForceMode.Impulse);

            //Se termina el ataque, empieza el cooldown de intervalo de ataque
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); //Vuelve a atacar en el intervalo de tiempo indicado, se suele timear con la animación de ataque

        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //Función para que los gizmos de detección (perseguir/ataque) se dibujen en escena al seleccionar el objeto
    private void OnDrawGizmosSelected()
    {
        //Dibuja una esfera de color rojo que define el rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        //Dibuja una esfera de color amarillo que define el rango de persecución
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
