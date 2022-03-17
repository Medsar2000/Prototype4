using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public bool hasPowerup = false;
    public float powerupStrength = 10;
    public GameObject powerupIndicator;
    private GameObject focalPoint;
    private Rigidbody playerRb;

    public PowerUpTypeEnum currentPowerUp = PowerUpTypeEnum.None;
    public GameObject rocketPrefab; 
    private GameObject tmpRocket; 
    private Coroutine powerupCountdown;


    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing =false;
    float floorY;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        
    }

    // Update is called once per frame
    void Update()
    {
        float forwardinput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardinput * speed );
        powerupIndicator.transform.position = transform.position+ new Vector3(0,-.5f,0);

        if (currentPowerUp == PowerUpTypeEnum.Missile && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets(); 
        }

        if (currentPowerUp == PowerUpTypeEnum.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            Debug.Log("smashing");
            smashing = true;
            StartCoroutine(Smash());
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
           // StartCoroutine(PowerupCountDownRoutine());

            if (powerupCountdown != null) { StopCoroutine(powerupCountdown); }
            powerupCountdown = StartCoroutine(PowerupCountDownRoutine());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && hasPowerup && currentPowerUp == PowerUpTypeEnum.Pushback)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayfromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayfromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log(" collided with : " + collision.gameObject.name + " withpowerup set to " + currentPowerUp.ToString());
        }
    }
     IEnumerator PowerupCountDownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerup = false;
        currentPowerUp = PowerUpTypeEnum.None;
        powerupIndicator.gameObject.SetActive(false);
    }

    void LaunchRockets() 
    { 
        foreach (var enemy in FindObjectsOfType<Enemy>()) 
        { 
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform); 
        } 
    }

    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while(Time.time < jumpTime)
        {
            //move the player up while still keeping theirx velocity.
            playerRb.velocity =new Vector2(playerRb.velocity.x,smashSpeed);
            yield return null;
        }
        //Now move the player down
        while(transform.position.y > floorY)
        {
            playerRb.velocity =new Vector2(playerRb.velocity.x,-smashSpeed * 2);
            yield return null;
        }
        //Cycle through all enemies.
        for(int i = 0; i < enemies.Length; i++)
        {
            //Apply an explosion force that originatesfrom our position.
            if(enemies[i] !=null)
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce,transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
        //We are no longer smashing, so set the booleanto false
        smashing =false;
    }
    }
