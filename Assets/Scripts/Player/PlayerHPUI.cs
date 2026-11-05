using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider ghostBar;

    [Header("Ghost HP Settings")]
    [SerializeField] private float ghostDelay = 0.5f;
    [SerializeField] private float ghostDuration = 0.5f;

    private float ghostHP;
    private float ghostStartHP;
    private float ghostTargetHP;

    private float ghostTimer;
    private float ghostElapsed;

    private bool isAnimating;

    private void Start()
    {
        hpBar.minValue = 0f;
        hpBar.maxValue = playerHP.MaxHP;
        hpBar.value = playerHP.CurrentHP;

        ghostBar.minValue = 0f;
        ghostBar.maxValue = playerHP.MaxHP;
        ghostBar.value = playerHP.CurrentHP;

        ghostHP = playerHP.CurrentHP;
    }

    private void Update()
    {
        float currentHP = playerHP.CurrentHP;

        // Check if HP changed
        if (!isAnimating && ghostHP != currentHP)
        {
            StartGhostAnimation(currentHP);
        }

        // Animate ghost HP
        if (isAnimating)
        {
            ghostTimer += Time.deltaTime;

            if (ghostTimer >= ghostDelay)
            {
                ghostElapsed += Time.deltaTime;

                float t = Mathf.Clamp01(
                    ghostElapsed / ghostDuration
                );

                ghostHP = Mathf.Lerp(
                    ghostStartHP,
                    ghostTargetHP,
                    t
                );

                ghostBar.value = ghostHP;

                if (t >= 1f)
                {
                    ghostHP = ghostTargetHP;
                    ghostBar.value = ghostHP;
                    isAnimating = false;
                }
            }
        }
    }

    private void StartGhostAnimation(float targetHP)
    {
        ghostStartHP = ghostHP;
        ghostTargetHP = targetHP;

        ghostTimer = 0f;
        ghostElapsed = 0f;

        isAnimating = true;
    }

    public void UpdateGhostHP()
    {
        StartGhostAnimation(playerHP.CurrentHP);
    }

    public void UpdateHP()
    {
        hpBar.value = playerHP.CurrentHP;
    }
}
