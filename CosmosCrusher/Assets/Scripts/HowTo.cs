using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HowTo : MonoBehaviour 
{
    private float speed = 20.0f;
    public GameObject shipExplosion;
    public GameObject bulletPool;
    public GameObject bullet;
    public SoundManager soundManager;
    public Material blueMat;
    public Ship playerShip;
    public Ship enemyShipOne;
    public Ship enemyShipTwo;
    public GameObject cam;
    public Material purpleMaterial;
    public Material playerMaterial;
    public Material playerHitMaterial;
    public Material enemyMaterial;
    public Material enemyHitMaterial;
    public HUDManager hud;
    public Text text;
    public GameObject planet;
    public Image textIndicator;
    private UserPilot playerPilot;
    private bool end;
    private bool changed = false;
    void Start()
    {
        playerShip.pilot = new UserPilot(speed, cam, blueMat, false, soundManager);
        playerPilot = (UserPilot)playerShip.pilot;
        playerShip.fireBullets = false;
        playerShip.bulletPool = bulletPool;
        playerShip.tier = 1;
        enemyShipOne.pilot = new HowToPilot();
        enemyShipOne.material = purpleMaterial;
        enemyShipOne.hitMaterial = enemyHitMaterial;
        enemyShipOne.gameObject.layer = 9;
        enemyShipTwo.pilot = new HowToPilot();
        enemyShipTwo.material = purpleMaterial;
        enemyShipTwo.hitMaterial = enemyHitMaterial;
        enemyShipTwo.gameObject.layer = 9;
        enemyShipTwo.health = 5;
        hud.setTier(1);
        GenerateBullets();
        end = false;
        textIndicator.enabled = true;
        textIndicator.canvasRenderer.SetAlpha(0.0f);
        soundManager.PlayPlanetBackground();
    }
    void Update()
    {
        IndicatorColorChange();
        if (Input.GetMouseButton(0))
        {
            if (playerShip.fireBullets)
            {
                playerPilot.Fire(playerShip.gameObject, bullet.gameObject, bulletPool);
                playerShip.fireBullets = false;
                StartCoroutine(playerShip.Countdown());
            }
        }
        if (playerShip.CollidedWithValidEnemy())
        {
            if (playerShip.collidedShip == enemyShipOne.gameObject)
            {
                enemyShipOne = null;
            }
            else
            {
                enemyShipTwo = enemyShipOne;
                enemyShipOne = null;
            }
            playerShip.collidedShip.GetComponent<Ship>().pilot = playerShip.pilot;
            Ship validEnemy = playerShip.collidedShip.GetComponent<Ship>();
            validEnemy.gameObject.transform.rotation = playerShip.gameObject.transform.rotation;
            GameObject shipDeath = Instantiate(shipExplosion) as GameObject;
            ParticleSystem system = shipDeath.transform.GetComponentInChildren<ParticleSystem>();
            system.transform.position = playerShip.transform.position;
            system.startColor = playerShip.material.color;
            system.Play();
            Destroy(playerShip.gameObject);
            playerShip = validEnemy;
            playerShip.gameObject.layer = 8;
            playerShip.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerMaterial;
            playerShip.invulnerable = true;
            playerShip.material = playerMaterial;
            playerShip.hitMaterial = playerHitMaterial;
            playerShip.tier = 2;
            playerShip.bulletPool = bulletPool;
            playerShip.fireBullets = false;
            enemyShipTwo.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = enemyMaterial;
            enemyShipTwo.material = enemyMaterial;
            hud.setTier(playerShip.tier);
            soundManager.PlayShipTakeover();
            ChangeText();
        }
        hud.checkHealth(playerShip.health);
        if (!changed && playerShip.health < 5)
        {
            ChangeText2();
            playerShip.fireBullets = true;
            changed = true;
        }
        if (enemyShipTwo != null && enemyShipTwo.health <= 0)
        {
            GameObject shipDeath = Instantiate(shipExplosion) as GameObject;
            ParticleSystem system = shipDeath.transform.GetComponentInChildren<ParticleSystem>();
            system.transform.position = enemyShipTwo.transform.position;
            system.startColor = enemyShipTwo.material.color;
            system.Play();
            Destroy(enemyShipTwo.gameObject);
            soundManager.PlayShipDestruction();
            StartCoroutine(Pause()); 
        }
        
        if (end)
        {
            Application.LoadLevel("MainMenu");
        }
    }
    private void fadeIndicator(Image indicator)
    {
        indicator.canvasRenderer.SetAlpha(1.0f);
        indicator.CrossFadeAlpha(0.0f, 0.75f, false);
    }
    private void GenerateBullets()
    {
        int numBullets = 100;
        for (int i = 0; i < numBullets; ++i)
        {
            Bullet theBullet = Object.Instantiate(bullet.GetComponent<Bullet>()) as Bullet;
            theBullet.transform.SetParent(bulletPool.transform);
            theBullet.gameObject.SetActive(false);
        }
    }
    IEnumerator Pause()
    {
        WaitForSeconds delay = new WaitForSeconds(1.0f);
        yield return delay;
        end = true;
    }
    void ChangeText()
    {
        text.text = "Now, seek out the red ship and run into it.";
        fadeIndicator(textIndicator);
    }
    void ChangeText2()
    {
        text.text = "Running into a red ship damages you. Destroy the red ship by holding fire and aiming at it.";
        fadeIndicator(textIndicator);
    }
    void LateUpdate()
    {
        cam.transform.position = playerShip.gameObject.transform.position + Vector3.Normalize(playerShip.gameObject.transform.position) * 60;
        cam.transform.rotation = Quaternion.LookRotation(playerShip.gameObject.transform.position - cam.transform.position, playerShip.transform.up.normalized);
    }
    private void IndicatorColorChange()
    {
        float distance = ClosestShipDistance();
        float beta = distance / planet.transform.lossyScale.x;
        hud.updateIndicator(beta);
    }

    private float ClosestShipDistance()
    {
        float closestEnemy = planet.transform.lossyScale.x;
        GameObject enemy = null;
        if (enemyShipOne != null && Vector3.Distance(playerShip.transform.position, enemyShipOne.gameObject.transform.position) < closestEnemy)
        {
            closestEnemy = Vector3.Distance(playerShip.transform.position, enemyShipOne.gameObject.transform.position);
            enemy = enemyShipOne.gameObject;
        }
        if (enemyShipTwo != null && Vector3.Distance(playerShip.transform.position, enemyShipTwo.gameObject.transform.position) < closestEnemy)
        {
            closestEnemy = Vector3.Distance(playerShip.transform.position, enemyShipTwo.gameObject.transform.position);
            enemy = enemyShipTwo.gameObject;
        }
        if (enemy != null)
        {
            hud.rotateTracker(playerShip.gameObject.transform.position, enemy.gameObject.transform.position, playerShip.transform.forward, playerShip.transform.right, -playerShip.transform.up, enemy.GetComponent<Ship>().material.color);
        }
        return closestEnemy;
    }
}
