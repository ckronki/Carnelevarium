using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Player player;          // referencia al Player en la escena
    public float health;           // vida actual
    public float maxHealth = 100;  // vida máxima

    [Header ("Interfaz")]
    public Image healthBar;
    public Text healthText;

    void Start()
    {
        
    }

    private void Update()
    {
        if (player != null)
        {
            health = player.CurrentLife;
        }

        updateInterface();
    }

    public void updateInterface()
    {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = health.ToString("f0");
    }
}
