using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserPilot : Pilot
{
    private SoundManager soundManager;
    private GameObject camera;
    private float speed;
    private Material bulletMaterial;
    private bool theBossFight;

    public UserPilot(float speed, GameObject camera, Material bulletMat, bool theBossFight, SoundManager soundManager)
    {
        this.speed = speed;
        this.camera = camera;
        this.bulletMaterial = bulletMat;
        this.theBossFight = theBossFight;
        this.soundManager = soundManager;
    }

    public void MoveShip(GameObject ship)
    {
        Transform currentTransform = ship.transform;
        if (!theBossFight)
        {
            if (Input.anyKey)
            {
                Vector3 frontDirection = Vector3.Normalize(currentTransform.transform.up) * speed * Time.deltaTime;//(currentTransform.position - frontTransform.position).normalized * speed;
                Vector3 leftDirection = Vector3.Normalize(-currentTransform.transform.right) * speed * Time.deltaTime;//(currentTransform.position - leftTransform.position).normalized * speed;

                if (Input.GetKey(KeyCode.W))
                {
                    currentTransform.RotateAround(Vector3.zero, leftDirection, -speed * Time.deltaTime);
                    //currentTransform.position += frontDirection;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    currentTransform.RotateAround(Vector3.zero, frontDirection, speed * Time.deltaTime);
                    //currentTransform.position += leftDirection;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    currentTransform.RotateAround(Vector3.zero, leftDirection, speed * Time.deltaTime);
                    //currentTransform.position += -frontDirection;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    currentTransform.RotateAround(Vector3.zero, frontDirection, -speed * Time.deltaTime);
                    //currentTransform.position += -leftDirection;
                }
                if (ship.GetComponent<Ship>().tier == 5)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Ship playerShip = ship.GetComponent<Ship>();
                        Material second = playerShip.material;
                        Material secondHit = playerShip.hitMaterial;
                        bulletMaterial = playerShip.secondaryMaterial;
                        playerShip.material = playerShip.secondaryMaterial;
                        playerShip.hitMaterial = playerShip.secondaryHitMaterial;
                        playerShip.secondaryMaterial = second;
                        playerShip.secondaryHitMaterial = secondHit;
                        ship.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerShip.material;
                        if (playerShip.gameObject.layer == 13)
                        {
                            playerShip.gameObject.layer = 12;
                        }
                        else
                        {
                            playerShip.gameObject.layer = 13;
                        }
                    }
                }
            }
            Vector3 mouse = Input.mousePosition;
            Camera cam = camera.GetComponent<Camera>();
            mouse.x -= cam.pixelWidth / 2;
            mouse.y -= cam.pixelHeight / 2;
            mouse = Vector3.Normalize(mouse);
            float angle = Vector3.Angle(Vector3.up, mouse);
            if (mouse.x < 0)
            {
                angle = -angle;
            }
            currentTransform.GetChild(0).rotation = currentTransform.rotation;
            currentTransform.GetChild(0).RotateAround(currentTransform.position, -currentTransform.forward, angle);
        }

        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (currentTransform.position.y < 55.0f)
                {
                    currentTransform.Translate(Vector3.up * speed * Time.deltaTime);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (currentTransform.position.x > -105.0f)
                {
                    currentTransform.Translate(Vector3.left * speed * Time.deltaTime);
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (currentTransform.position.y > -55.0f)
                {
                    currentTransform.Translate(Vector3.down * speed * Time.deltaTime);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (currentTransform.position.x < 105.0f)
                {
                    currentTransform.Translate(Vector3.right * speed * Time.deltaTime);
                }
            }
            if (ship.GetComponent<Ship>().tier == 5)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Ship playerShip = ship.GetComponent<Ship>();
                    Material second = playerShip.material;
                    Material secondHit = playerShip.hitMaterial;
                    bulletMaterial = playerShip.secondaryMaterial;
                    playerShip.material = playerShip.secondaryMaterial;
                    playerShip.hitMaterial = playerShip.secondaryHitMaterial;
                    playerShip.secondaryMaterial = second;
                    playerShip.secondaryHitMaterial = secondHit;
                    ship.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerShip.material;
                    playerShip.bossHud.flipColor();
                    if (playerShip.gameObject.layer == 13)
                    {
                        playerShip.gameObject.layer = 12;
                    }
                    else
                    {
                        playerShip.gameObject.layer = 13;
                    }
                }
            }

            Vector3 mouse = Input.mousePosition;
            Camera cam = camera.GetComponent<Camera>();
            mouse.z = ship.transform.position.z - camera.transform.position.z;
            mouse = cam.ScreenToWorldPoint(mouse);
            Vector3 angleLine = Vector3.Normalize(mouse - ship.transform.position);
            float angle = Vector3.Angle(Vector3.up, angleLine);
            if (angleLine.x < 0)
            {
                angle = -angle;
            }
            currentTransform.GetChild(0).rotation = currentTransform.rotation;
            currentTransform.GetChild(0).RotateAround(currentTransform.transform.position, -currentTransform.transform.forward, angle);
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
        soundManager.PlayBulletFire();
        if (ship.GetComponent<Ship>().tier == 1)
        {

            Bullet theBullet = GetNonActiveBullet(bulletPool);
            theBullet.tier = ship.GetComponent<Ship>().tier;
            theBullet.isEnemy = false;
            theBullet.isFlood = false;
            theBullet.isPulse = false;

            theBullet.gameObject.layer = 10;
            theBullet.color = ship.GetComponent<Ship>().color;

            theBullet.transform.GetComponent<Renderer>().material = bulletMaterial;
            Vector3 mouse = Input.mousePosition;
            Camera cam = camera.GetComponent<Camera>();
            mouse.x -= cam.pixelWidth / 2;
            mouse.y -= cam.pixelHeight / 2;
            mouse = Vector3.Normalize(mouse);
            float angle = Vector3.Angle(Vector3.up, mouse);
            if (mouse.x < 0)
            {
                angle = -angle;
            }
            theBullet.gameObject.SetActive(true);
            theBullet.startLife();
            theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
            theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            theBullet.transform.position = ship.transform.position;
            theBullet.transform.rotation = ship.transform.rotation;
            theBullet.transform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);
            theBullet.transform.SetParent(bulletPool.transform);
        }

        else if (ship.GetComponent<Ship>().tier == 2)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x != 0)
                {
                    Bullet theBullet = GetNonActiveBullet(bulletPool);
                    theBullet.tier = ship.GetComponent<Ship>().tier;
                    theBullet.isEnemy = false;
                    theBullet.isFlood = false;
                    theBullet.isPulse = false;
                    theBullet.gameObject.layer = 10;
                    theBullet.color = ship.GetComponent<Ship>().color;

                    theBullet.transform.GetComponent<Renderer>().material = bulletMaterial;
                    Vector3 mouse = Input.mousePosition;
                    Camera cam = camera.GetComponent<Camera>();
                    mouse.x -= cam.pixelWidth / 2;
                    mouse.y -= cam.pixelHeight / 2;
                    mouse = Vector3.Normalize(mouse);
                    float angle = Vector3.Angle(Vector3.up, mouse);
                    angle += 2.5f * x;
                    if (mouse.x < 0)
                    {
                        angle = -angle;
                    }
                    if (mouse.x < 0)
                    {
                        if (x > 0)
                        {
                            theBullet.transform.position = ship.transform.position - ship.transform.right;
                        }
                        else
                        {
                            theBullet.transform.position = ship.transform.position + ship.transform.right;
                        }
                    }
                    else
                    {
                        if (x > 0)
                        {
                            theBullet.transform.position = ship.transform.position + ship.transform.right;
                        }
                        else
                        {
                            theBullet.transform.position = ship.transform.position - ship.transform.right;
                        }
                    }

                    theBullet.gameObject.SetActive(true);
                    theBullet.startLife();
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                    theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    theBullet.transform.rotation = ship.transform.rotation;
                    theBullet.transform.RotateAround(ship.transform.position, -theBullet.transform.forward, angle);
                    theBullet.transform.SetParent(bulletPool.transform);
                }
            }
        }

        else if (ship.GetComponent<Ship>().tier == 3)
        {
            for (int x = -1; x < 2; x++)
            {
                Bullet theBullet = GetNonActiveBullet(bulletPool);
                theBullet.tier = ship.GetComponent<Ship>().tier;
                theBullet.isEnemy = false;
                theBullet.isFlood = false;
                theBullet.isPulse = false;
                theBullet.gameObject.layer = 10;
                theBullet.color = ship.GetComponent<Ship>().color;

                theBullet.transform.GetComponent<Renderer>().material = bulletMaterial;
                Vector3 mouse = Input.mousePosition;
                Camera cam = camera.GetComponent<Camera>();
                mouse.x -= cam.pixelWidth / 2;
                mouse.y -= cam.pixelHeight / 2;
                mouse = Vector3.Normalize(mouse);
                float angle = Vector3.Angle(Vector3.up, mouse);
                angle += 10 * x;
                if (mouse.x < 0)
                {
                    angle = -angle;
                }
                theBullet.gameObject.SetActive(true);
                theBullet.startLife();
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                theBullet.transform.position = ship.transform.position;
                theBullet.transform.rotation = ship.transform.rotation;
                theBullet.transform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);
                theBullet.transform.SetParent(bulletPool.transform);
            }
        }

        else if (ship.GetComponent<Ship>().tier == 4 || ship.GetComponent<Ship>().tier == 5)
        {
            for (int x = -1; x <= 2; x++)
            {
                Bullet theBullet = GetNonActiveBullet(bulletPool);
                theBullet.tier = ship.GetComponent<Ship>().tier;
                theBullet.isEnemy = false;
                theBullet.isFlood = false;
                theBullet.isPulse = false;
                theBullet.gameObject.layer = 10;
                theBullet.color = ship.GetComponent<Ship>().color;

                theBullet.transform.GetComponent<Renderer>().material = bulletMaterial;
                Vector3 mouse = Input.mousePosition;
                Camera cam = camera.GetComponent<Camera>();
                float angle = 0;
                if (!theBossFight)
                {
                    mouse.x -= cam.pixelWidth / 2;
                    mouse.y -= cam.pixelHeight / 2;

                    mouse = Vector3.Normalize(mouse);
                    angle = Vector3.Angle(Vector3.up, mouse);
                    if (mouse.x < 0)
                    {
                        angle = -angle;
                    }
                }
                else
                {
                    if (ship.gameObject.layer == 12)
                    {
                        theBullet.gameObject.layer = 17;
                    }
                    else if (ship.gameObject.layer == 13)
                    {
                        theBullet.gameObject.layer = 18;
                    }
                    mouse.z = ship.transform.position.z - camera.transform.position.z;
                    mouse = cam.ScreenToWorldPoint(mouse);
                    Vector3 angleLine = Vector3.Normalize(mouse - ship.transform.position);
                    angle = Vector3.Angle(Vector3.up, angleLine);
                    theBullet.isBoss = true;
                    if (angleLine.x < 0)
                    {
                        angle = -angle;
                    }
                    //angle += 180;

                }

                if (x == 0)
                {
                    angle -= 2.5f;
                }

                else if (x == 2)
                {
                    angle += 2.5f;
                }

                else
                {
                    angle += 10 * x;
                }

                theBullet.gameObject.SetActive(true);
                theBullet.startLife();
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                theBullet.transform.position = ship.transform.position;
                theBullet.transform.rotation = ship.transform.rotation;
                theBullet.transform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angle);
                theBullet.transform.SetParent(bulletPool.transform);
            }
        }
    }
}