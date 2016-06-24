using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject shipExplosion;
    public SoundManager soundManager;
    public List<Ship> tierPrefabs = new List<Ship>();
    public int numShipPerTier;
    public GameObject gameOverLose;
    public GameObject gameOverWin;
    public GameObject pauseScreen;
       
    public GameObject planet;
    public float speed;
    public GameObject cam;
    public GameObject bullet;
    public Material playerMaterial;
    public Material secondaryMaterial;
    public Material enemyMaterial;
    public Material playerHitMaterial;
    public Material enemyHitMaterial;
    public Material nextTierMaterial;
    public HUDManager hud;
    public GameObject bulletPool;
    public GameObject blackHole;

    private List<Ship> activeShips = new List<Ship>();
    private List<Vector3> axes = new List<Vector3>();
    private Ship playerShip;

    private int currentTier;
    private bool finalShip = false;
    private bool gameOver = false;
    private bool win = false;
    private bool paused = false;
    private float secondsToPortal = 0.85f;
    private float timePassed = 0.0f;

    void Start()
    {
        speed = speed / planet.transform.lossyScale.x;
        GenerateBullets();
        SpawnShips();
        hud.setTillNextTier(numShipPerTier, 3);
        soundManager.PlayShipSpawn();
        hud.setTier(1);
        soundManager.PlayPlanetBackground();
    }

    private void GenerateBullets()
    {
        //tier count * 20
        //20 * 20
        int numBullets = 50 * (numShipPerTier * 2);
        for (int i = 0; i < numBullets; ++i)
        {
            Bullet theBullet = Object.Instantiate(bullet.GetComponent<Bullet>()) as Bullet;
            theBullet.transform.SetParent(bulletPool.transform);
            theBullet.gameObject.SetActive(false);
        }
    }

    private void SpawnShips()
    {
        playerShip = CreatePlayerShip(tierPrefabs[0], 1);
        currentTier = 1;
        for (int i2 = 0; i2 < 2; i2++)
        {
            for (int i = 0; i < numShipPerTier; i++)
            {
                Ship ship = CreateShip(tierPrefabs[currentTier - 1], currentTier);
                activeShips.Add(ship);
                axes.Add(Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))));
            }
            ++currentTier;
        }
    }

    private Ship CreatePlayerShip(Ship prefab, int tier)
    {
        float distanceFromPlanet = (planet.transform.lossyScale.x / 2) + 1.0f;
        Vector3 position = Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))) * distanceFromPlanet;
        Ship ship = Instantiate(prefab) as Ship;
        ship.gameObject.layer = 8;
        ship.health = 5;
        ship.transform.position = position;
        ship.transform.rotation = Quaternion.LookRotation(Vector3.zero - position);
        ship.bullet = bullet;
        ship.tier = tier;
        ship.pilot = new UserPilot(speed, cam, playerMaterial, false, soundManager);
        ship.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerMaterial;
        ship.bullet = bullet;
        ship.bulletPool = bulletPool;
        ship.hud = hud;
        ship.material = playerMaterial;
        ship.hitMaterial = playerHitMaterial;
        return ship;
    }

    private Ship CreateShip(Ship prefab, int tier)
    {
        float distanceFromPlanet = (planet.transform.lossyScale.x / 2) + 1.0f;
        bool goodPosition = false;
        Vector3 position = new Vector3();
        while (!goodPosition)
        {
            position = Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))) * distanceFromPlanet;
            if (Vector3.Distance(playerShip.transform.position, position) > planet.transform.lossyScale.x / 8)
            {
                goodPosition = true;
            }
        }

        Ship ship = Instantiate(prefab) as Ship;
        ship.gameObject.layer = 9;
        ship.health = 5;
        ship.transform.position = position;
        ship.transform.rotation = Quaternion.LookRotation(Vector3.zero - position);
        ship.bullet = bullet;
        ship.tier = tier;       
        ship.hud = hud;
        ship.bulletPool = bulletPool;
        if (ship.tier - playerShip.tier == 1)
        {
            ship.material = nextTierMaterial;
            ship.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = nextTierMaterial;
        }
        else
        {
            ship.material = enemyMaterial;
        }
        ship.pilot = new AIPilot(speed * .675f, ship.material);
        ship.hitMaterial = enemyHitMaterial;
        return ship;
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
        Vector3 playerPosition = playerShip.transform.position;
        Vector3 closestEnemyPosition = new Vector3();
        Color color = new Color();
        foreach (Ship enemy in activeShips)
        {
            Vector3 enemyPosition = enemy.gameObject.transform.position;
            if (Vector3.Distance(playerPosition, enemyPosition) < closestEnemy)
            {
                closestEnemyPosition = enemyPosition;
                closestEnemy = Vector3.Distance(playerShip.transform.position, enemy.gameObject.transform.position);
                color = enemy.material.color;
            }
        }
        hud.rotateTracker(playerPosition, closestEnemyPosition, playerShip.transform.forward, playerShip.transform.right, -playerShip.transform.up, color);
        return closestEnemy;
    }

    private Vector3 projection(Vector3 first, Vector3 second)
    {
        float length = Vector3.Dot(first, second) / second.magnitude;
        return second.normalized * length;
    }

    void LateUpdate()
    {
        if (!gameOver)
        {
            cam.transform.position = playerShip.gameObject.transform.position + Vector3.Normalize(playerShip.gameObject.transform.position) * 60;
            cam.transform.rotation = Quaternion.LookRotation(playerShip.gameObject.transform.position - cam.transform.position, playerShip.transform.up.normalized);
        }        
    }

    void Update()
    {
        if (!gameOver)
        {
            if (paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    unPause();
                }
                else
                {
                    return;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseGame();
            }
            if (currentTier <= tierPrefabs.Count - 1 && activeShips.Count <= numShipPerTier)
            {
                for (int i = 0; i < numShipPerTier; i++)
                {
                    Ship ship = CreateShip(tierPrefabs[currentTier - 1], currentTier);
                    activeShips.Add(ship);
                    axes.Add(Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))));
                }
                ++currentTier; 
                if (currentTier == tierPrefabs.Count) {
                    hud.setTillNextTier(activeShips.Count, currentTier);
                }
                else {
                    hud.setTillNextTier(numShipPerTier, currentTier);
                }
                soundManager.PlayShipSpawn();
            }
            if (playerShip.CollidedWithValidEnemy())
            {
                playerShip.collidedShip.GetComponent<Ship>().pilot = playerShip.pilot;
                Ship validEnemy = playerShip.collidedShip.GetComponent<Ship>();
                validEnemy.gameObject.transform.rotation = playerShip.gameObject.transform.rotation;
                Destroy(playerShip.gameObject);
                playerShip = validEnemy;
                activeShips.Remove(validEnemy);
                playerShip.gameObject.layer = 8;
                playerShip.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerMaterial;
                playerShip.invulnerable = true;
                playerShip.material = playerMaterial;
                playerShip.hitMaterial = playerHitMaterial;
                StartCoroutine(InvinciFrames());
                hud.setTier(playerShip.tier);
                if(playerShip.tier == tierPrefabs.Count)
                {
                    playerShip.color = 1;
                }
                foreach (Ship ship in activeShips)
                {
                    AIPilot pilot = (AIPilot)ship.pilot;
                    if (ship.tier - playerShip.tier == 1)
                    {
                        ship.material = nextTierMaterial;
                        ship.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = nextTierMaterial;
                        pilot.ChangeBulletMaterial(nextTierMaterial);
                    }
                    else
                    {
                        ship.material = enemyMaterial;
                        ship.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = enemyMaterial;
                        pilot.ChangeBulletMaterial(enemyMaterial);
                    }
                }
                GameObject shipDeath = Instantiate(shipExplosion) as GameObject;
                ParticleSystem system = shipDeath.transform.GetComponentInChildren<ParticleSystem>();
                system.transform.position = playerShip.transform.position;
                system.startColor = playerShip.material.color;
                system.Play();
                soundManager.PlayShipTakeover();
            }

            List<Ship> destroyedShips = new List<Ship>();
            foreach (Ship ship in activeShips)
            {
                if (ship.health <= 0)
                {
                    destroyedShips.Add(ship);
                    GameObject shipDeath = Instantiate(shipExplosion) as GameObject;
                    ParticleSystem system = shipDeath.transform.GetComponentInChildren<ParticleSystem>();
                    system.transform.position = ship.transform.position;
                    system.startColor = ship.material.color;
                    system.Play();
                }
            }
            foreach (Ship ship in destroyedShips)
            {
                activeShips.Remove(ship);
                Destroy(ship.gameObject);
                hud.enemyDestroyed();
                soundManager.PlayShipDestruction();
            }
            hud.checkHealth(playerShip.health);
            IndicatorColorChange();
            if (playerShip.health <= 0)
            {
                GameObject shipDeath = Instantiate(shipExplosion) as GameObject;
                ParticleSystem system = shipDeath.transform.GetComponentInChildren<ParticleSystem>();
                system.transform.position = playerShip.transform.position;
                system.startColor = playerShip.material.color;
                system.Play();
                gameOverLose.SetActive(true);
                gameOver = true;
                Destroy(playerShip.gameObject);
                playerShip = null;
                foreach (Ship ship in activeShips)
                {
                    ship.gameOver = true;
                }
                soundManager.PlayShipDestruction();
            }
            else if (activeShips.Count == 0 && !finalShip)
            {
                soundManager.PlayShipSpawn();
                Ship ship = CreateShip(tierPrefabs[currentTier - 1], currentTier);
                activeShips.Add(ship);
                axes.Add(Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))));
                hud.setFinalTilNextTier();
                finalShip = true;
            }
            else if (activeShips.Count == 0 && finalShip)
            {
                blackHole.SetActive(true);
                gameOver = true;
                win = true;
                playerShip.gameOver = true;
            }
        }
        else if (win)
        {
           timePassed += Time.deltaTime;
           Vector3 leftDirection = Vector3.Normalize(-playerShip.transform.right) * speed * Time.deltaTime;
           playerShip.transform.RotateAround(Vector3.zero, leftDirection, -speed * Time.deltaTime);
           if (timePassed >= secondsToPortal)
           {
               Application.LoadLevel("BossScene");
           }
        }
    }

    private void pauseGame()
    {
        paused = true;
        pauseScreen.SetActive(true);
        foreach (Ship s in activeShips)
        {
            s.paused = true;
        }
        foreach (Bullet b in bulletPool.GetComponentsInChildren<Bullet>()) {
            b.paused = true;
        }
        playerShip.paused = true;
        soundManager.PauseSounds();
    }

    private void unPause()
    {
        paused = false;
        pauseScreen.SetActive(false);
        foreach (Ship s in activeShips)
        {
            s.paused = false;
        }
        foreach (Bullet b in bulletPool.GetComponentsInChildren<Bullet>())
        {
            b.paused = false;
        }
        playerShip.paused = false;
        soundManager.UnPauseSounds();
    }

    private IEnumerator InvinciFrames()
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);
        yield return delay;
        playerShip.invulnerable = false;
    }

    public void Restart()
    {
        gameOverLose.SetActive(false);
        gameOverWin.SetActive(false);
        Destroy(playerShip);
        foreach (Ship s in activeShips)
        {
            Destroy(s);
        }
        activeShips = new List<Ship>();
        gameOver = false;
        SpawnShips();
    }
}
