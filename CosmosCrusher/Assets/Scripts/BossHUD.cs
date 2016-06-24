using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour {

    public GameObject enemyHud;
    public Sprite red;
    public Sprite blue;
    public Sprite green;

    public Image bossHealthBack;
    public Image bossHealthFront;
    public Image greenColorIndicator;
    public Image blueColorIndicator;
    public Image heartIndicator;

    public Image bossHealthSeperator;
    public GameObject leftEnd;

    public Sprite emptyHeart;
    //public Sprite fourthHeart;
    //public Sprite halfHeart;
    //public Sprite threeFourthHeart;
    public Sprite fullHeart;

    public Sprite greenShip;
    public Sprite blueShip;

    public Image shipImage;

    public Image[] hearts = new Image[5];

    private int enemyMaxShield = 100;
    private int enemyMaxHealth = 100;
    private int currentEnemyHealth;
    private int currentEnemyShield;

    private int currentPlayerHealth = 5;
    private bool currentlyFlashing = true;

    private RectTransform seperatorTransform;
    private Vector2 leftEndPosition;
    private Vector2 rightEndPosition;

    // Use this for initialization
	void Start () {
        seperatorTransform = bossHealthSeperator.GetComponent<RectTransform>();
        leftEndPosition = leftEnd.GetComponent<RectTransform>().position;
        rightEndPosition = seperatorTransform.position;
        StartCoroutine(startFading());
	}
	
	// Update is called once per frame
	void Update () {
        greenColorIndicator.enabled = true;
        greenColorIndicator.canvasRenderer.SetAlpha(0.0f);
        blueColorIndicator.enabled = true;
        blueColorIndicator.canvasRenderer.SetAlpha(0.0f);
        heartIndicator.enabled = true;
        heartIndicator.canvasRenderer.SetAlpha(0.0f);
	}

    public void setBossStats(int maxHealth, int maxShield)
    {
        enemyMaxHealth = maxHealth;
        currentEnemyHealth = maxHealth;
        enemyMaxShield = maxShield;
        currentEnemyShield = maxShield;
    }

    public void hitEnemy(int damage)
    {
        if (currentEnemyShield > 0)
        {
            currentEnemyShield -= damage;
            currentEnemyShield = Mathf.Clamp(currentEnemyShield, 0, enemyMaxShield);
            if (currentEnemyShield < 1)
            {
                bossHealthBack.sprite = red;
                bossHealthFront.fillAmount = 1.0f;
                bossHealthFront.sprite = green;
                seperatorTransform.position = leftEndPosition;
            }
            else
            {
                float shieldLeft = (float)currentEnemyShield / (float)enemyMaxShield;
                bossHealthFront.fillAmount = shieldLeft;
                Vector2 lerpedPosition = Vector2.Lerp(leftEndPosition, rightEndPosition, shieldLeft);
                seperatorTransform.position = lerpedPosition;
            }
        }
        else if (currentEnemyHealth > 0)
        {
            currentEnemyHealth -= damage;
            currentEnemyHealth = Mathf.Clamp(currentEnemyHealth, 0, enemyMaxHealth);
            if (currentEnemyHealth == 0)
            {
                Destroy(enemyHud);
            }
            else
            {
                float healthLeft = (float)currentEnemyHealth / (float)enemyMaxHealth;
                bossHealthFront.fillAmount = healthLeft;
                Vector2 lerpedPosition = Vector2.Lerp(leftEndPosition, rightEndPosition, healthLeft);
                seperatorTransform.position = lerpedPosition;
            }            
        }
    }

    public void flipColor()
    {
        currentlyFlashing = false;
        if (shipImage.sprite == blueShip)
        {
            shipImage.sprite = greenShip;
            fadeIndicator(greenColorIndicator);
        }
        else
        {
            shipImage.sprite = blueShip;
            fadeIndicator(blueColorIndicator);
        }
    }

    public void checkHealth(int totalHealth)
    {
        int i = 0;
        while (i < totalHealth)
        {
            hearts[i].sprite = fullHeart;
            ++i;
        }
        while (i < 5)
        {
            hearts[i].sprite = emptyHeart;
            ++i;
        }
    }

    public void lostHealth(int damage)
    {
        int formerHealth = currentPlayerHealth;
        currentPlayerHealth -= damage;
        currentPlayerHealth = Mathf.Clamp(currentPlayerHealth, 0, 5);
        for (int i = currentPlayerHealth + 1; i <= formerHealth; ++i)
        {
            hearts[i - 1].sprite = emptyHeart;
        }
        fadeIndicator(heartIndicator);
    }

    private IEnumerator startFading()
    {
        fadeIndicator(blueColorIndicator);
        yield return new WaitForSeconds(0.75f);
        if (currentlyFlashing)
        {
            StartCoroutine(startFading());
        }
    }

    private void fadeIndicator(Image indicator)
    {
        indicator.canvasRenderer.SetAlpha(1.0f);
        indicator.CrossFadeAlpha(0.0f, 0.75f, false);
    }
}
