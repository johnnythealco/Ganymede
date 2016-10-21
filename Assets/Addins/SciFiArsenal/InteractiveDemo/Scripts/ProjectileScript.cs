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
    Vector3 Miss { get; set; }
    float speed { get; set; }   
  
    bool Moving;
	
	void Start () 
	{
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;  
    }
  
    public void SetTarget(GameObject _Target, float _speed)
    {
        Target = _Target;
        speed = _speed;

        if (Target != null && speed != 0)
        {
            StartCoroutine(Hit_Target());
        }
    }

    public void MissTarget(Vector3 _Miss, float _speed)
    {
        Miss = _Miss;
        speed = _speed;


        if (speed != 0 && Target == null)
        {
            StartCoroutine(Miss_Target());
        }
        else
        {
            StartCoroutine(Hit_Target());
        }

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

    IEnumerator Hit_Target()
    {
        Moving = true;
        var destination = Target.transform.position;

        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude; //sqrMagnitude is cheaper on the CPU than Magnitude

        while (sqrRemainingDistance > float.Epsilon && Moving)
        {
            destination = Target.transform.position;
            transform.LookAt(destination);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
            yield return null;
        }

        Moving = false;
    }

    IEnumerator Miss_Target()
    {
        
        var destination = Miss;

        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude; //sqrMagnitude is cheaper on the CPU than Magnitude

        while (sqrRemainingDistance > float.Epsilon)
        {
            transform.LookAt(destination);
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
            yield return null;
        }
        Destroy(gameObject);
    }


}