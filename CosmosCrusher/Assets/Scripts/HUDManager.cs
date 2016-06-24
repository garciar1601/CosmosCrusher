using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private int previousHealth = 5;

    public Color farColor;
    public Color nearColor;
    public RawImage indicator;

    public GameObject trackerHolder;
    public Image nextTier;
    public Text tillNextTier;
    public Image nextTierIndicator;
    public Image heartIndicator;
    public Image currentTierIndicator;
    public Image trackerImage;

    public Image[] hearts = new Image[5];
    public Sprite[] tierSprites = new Sprite[4];
    public Sprite[] playerShips = new Sprite[4];
    public Image[] tierImages = new Image[4];

    private int untilNext;
    private Transform trackerTransform;

	// Use this for initialization
	void Start () {
        nextTierIndicator.enabled = true;
        nextTierIndicator.canvasRenderer.SetAlpha(0.0f);
        heartIndicator.enabled = true;
        heartIndicator.canvasRenderer.SetAlpha(0.0f);
        currentTierIndicator.enabled = true;
        currentTierIndicator.canvasRenderer.SetAlpha(0.0f);
        trackerTransform = trackerHolder.GetComponent<Transform>();
	}

	// Update is called once per frame
	void Update () {

	}

    public void setTillNextTier(int untilNext, int next)
    {
        this.untilNext = untilNext;
        tillNextTier.text = "Until Next Tier: " + untilNext;
        nextTier.sprite = tierSprites[next - 1];
        fadeIndicator(nextTierIndicator);
    }

    public void setFinalTilNextTier()
    {
        this.untilNext = untilNext + 1;
        tillNextTier.text = "Until Next Tier: " + 0;
        nextTier.enabled = false;
        fadeIndicator(nextTierIndicator);
    }

    private void fadeIndicator(Image indicator)
    {
        indicator.canvasRenderer.SetAlpha(1.0f);
        indicator.CrossFadeAlpha(0.0f, 0.75f, false);
    }

    public void checkHealth(int totalHealth) {
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
        if (totalHealth != previousHealth)
        {
            previousHealth = totalHealth;
            fadeIndicator(heartIndicator);
        }
    }

    public void enemyDestroyed()
    {
        --untilNext;
        this.tillNextTier.text = "Until Next Tier: " + untilNext;
    }

    public void setTier(int tier)
    {
        for (int i = 0; i < tierImages.Length; ++i)
        {
            tierImages[i].sprite = tierSprites[i];
        }
        int newPosition = tier - 1;
        tierImages[newPosition].sprite = playerShips[newPosition];
        fadeIndicator(currentTierIndicator);
    }

    private Vector3 projection(Vector3 first, Vector3 second)
    {
        float length = Vector3.Dot(first, second) / second.magnitude;
        return second.normalized * length;
    }

    public void updateIndicator(float beta)
    {
        Vector3 newColor = 1.5f * beta * new Vector3(farColor.r, farColor.g, farColor.b) + (1.0f - beta) * new Vector3(nearColor.r, nearColor.g, nearColor.b);
        Color indicatorColor = new Color(newColor.x, newColor.y, newColor.z);
        indicator.color = indicatorColor;
    }

    public void rotateTracker(Vector3 playerPosition, Vector3 enemyPosition, Vector3 normal, Vector3 right, Vector3 forward, Color color)
    {
        Vector3 theProjection = projection(enemyPosition, normal);
        Vector3 firstVector = theProjection - enemyPosition;
        float angle = Vector3.Angle(-right, firstVector);
        float dotResult = Vector3.Dot(firstVector, forward);
        angle = dotResult < 0.0f ? -angle : angle;
        Color imageColor = new Color(color.r * .85f, color.g * .85f, color.b * .85f);
        trackerImage.color = imageColor;
        trackerTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
