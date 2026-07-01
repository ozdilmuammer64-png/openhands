using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float floatSpeed = 2f;
    public float lifetime = 1f;
    public Vector3 offset = new Vector3(0, 1, 0);

    private float lifetimeTimer;

    void Start()
    {
        lifetimeTimer = lifetime;
    }

    void Update()
    {
        lifetimeTimer -= Time.deltaTime;

        // Float up
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        if (damageText != null)
        {
            float alpha = lifetimeTimer / lifetime;
            damageText.alpha = alpha;
        }

        if (lifetimeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float damage, bool isCrit)
    {
        if (damageText != null)
        {
            damageText.text = Mathf.Floor(damage).ToString();
            
            if (isCrit)
            {
                damageText.color = new Color(1f, 0.4f, 0.4f);
                damageText.fontSize = 24;
                damageText.text += " CRIT!";
            }
            else
            {
                damageText.color = Color.white;
                damageText.fontSize = 18;
            }
        }
    }

    public void SetHeal(float healAmount)
    {
        if (damageText != null)
        {
            damageText.text = "+" + Mathf.Floor(healAmount).ToString();
            damageText.color = Color.green;
            damageText.fontSize = 20;
        }
    }
}