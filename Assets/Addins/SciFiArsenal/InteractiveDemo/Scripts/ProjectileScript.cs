using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour 
{
    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject[] trailParticles;
    [HideInInspector]
    public Vector3 impactNormal; //Used to rotate impactparticle.
    GameObject Target { get; set; }
    float speed { get; set; }   
    float MaxLifeTime { get; set; }
	
	void Start () 
	{
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
        MaxLifeTime = 10f;
       
        Destroy(gameObject, MaxLifeTime);
	}

    void FixedUpdate()
    {
        if(Target != null && speed != 0)
        {
            
            var rb = gameObject.GetComponent<Rigidbody>();
            if (rb.velocity.sqrMagnitude < 1f)
            {
                transform.LookAt(Target.transform.position);
                rb.AddForce(transform.forward * speed);
            }
            var sqr_distance = (transform.position - Target.transform.position).sqrMagnitude;
            if (sqr_distance > 3600)
            {
                transform.LookAt(Target.transform.position);
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.forward * speed/2);
            }       
        }
    }

    public void SetTarget(GameObject _Target, float _speed)
    {
        Target = _Target;
        speed = _speed;
    }

	void OnCollisionEnter (Collision hit) {

        if (hit.collider.gameObject.layer != 10)
            return;

        impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;

        //Debug.Log("Collider hit : " + hit.collider.gameObject.name);

        if (hit.gameObject.tag == "Destructible") // Projectile will destroy objects tagged as Destructible
        {
            Destroy(hit.gameObject);
        }


        if (trailParticles != null)
        {
           foreach (GameObject trail in trailParticles)
            {
                GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }
        }
        Destroy(projectileParticle, 3f);
        Destroy(impactParticle, 1f);
        Destroy(gameObject);
	}

   
}