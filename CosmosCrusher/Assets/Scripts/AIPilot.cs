using UnityEngine;
using System.Collections;

public class AIPilot : Pilot
{

    public float secondsToChange = 2.0f;
    public float distanceToAttack = 20.0f;

    private float speed;
    private float secondsPassed;
    private int randomDirection;
    private bool fireTier1 = false;
    private bool fireTier2 = false;
    private bool fireTier3 = false;
    private bool fireTier4 = false;
    private bool fireTier5 = false;
    private float tier1Delay;
    private float tier2Delay;
    private float tier3Delay;
    private float tier4Delay;
    private float tier5Delay;
    private int tier4Increment = 0;
    private int tier5Increment = 0;
    private Material bulletMaterial;

    public AIPilot(float speed, Material bulletMat)
    {
        this.speed = speed;
        this.bulletMaterial = bulletMat;
    }

    public void ChangeBulletMaterial(Material bulletMat)
    {
        bulletMaterial = bulletMat;
    }

    public void MoveShip(GameObject ship)
    {
        Transform theTransform = ship.transform;
        if (secondsPassed >= secondsToChange)
        {
            secondsPassed = 0;
            randomDirection = Random.Range(0, 8);
        }
        else
        {
            secondsPassed += Time.deltaTime;
        }
        Vector3 frontDirection = Vector3.Normalize(theTransform.transform.up) * speed * Time.deltaTime;
        Vector3 leftDirection = Vector3.Normalize(-theTransform.transform.right) * speed * Time.deltaTime;
        switch (randomDirection)
        {
            case 0: theTransform.RotateAround(Vector3.zero, leftDirection, -speed * Time.deltaTime);
                break;
            case 1: theTransform.RotateAround(Vector3.zero, frontDirection, speed * Time.deltaTime);
                break;
            case 2: theTransform.RotateAround(Vector3.zero, frontDirection, -speed * Time.deltaTime);
                break;
            case 3: theTransform.RotateAround(Vector3.zero, leftDirection, speed * Time.deltaTime);
                break;
            case 4: theTransform.RotateAround(Vector3.zero, (leftDirection.normalized + frontDirection.normalized) * speed * Time.deltaTime, speed * Time.deltaTime);
                break;
            case 5: theTransform.RotateAround(Vector3.zero, (leftDirection.normalized - frontDirection.normalized) * speed * Time.deltaTime, speed * Time.deltaTime);
                break;
            case 6: theTransform.RotateAround(Vector3.zero, -(leftDirection.normalized + frontDirection.normalized) * speed * Time.deltaTime, speed * Time.deltaTime);
                break;
            case 7: theTransform.RotateAround(Vector3.zero, -(leftDirection.normalized - frontDirection.normalized) * speed * Time.deltaTime, speed * Time.deltaTime);
                break;
            default:
                Debug.LogError("The random number generator did not produce a number from 0 - 7");
                break;
        }
    }

    private Bullet GetNonActiveBullet(GameObject bulletPool)
    {
        for (int i = 0; i < bulletPool.transform.childCount; ++i)
        {
            if (!bulletPool.transform.GetChild(i).gameObject.activeSelf)
            {
                Bullet bill = bulletPool.transform.GetChild(i).GetComponent<Bullet>() as Bullet;
                bill.transform.rotation = new Quaternion();
                return bill;
            }
        }
        Debug.LogError("NOT Enough Bullets");
        return null;
    }
    public void Fire(GameObject ship, GameObject bullet, GameObject bulletPool)
    {
        if (ship.GetComponent<Ship>().tier == 1)
        {
            if (fireTier1)
            {
                tier1Delay = 0;
                Bullet theBullet = GetNonActiveBullet(bulletPool);
                theBullet.color = ship.GetComponent<Ship>().color;
                theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                theBullet.tier = ship.GetComponent<Ship>().tier;
                theBullet.gameObject.layer = 11;
                theBullet.isEnemy = true;
                theBullet.transform.position = ship.transform.position;
                theBullet.transform.rotation = ship.transform.rotation;
                theBullet.transform.SetParent(bulletPool.transform);
                float angle = 0;
                theBullet.gameObject.SetActive(true);
                theBullet.startLife();
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                Transform bulletTransform = theBullet.transform;
                bulletTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);

                fireTier1 = false;
            }

            else
            {
                tier1Delay += Time.deltaTime;

                if (tier1Delay >= 0.25)
                {
                    fireTier1 = true;
                }
            }
        }

        if(ship.GetComponent<Ship>().tier == 2)
        {
            if (fireTier2)
            {
                tier2Delay = 0;

                for (float x = 0; x < 12; x++)
                {
                    Bullet theBullet = GetNonActiveBullet(bulletPool);
                    theBullet.color = ship.GetComponent<Ship>().color;
                    theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                    theBullet.tier = ship.GetComponent<Ship>().tier;
                    theBullet.gameObject.layer = 11;
                    theBullet.isEnemy = true;
                    theBullet.transform.position = ship.transform.position;
                    theBullet.transform.rotation = ship.transform.rotation;
                    theBullet.transform.SetParent(bulletPool.transform);
                    float angle = 30 * x;
                    theBullet.gameObject.SetActive(true);
                    theBullet.startLife();
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    Transform bulletTransform = theBullet.transform;
                    bulletTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);
                }

                fireTier2 = false;
            }

            else
            {
                tier2Delay += Time.deltaTime;

                if (tier2Delay >= 2.5f)
                {
                    fireTier2 = true;
                }
            }
        }

        if (ship.GetComponent<Ship>().tier == 3)
        {
            if (fireTier3)
            {
                tier3Delay = 0;

                for (float x = -1; x < 2; x++)
                {
                    Bullet theBullet = GetNonActiveBullet(bulletPool);
                    theBullet.color = ship.GetComponent<Ship>().color;
                    theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                    theBullet.tier = ship.GetComponent<Ship>().tier;
                    theBullet.gameObject.layer = 11;
                    theBullet.isEnemy = true;
                    theBullet.transform.position = ship.transform.position;
                    theBullet.transform.rotation = ship.transform.rotation;
                    theBullet.transform.SetParent(bulletPool.transform);
                    float angle = 120 * x;
                    theBullet.gameObject.SetActive(true);
                    theBullet.startLife();
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    Transform bulletTransform = theBullet.transform;
                    bulletTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);
                }

                fireTier3 = false;
            }

            else
            {
                tier3Delay += Time.deltaTime;

                if (tier3Delay >= .25)
                {
                    fireTier3 = true;
                }
            }
        }

        if (ship.GetComponent<Ship>().tier == 4)
        {
            if (fireTier4)
            {
                tier4Delay = 0;
                Bullet theBullet = GetNonActiveBullet(bulletPool);
                theBullet.color = ship.GetComponent<Ship>().color;
                theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                theBullet.tier = ship.GetComponent<Ship>().tier;
                theBullet.gameObject.layer = 11;
                theBullet.isEnemy = true;
                theBullet.transform.position = ship.transform.position;
                theBullet.transform.rotation = ship.transform.rotation;
                theBullet.transform.SetParent(bulletPool.transform);

                if(tier4Increment == 12)
                {
                    tier4Increment = 0;
                }

                float angle = 30 * tier4Increment;

                tier4Increment++;

                theBullet.gameObject.SetActive(true);
                theBullet.startLife();
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                Transform bulletTransform = theBullet.transform;
                bulletTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);

                fireTier4 = false;
            }

            else
            {
                tier4Delay += Time.deltaTime;

                if (tier4Delay >= .1)
                {
                    fireTier4 = true;
                }
            }
        }
        if (ship.GetComponent<Ship>().tier == 5)
        {
            if (fireTier5)
            {
                tier5Delay = 0;
                Bullet theBullet = GetNonActiveBullet(bulletPool);
                theBullet.color = ship.GetComponent<Ship>().color;
                theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                theBullet.tier = ship.GetComponent<Ship>().tier;
                theBullet.gameObject.layer = 11;
                theBullet.isEnemy = true;
                theBullet.transform.position = ship.transform.position;
                theBullet.transform.rotation = ship.transform.rotation;
                theBullet.transform.SetParent(bulletPool.transform);
                theBullet.gameObject.SetActive(true);
                theBullet.startLife();
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

                Bullet theBullet2 = GetNonActiveBullet(bulletPool);
                theBullet2.color = ship.GetComponent<Ship>().color;
                theBullet2.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                theBullet2.tier = ship.GetComponent<Ship>().tier;
                theBullet2.gameObject.layer = 11;
                theBullet2.isEnemy = true;
                theBullet2.transform.position = ship.transform.position;
                theBullet2.transform.rotation = ship.transform.rotation;
                theBullet2.transform.SetParent(bulletPool.transform);
                theBullet2.gameObject.SetActive(true);
                theBullet2.startLife();
                theBullet2.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet2.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

                if (tier5Increment == 12)
                {
                    tier5Increment = 0;
                }

                float angle = 30 * tier5Increment;

                tier5Increment++;

                Transform bulletTransform = theBullet.transform;
                bulletTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);

                bulletTransform = theBullet2.transform;
                bulletTransform.RotateAround(theBullet2.transform.position, -theBullet2.transform.forward, -angle);

                fireTier5 = false;
            }

            else
            {
                tier5Delay += Time.deltaTime;

                if (tier5Delay >= .08)
                {
                    fireTier5 = true;
                }
            }
        }
    }
}
