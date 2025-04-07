using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Slider healthBarPrefab;  // ������ ������� ��������
    public Transform healthBarParent;  // ������������ ������ ��� ���� ������� �������� �� Canvas

    private Slider healthBar;
    private MiniGamePlayer player;  // ������ �� ��������� Player

    void Start()
    {
        // �������� ��������� Player � �������
        player = GetComponent<MiniGamePlayer>();
        if (player == null)
        {
            Debug.LogError("Player component not found!");
            return;
        }

        // ������� ������� ��������
        healthBar = Instantiate(healthBarPrefab, healthBarParent);
        healthBar.gameObject.SetActive(true);

        // �������������� ������� ��������
        UpdateHealthBar();
    }

    void Update()
    {
        if (player != null)
        {
            // ��������� ������� �������� �� ������ �����
            UpdateHealthBar();
            if (player.Health == 0)
            {
                RemoveHealthBar(); // ������� ������� ��������, ���� �������� = 0
                this.gameObject.SetActive(false); // ������������ ������� ������ (�� ������� ����� ������)
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null && player != null)
        {
            // �������� ������� �������� � ������������ �������� ������
            float currentHealth = player.Health;
            float maxHealth = player.MaxHealth;

            // ��������� �������� ��������
            healthBar.value = currentHealth / maxHealth;

            // ��������� ����� (���� ����) ��� ����������� ����� ��������
            Text healthBarText = healthBar.GetComponentInChildren<Text>();
            if (healthBarText != null)
            {
                healthBarText.text = $"{Mathf.Ceil(currentHealth)} / {Mathf.Ceil(maxHealth)}";
            }
        }
    }

    public void RemoveHealthBar()
    {
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
    }
}
