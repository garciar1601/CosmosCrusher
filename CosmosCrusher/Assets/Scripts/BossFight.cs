using UnityEngine;
using System.Collections;

public class BossFight : MonoBehaviour 
{
    public SoundManager soundManager;
    public GameObject theBoss;
    public GameObject bullet;
    
    public Material bulletMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material playerHitMaterial;
    public Material playerMaterial;
    public Material secondaryMaterial;
    public Material secondaryHitMaterial;
    public Material purpleMaterial;
    public ParticleSystem bossDeath;

    public Ship theShip;
    public BossHUD theHud;
    public GameObject cam;
    public GameObject bulletPool;
    public GameObject gameOverLose;
    public GameObject gameOverWin;
    public GameObject pauseScreen;

    private float angleOfBullets1;
    private float angleOfBullets2;
    private float angleOfBullets3;
    private float angleOfBullets4;
    private float rateOfChangeOfAngles;
    private bool firing = true;
    private bool flood = false;
    private float floodDelay = 0.0f;
    private bool floodSwitch = false;
    private float floodDelayTime = 0;
    private Ship playerShip;
    private bool pulse = false;
    private float pulseDelay = 0.0f;
    private bool gameOver = false;

    int counter = 0;
    int numBullets = 2000;
    bool paused = false;
    private bool phaseShifted = false;
    private bool shiftStarted = false;
    
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

	void Start () 
    {
        rateOfChangeOfAngles = 55.0f;
        angleOfBullets1 = 0;
        angleOfBullets2 = 90;
        angleOfBullets3 = 180;
        angleOfBullets4 = 270;
        MakeShip();
        GenerateBullets();
        StartCoroutine(StartDelay());
        soundManager.PlayBossBackground();
	}

    private void GenerateBullets()
    {
        //tier count * 20
        //20 * 20
        for (int i = 0; i < numBullets; ++i)
        {
            Bullet theBullet = Object.Instantiate(bullet.GetComponent<Bullet>()) as Bullet;
            theBullet.transform.SetParent(bulletPool.transform);
            theBullet.gameObject.SetActive(false);
        }
    }

	
	void Update () 
    {
        if (gameOver)
        {
            return;
        }
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
        if (!firing)
        {
            firing = true;
            StartCoroutine(Fire());
        }

        if (!gameOver)
        {
            if (theBoss.GetComponent<Boss>().shield > 0)
            {
                Flood();
            }

            else if(phaseShifted)
            {
                Pulse();
            }

            else if(!shiftStarted)
            {
                StartCoroutine(changePhase());
            }
        }

        if (theBoss != null && theBoss.GetComponent<Boss>().Health <= 0)
        {
            soundManager.PlayBossDestruction();
            StopAllCoroutines();
            firing = true;
            gameOverWin.SetActive(true);
            Destroy(theBoss.gameObject);
            gameOver = true;
            playerShip.gameOver = true;
            clearAllBullets();
            bossDeath.Play();
        }

        else if (playerShip != null && playerShip.health <= 0)
        {
            soundManager.PlayShipDestruction();
            StopAllCoroutines();
            firing = true;
            gameOverLose.SetActive(true);
            Destroy(playerShip.gameObject);
            gameOver = true;
            clearAllBullets();
        }
	}

    private void clearAllBullets()
    {
        foreach (Bullet b in bulletPool.GetComponentsInChildren<Bullet>())
        {
            b.clearBullet();
        }
    }

    private void pauseGame()
    {
        pauseScreen.SetActive(true);
        playerShip.paused = true;
        paused = true;
        foreach (Bullet b in bulletPool.GetComponentsInChildren<Bullet>())
        {
            b.paused = true;
        }
        soundManager.PauseSounds();
    }

    private void unPause()
    {
        pauseScreen.SetActive(false);
        playerShip.paused = false;
        foreach (Bullet b in bulletPool.GetComponentsInChildren<Bullet>())
        {
            b.paused = false;
        }
        paused = false;
        soundManager.UnPauseSounds();
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1.0f);
        firing = false;
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(0.005f);

        if (!phaseShifted)
        {
            BossPhase1();
        }

        else if(phaseShifted)
        {
            BossFire();
        }
        
        firing = false;        
    }

    private Bullet GetNonActiveBullet()
    {
        int j = 0;
        for (int i = counter; j < bulletPool.transform.childCount; ++i)
        {
            if (!bulletPool.transform.GetChild(i).gameObject.activeSelf)
            {
                Bullet bill = bulletPool.transform.GetChild(i).GetComponent<Bullet>() as Bullet;
                bill.transform.rotation = new Quaternion();
                return bill;
            }
            ++counter;
            if (counter >= numBullets)
            {
                counter -= numBullets;
                i -= numBullets;
            }
            ++j;
        }
        Debug.LogError("NOT Enough Bullets");
        return null;
    }

    void BossFire()
    {
        if (paused) { return; }

        angleOfBullets1 %= 360;
        angleOfBullets2 %= 360;
        angleOfBullets3 %= 360;      
        angleOfBullets4 %= 360;

        if (paused) { return; }

        angleOfBullets1 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets2 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets3 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets4 += rateOfChangeOfAngles * Time.deltaTime;

        if (paused) { return; }
        //Spin spin 1
        Bullet theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 11;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets1;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        if (paused) { return; }
        //Transform bulletTransform = bullet.transform;
        //bulletTransform.Translate(Vector3.down * Time.deltaTime);
        Transform currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets1);

        if (paused) { return; }
        //Spin spin 2
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 11;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets2;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets2);

        if (paused) { return; }
        //Spin spin 3
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 11;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets3;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets3);

        if (paused) { return; }
        //Spin spin 4
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 11;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets4;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets4);

        if (paused) { return; }
        //Stationary 1
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 45;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 45);

        if (paused) { return; }
        //Stationary 2
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 135;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 135);

        if (paused) { return; }
        //Stationary 3
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 225;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 225);

        if (paused) { return; }
        //Stationary 4
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 315;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 315);
    }

    private void Pulse()
    {
        if (pulse)
        {
            Material bulletMaterial = blueMaterial;
            int bulletLayer = 14;
            int bossLayer = 19;
            if (Random.value > 0.5f)
            {
                bulletMaterial = greenMaterial;
                bulletLayer = 15;
                bossLayer = 20;
            }
            for (float x = 0; x < 96; x++)
            {
                Bullet theBullet = GetNonActiveBullet();
                theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                theBullet.tier = 5;
                theBullet.gameObject.layer = bulletLayer;
                theBullet.isEnemy = true;
                theBullet.isBoss = true;
                theBullet.isPulse = true;
                theBullet.isFlood = false;
                theBullet.angle = 3.75f * x;
                theBullet.transform.position = theBoss.transform.position;
                theBullet.gameObject.SetActive(true);
                theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                theBoss.gameObject.GetComponent<Renderer>().material = bulletMaterial;
                theBoss.gameObject.layer = bossLayer;
                theBullet.GetComponent<Bullet>().startLife();
                if (paused) { theBullet.paused = true; }

                Transform currentTransform = theBullet.transform;
                currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, theBullet.angle);
            }
            pulse = false;
            pulseDelay = 0.0f;
        }
        else
        {
            if (pulseDelay < 2.5f)
            {
                pulseDelay += Time.deltaTime;
            }
            else
            {
                pulse = true;
            }
        }
    }

    void BossPhase1()
    {
        if (paused) { return; }

        angleOfBullets1 %= 360;
        angleOfBullets2 %= 360;
        angleOfBullets3 %= 360;
        angleOfBullets4 %= 360;

        if (paused) { return; }

        angleOfBullets1 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets2 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets3 += rateOfChangeOfAngles * Time.deltaTime;
        angleOfBullets4 += rateOfChangeOfAngles * Time.deltaTime;

        Bullet theBullet = GetNonActiveBullet();
        //Stationary 1
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 45;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        Transform currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 45);

        //Stationary 2
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 135;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 135);

        //Stationary 3
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 225;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 225);

        //Stationary 4
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = 315;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, 315);

        //Spin spin 1
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets1;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        if (paused) { return; }
        //Transform bulletTransform = bullet.transform;
        //bulletTransform.Translate(Vector3.down * Time.deltaTime);
        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets1);

        if (paused) { return; }
        //Spin spin 2
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets2;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets2);

        if (paused) { return; }
        //Spin spin 3
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = blueMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 14;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets3;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = blueMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets3);

        if (paused) { return; }
        //Spin spin 4
        theBullet = GetNonActiveBullet();
        theBullet.gameObject.transform.GetComponent<Renderer>().material = greenMaterial;
        theBullet.tier = 5;
        theBullet.gameObject.layer = 15;
        theBullet.isEnemy = true;
        theBullet.isBoss = true;
        theBullet.isPulse = false;
        theBullet.isFlood = false;
        theBullet.angle = angleOfBullets4;
        theBullet.transform.position = theBoss.transform.position;
        theBullet.gameObject.SetActive(true);
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = greenMaterial.color;
        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        theBullet.GetComponent<Bullet>().startLife();
        if (paused) { theBullet.paused = true; }

        currentTransform = theBullet.transform;
        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, angleOfBullets4);
    }

    void Flood()
    {
        if (flood)
        {
            int bulletLayer = 11;

            if (floodDelayTime >= .25f)
            {
                floodDelayTime = 0;
                if (floodSwitch)
                {
                    for (float x = -11; x < 12; x++)
                    {
                        Bullet theBullet = GetNonActiveBullet();
                        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                        theBullet.tier = 5;
                        theBullet.gameObject.layer = bulletLayer;
                        theBullet.isEnemy = true;
                        theBullet.isBoss = true;
                        theBullet.isPulse = false;
                        theBullet.isFlood = true;
                        theBullet.angle = 3.75f * x;
                        theBullet.transform.position = theBoss.transform.position;
                        theBullet.gameObject.SetActive(true);
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        theBullet.GetComponent<Bullet>().startLife();
                        if (paused) { theBullet.paused = true; }

                        Transform currentTransform = theBullet.transform;
                        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, theBullet.angle);
                    }
                    for (float x = 37; x < 60; x++)
                    {
                        Bullet theBullet = GetNonActiveBullet();
                        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                        theBullet.tier = 5;
                        theBullet.gameObject.layer = bulletLayer;
                        theBullet.isEnemy = true;
                        theBullet.isBoss = true;
                        theBullet.isPulse = false;
                        theBullet.isFlood = true;
                        theBullet.angle = 3.75f * x;
                        theBullet.transform.position = theBoss.transform.position;
                        theBullet.gameObject.SetActive(true);
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        theBullet.GetComponent<Bullet>().startLife();
                        if (paused) { theBullet.paused = true; }

                        Transform currentTransform = theBullet.transform;
                        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, theBullet.angle);
                    }
                }

                else
                {
                    for (float x = 13; x < 36; x++)
                    {
                        Bullet theBullet = GetNonActiveBullet();
                        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                        theBullet.tier = 5;
                        theBullet.gameObject.layer = bulletLayer;
                        theBullet.isEnemy = true;
                        theBullet.isBoss = true;
                        theBullet.isPulse = false;
                        theBullet.isFlood = true;
                        theBullet.angle = 3.75f * x;
                        theBullet.transform.position = theBoss.transform.position;
                        theBullet.gameObject.SetActive(true);
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        theBullet.GetComponent<Bullet>().startLife();
                        if (paused) { theBullet.paused = true; }

                        Transform currentTransform = theBullet.transform;
                        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, theBullet.angle);
                    }

                    for (float x = 61; x < 84; x++)
                    {
                        Bullet theBullet = GetNonActiveBullet();
                        theBullet.gameObject.transform.GetComponent<Renderer>().material = bulletMaterial;
                        theBullet.tier = 5;
                        theBullet.gameObject.layer = bulletLayer;
                        theBullet.isEnemy = true;
                        theBullet.isBoss = true;
                        theBullet.isPulse = false;
                        theBullet.isFlood = true;
                        theBullet.angle = 3.75f * x;
                        theBullet.transform.position = theBoss.transform.position;
                        theBullet.gameObject.SetActive(true);
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().startColor = bulletMaterial.color;
                        theBullet.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                        theBullet.GetComponent<Bullet>().startLife();
                        if (paused) { theBullet.paused = true; }

                        Transform currentTransform = theBullet.transform;
                        currentTransform.RotateAround(theBullet.transform.position, -theBullet.transform.forward, theBullet.angle);
                    }
                }
            }

            else
            {
                floodDelayTime += Time.deltaTime;
            }

            if (floodDelay < 2.5f)
            {
                floodDelay += Time.deltaTime;
            }

            else
            {
                flood = false;
                floodDelay = 0;
            }
        }

        else
        {
            if (floodDelay < 2.5f)
            {
                floodDelay += Time.deltaTime;
            }
            else
            {
                flood = true;
                floodDelay = 0.0f;
                floodSwitch = !floodSwitch;
            }
        }
    }

    public void MakeShip()
    {
        Vector3 position = new Vector3(0, -50.6f, 94.1f);
        Ship ship = Instantiate(theShip) as Ship;
        ship.transform.position = position;
        ship.gameObject.layer = 12;
        ship.health = 5;
        ship.bossHud = theHud;
        ship.bossFight = true;
        ship.bullet = bullet;
        ship.tier = 5;
        ship.pilot = new UserPilot(30, cam, playerMaterial, true, soundManager);
        ship.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = playerMaterial;
        ship.bullet = bullet;
        ship.material = playerMaterial;
        ship.hitMaterial = playerHitMaterial;
        ship.GetComponent<Ship>().bulletPool = bulletPool;
        ship.secondaryMaterial = secondaryMaterial;
        ship.secondaryHitMaterial = secondaryHitMaterial;
        playerShip = ship;
    }

    private IEnumerator changePhase()
    {
        theBoss.gameObject.layer = 21;
        theBoss.gameObject.transform.GetComponent<Renderer>().material = purpleMaterial;
        yield return new WaitForSeconds(1.95f);
        phaseShifted = true;
        angleOfBullets1 = 0;
        angleOfBullets2 = 90;
        angleOfBullets3 = 180;
        angleOfBullets4 = 270;
    }
}
