using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 110f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Hearing Settings")]
    public float hearingRange = 12f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    private NavMeshAgent agent;
    private bool isDead = false;
    public float health = 100f;

    [Header("Drop Settings")]
    public Vector2Int bloodDropRange = new Vector2Int(5, 15);
    public Vector2Int pistolAmmoDropRange = new Vector2Int(2, 5);
    public Vector2Int rifleAmmoDropRange = new Vector2Int(5, 10);

    [Header("Sound setting")]
    [SerializeField] private AudioClip idle;
    [SerializeField] private AudioClip chase;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = Camera.main.transform; // Fallback if not manually set

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

       
        audioSource.PlayOneShot(idle);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = IsPlayerInView();
        bool canHearPlayer = IsPlayerMakingNoise(distanceToPlayer);

        if (canSeePlayer || canHearPlayer)
        {
            ChasePlayer();

            if (distanceToPlayer <= attackRange && Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            StopChase();
        }
    }

    void ChasePlayer()
    {
       
        audioSource.PlayOneShot(chase);
        agent.SetDestination(player.position);
        animator.SetBool("IsWalking", true);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void StopChase()
    {
        agent.ResetPath();
        animator.SetBool("IsWalking", false);
        animator.SetFloat("Speed", 0f);
    }

    void Attack()
    {
      
        audioSource.PlayOneShot(attack);
        int attackIndex = Random.Range(1, 3); // 1 or 2
        animator.SetTrigger("Attack" + attackIndex);

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Apply damage to player
       
        StartCoroutine(AttackDelay(attackCooldown));
    }

    IEnumerator AttackDelay(float amt)
    {
        yield return new WaitForSecondsRealtime(amt);
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                int randomDmg = Random.Range(2, 10);
                playerHealth.TakeDamage(randomDmg); // Adjust damage amount
            }
        }
    }

    bool IsPlayerInView()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > viewRadius) return false;

        float angleBetween = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleBetween < viewAngle / 2)
        {
            // Raycast to check if player is behind wall
            if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }
        return false;
    }

    bool IsPlayerMakingNoise(float distanceToPlayer)
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        return isSprinting && distanceToPlayer <= hearingRange;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;


        health -= damage;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        int blood = Random.Range(bloodDropRange.x, bloodDropRange.y + 1);
        int pistolAmmo = Random.Range(pistolAmmoDropRange.x, pistolAmmoDropRange.y + 1);
        int rifleAmmo = Random.Range(rifleAmmoDropRange.x, rifleAmmoDropRange.y + 1);

        // Send data to GameManager (which we’ll create next)
        GameManager.instance.CollectResources(blood, pistolAmmo, rifleAmmo);

        isDead = true;
        agent.isStopped = true;
        animator.SetTrigger("Die");
        Destroy(gameObject, 2.7f);
    }

    // Optional: visualize the vision cone
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Vector3 fovLine1 = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRadius;
        Vector3 fovLine2 = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
