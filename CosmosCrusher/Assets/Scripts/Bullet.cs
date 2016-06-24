using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    public int tier;
    public bool isEnemy;
    public bool isBoss;
    public bool isPulse;
    public bool isFlood;
    public GameObject explosion;
    public int bulletFired;
    public int color;
    public float angle;
    public bool paused = false;
    private float life = 0.0f;

    void Start()
    {

    }
    
    void Update()
    {
        if (paused)
        {
            return;
        }
        life -= Time.deltaTime;
        if (life <= 0)
        {
            clearBullet();
        }
        if (!isBoss)
        {
            if (!isEnemy)
            {
                Transform currentTransform = gameObject.transform;
                Vector3 leftDirection = Vector3.Normalize(-currentTransform.transform.right) * 60 * Time.deltaTime;
                currentTransform.RotateAround(Vector3.zero, leftDirection, -60 * Time.deltaTime);
            }

            else
            {

                Transform currentTransform = gameObject.transform;
                Vector3 leftDirection = Vector3.Normalize(-currentTransform.transform.right) * 15 * Time.deltaTime;
                currentTransform.RotateAround(Vector3.zero, leftDirection, -15 * Time.deltaTime);
            }
        }

        else
        {
            float speed = 60;
            if (isPulse)
            {
                speed = 30;
            }
            if(isFlood)
            {
                speed = 20;
            }
            Transform currentTransform = gameObject.transform;
            currentTransform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    public void clearBullet()
    {
        gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 12 || collision.gameObject.layer == 13)
        {
            GameObject particleSystem = Instantiate(explosion);
            ParticleSystem system = particleSystem.transform.GetComponentInChildren<ParticleSystem>();
            system.transform.position = gameObject.transform.position;
            system.startColor = gameObject.transform.GetComponent<Renderer>().material.color;
            system.Play();
            gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            gameObject.SetActive(false);
            StopAllCoroutines();
            Ship player = collision.gameObject.GetComponent<Ship>();
            if (!player.invulnerable && !player.gameOver)
            {
                player.invulnerable = true;
                if (color == 0)
                {
                    player.HitAnimation(1);
                }
                else if (color == player.color)
                {
                    player.HitAnimation(1);
                }
            }
        }

        if (collision.gameObject.layer == 9)
        {
            GameObject particleSystem = Instantiate(explosion);
            ParticleSystem system = particleSystem.transform.GetComponentInChildren<ParticleSystem>();
            system.transform.position = gameObject.transform.position;
            system.startColor = gameObject.transform.GetComponent<Renderer>().material.color;
            system.Play();
            gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            gameObject.SetActive(false);
            StopAllCoroutines();
            Ship enemy = collision.gameObject.GetComponent<Ship>();
            if (this.tier >= enemy.tier)
            {
                if (color == 0)
                {
                    enemy.HitAnimation(1);
                }
                else if (color == enemy.color)
                {
                    enemy.HitAnimation(1);
                }
            }
        }
        if (collision.gameObject.layer == 16 || collision.gameObject.layer == 19 || collision.gameObject.layer == 20)
        {
            Boss theBoss = collision.gameObject.GetComponent<Boss>();
            GameObject particleSystem = Instantiate(explosion);
            ParticleSystem system = particleSystem.transform.GetComponentInChildren<ParticleSystem>();
            system.transform.position = gameObject.transform.position;
            system.startColor = gameObject.transform.GetComponent<Renderer>().material.color;
            system.Play();
            gameObject.SetActive(false);
            StopAllCoroutines();
            theBoss.takeDamage(1);
        }
    }
    //public void startLife()
    //{
    //    StartCoroutine(Life());
    //}

    public void startLife()
    {
        life = 0;
        if(!isEnemy)
        {
            life = 0.65f;
            if (tier == 5)
            {
                life *= 1.5f;
            }
        }
        else
        {
            life = 2.0f;
            if (isPulse && isBoss)
            {
                life *= 2.0f;
            }

            if (isFlood && isBoss)
            {
                life *= 3f;
            }
        }

        //WaitForSeconds delay = new WaitForSeconds(life);

        //yield return delay;
        //Destroy(gameObject);

        //gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        //gameObject.SetActive(false);
    }
}
