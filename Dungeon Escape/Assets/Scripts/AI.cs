using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{

    private Vector3 bugPosition;
    private float timeBug;
    private NavMeshAgent _agent;
    private LevelGeneration _lvlgen;
    private bool patrul;
    private bool moving;
    private int randomPosition;
    private float timeWait;
    private Animator anim;
    private PlayerController player;
    private EnemyVision vision;
    public bool Detect = false; 
    
    void Start()
    {
        anim = GetComponent<Animator>();
        _lvlgen = GameObject.FindWithTag("LevelGeneration").GetComponent<LevelGeneration>();
        _agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        vision = GetComponent<EnemyVision>();

        patrul = true;
        moving = false;
    }

    void Update()
    {
            Patrol();
            Follow();
            StopFollow();
    }

    void Patrol()
    {
        if (patrul)
        {
            if (moving) 
            {
                _agent.SetDestination(_lvlgen.wallPositions[randomPosition]);


                if (Vector3.Distance(transform.position, _lvlgen.wallPositions[randomPosition]) < 0.3f)
                {
                    anim.SetInteger("State", 0);
                    timeWait = Time.time;
                    moving = false;
                }

                if (Vector3.Distance(transform.position, bugPosition) < 0.4f && Time.time - timeBug > 1.5f)
                {
                    StartWalkToPostion();
                    timeBug = Time.time;
                }
            }
            else
            {
                if (Time.time - timeWait > 2.0f)
                {
                    StartWalkToPostion();
                    moving = true;
                    bugPosition = transform.position;
                    timeBug = Time.time;
                }
            }
        }
    }

    void Follow()
    {
        if (player.noiseSlider.value == 10 )
        {
            Detect = true;
            patrul = false;
            anim.SetInteger("State", 1);
            _agent.speed = 3;
            _agent.SetDestination(player.transform.position);

        }
        if (vision.vis == true)
        {
            Detect = true;
            patrul = false;
            anim.SetInteger("State", 1);
            _agent.speed = 3;
            _agent.SetDestination(player.transform.position);
        }
    }

    void StopFollow()
    {
        if (!player.enabled)
        {
            _agent.SetDestination(transform.position);
            anim.SetInteger("State", 0);
            Detect = false;
        }
    }

    void StartWalkToPostion()
    {
        randomPosition = Random.Range(0, _lvlgen.wallPositions.Count);
        _agent.SetDestination(_lvlgen.wallPositions[randomPosition]);
        anim.SetInteger("State", 1);
    }
}
