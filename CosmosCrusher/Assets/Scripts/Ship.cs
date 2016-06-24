using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour 
{
    public int tier;
    public int health;
    public GameObject collidedShip;
    public GameObject bullet;
    public GameObject bulletPool;
    public Pilot pilot;
    public bool fireBullets = true;
    public bool gameOver = false;
    public HUDManager hud;
    public BossHUD bossHud;
    public bool invulnerable = false;
    public Material material;
    public Material hitMaterial;
    public Material secondaryMaterial;
    public Material secondaryHitMaterial;
    public int color = 0;
    public bool bossFight;
    public bool paused = false;

    void Update()
    {
        if (!gameOver)
        {
            if (paused)
            {
                return;
            }
            pilot.MoveShip(gameObject);

            if (pilot is UserPilot && Input.GetMouseButton(0))
            {
                if (fireBullets)
                {
                    if (tier == 1)
                    {
                        pilot.Fire(gameObject, bullet.gameObject, bulletPool);
                    }

                    else if (tier == 2)
                    {
                        pilot.Fire(gameObject, bullet.gameObject, bulletPool);
                    }

                    else if (tier == 3)
                    {
                        pilot.Fire(gameObject, bullet.gameObject, bulletPool);
                    }

                    else if (tier == 4 || tier == 5)
                    {
                        pilot.Fire(gameObject, bullet.gameObject, bulletPool);
                    }

                    fireBullets = false;
                    StartCoroutine(Countdown());
                }
            }

            if (pilot is AIPilot)
            {
                pilot.Fire(gameObject, bullet, bulletPool);
            }
        }
    }
    public void HitAnimation(int damage)
    {
        health -= damage;
        if (pilot is UserPilot && bossFight)
        {
            bossHud.lostHealth(damage);
        }
        StartCoroutine(Hit());
    }
    public IEnumerator Hit()
    {
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = hitMaterial;
        yield return new WaitForSeconds(.1f);       
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = material;
        invulnerable = false;
    }
    public IEnumerator Countdown()
    {
        WaitForSeconds delay = new WaitForSeconds(.125f);
        yield return delay;
        fireBullets = true;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (pilot is UserPilot || pilot is HowToPilot)
        {
            if (collision.gameObject.layer == 9)
            {
                if (collision.gameObject.GetComponent<Ship>().tier == tier + 1)
                {
                    collidedShip = collision.gameObject;
                    if (hud != null)
                    {
                        hud.enemyDestroyed();
                    }
                    tier++;

                    //put health to full and increase size                    
                }

                else
                {
                    //take away health
                    health -= 1;
                }
            }
        }

        else
        {
            if (collision.gameObject.layer == 8)
            {

            }            
        }
    }

    public bool CollidedWithValidEnemy()
    {
        if(collidedShip != null)
        {
            return true;
        }
        
        return false;
    }
}
