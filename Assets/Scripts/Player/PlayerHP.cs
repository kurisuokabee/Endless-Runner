
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHP : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float damagePerHit = 5f;
    [SerializeField] private float regenerationRate = 1f;

    [Header("UI")]
    [SerializeField] private PlayerHPUI hpUI;

    private float regenTimer;
    public float CurrentHP { get; private set; }
    public float MaxHP => maxHP;
    CameraMovement cameraMovement;

    private void Start()
    {   
        cameraMovement = Camera.main.GetComponent<CameraMovement>();

        CurrentHP = maxHP;

        hpUI.UpdateHP();
        hpUI.UpdateGhostHP();
    }

    private void Update()
    {
        RegenerateHealth();

        if (Keyboard.current != null &&
            Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        CurrentHP = Mathf.Max(
            0f,
            CurrentHP - damagePerHit
        );

        hpUI.UpdateHP();
        hpUI.UpdateGhostHP();

        if (cameraMovement != null)
        {
            cameraMovement.TriggerShake(0.1f, 0.2f);
        }

        if (CurrentHP <= 0f)
        {
            Debug.Log("Scene Restart");
            SceneManager.LoadScene("Gameplay");
        }
    }

    


    private void RegenerateHealth()
    {
        if (CurrentHP >= maxHP)
        {
            regenTimer = 0f;
            return;
        }

        regenTimer += Time.deltaTime;

        if (regenTimer >= 1f)
        {
            CurrentHP = Mathf.Min(maxHP, CurrentHP + regenerationRate);

            regenTimer -= 1f;

            hpUI.UpdateHP();
            hpUI.UpdateGhostHP();
        }
    }

}
