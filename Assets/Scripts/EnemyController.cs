using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    private bool chasing;
    public float distanceToChase = 10f, distanceToLose = 15f, distanceToStop;

    private Vector3 targetPoint, startPoint;

    public NavMeshAgent agent;

    public float keepChasingTime = 5f;
    private float chaseCounter;

    public GameObject bullet;
    public Transform firePoint;

    public float fireRate, waitBetweenShots = 2f, timeToShoot = 1f;
    private float fireCount, shotWaitCounter, ShootTimeCounter;

    public Animator anim;

    void Start()
    {
        startPoint = transform.position;

        ShootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {
        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if(!chasing)
        {
            if(Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;

                ShootTimeCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }
            if(chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;

                if(chaseCounter <= 0)
                {
                    agent.destination = startPoint;
                }
            }

            if(agent.remainingDistance < .25f)
            {
                anim.SetBool("isMoving", false);
            }else
            {
                anim.SetBool("isMoving", true);
            }
        }else
        {
            //transform.LookAt(targetPoint);

            //theRB.velocity = transform.forward * moveSpeed;
            agent.destination = targetPoint;

            if(Vector3.Distance(transform.position, targetPoint) > distanceToLose)
            {
                chasing = false;

                chaseCounter = keepChasingTime;
            }

            if(shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;

                if(shotWaitCounter <= 0)
                {
                    ShootTimeCounter = timeToShoot;
                }
                anim.SetBool("isMoving", true);
            }else
            {
                if(PlayerController.instance.gameObject.activeInHierarchy)
                {
                    ShootTimeCounter -= Time.deltaTime;

                    if(ShootTimeCounter > 0)
                    {
                        fireCount -= Time.deltaTime;

                        if(fireCount <= 0)
                        {
                            fireCount = fireRate;

                            firePoint.LookAt(PlayerController.instance.transform.position + new Vector3(0f, 1f, 0f));

                            //chequear angulo del jugador

                            Vector3 targetDir = PlayerController.instance.transform.position - transform.position;
                            float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

                            if(Mathf.Abs(angle) < 30f)
                            {
                                Instantiate(bullet, firePoint.position, firePoint.rotation);

                                anim.SetTrigger("fireShot");
                            }else
                            {
                                shotWaitCounter = waitBetweenShots;
                            }

                        
                        }

                        agent.destination = transform.position;

                    }else
                    {
                        shotWaitCounter = waitBetweenShots;
                    }
                }
                
                anim.SetBool("isMoving", false);
            }

            

           
        }
        
    }
}
