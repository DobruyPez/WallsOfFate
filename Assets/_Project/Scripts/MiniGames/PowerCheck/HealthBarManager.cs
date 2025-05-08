using UnityEngine;
using UnityEngine.UI;
using Zenject.SpaceFighter;

public class HealthBarManager : MonoBehaviour
{
    // ������� �������� (Slider), ����������� ����� SetHealthBar
    private Slider _healthBar;
    // ��������� MiniGamePlayer �� ���� �� �������
    private MiniGamePlayer _player;

    /// <summary>
    /// ��������� ������� �������� (���������� �� MiniGameInstaller).
    /// </summary>
    /// <param name="healthBar">Slider ��� ����������� ��������.</param>
    public void SetHealthBar(Slider healthBar)
    {
        _healthBar = healthBar;
    }

    void Start()
    {
        // ���������� ������� ��������, ���� ������ ������� � _healthBar ��������
        if (this.gameObject.activeSelf && _healthBar != null)
        {
            _healthBar.gameObject.SetActive(true);
        }

        // �������� ��������� MiniGamePlayer
        _player = GetComponent<MiniGamePlayer>();
        if (_player == null)
        {
            Debug.LogError("��������� MiniGamePlayer �� ������!", this);
            return;
        }

        // ���������, ��������� �� ������� ��������
        if (_healthBar == null)
        {
            Debug.LogError("������� �������� �� ���������!", this);
            return;
        }

        // ��������� �������� � ������� ��� ������
        UpdateHealthBar();
        UpdatePortrait();
    }

    void OnDisable()
    {
        // ������������ ������� �������� ��� ���������� �������
        RemoveHealthBar();
    }

    void OnEnable()
    {
        // ���������� ������� �������� ��� ��������� �������
        AddHealthBar();
    }

    void Update()
    {
        // ���������� ������� ��������, ���� ������ ������� � _healthBar ��������
        if (this.gameObject.activeSelf && _healthBar != null)
        {
            _healthBar.gameObject.SetActive(true);
        }

        // ��������� ��������, ���� player � _healthBar �� null
        if (_player != null && _healthBar != null)
        {
            UpdateHealthBar();
        }
    }

    /// <summary>
    /// ��������� �������� ������� �������� � �����.
    /// </summary>
    private void UpdateHealthBar()
    {
        // ��������� ��������� �������� �������� � �������������
        float currentHealth = _player.Health;
        float maxHealth = _player.MaxHealth;
        _healthBar.value = currentHealth / maxHealth;

        // ��������� ����� �������� (��������, "50 / 100")
        Text healthBarText = _healthBar.GetComponentInChildren<Text>();
        if (healthBarText != null)
        {
            healthBarText.text = $"{Mathf.Ceil(currentHealth)} / {Mathf.Ceil(maxHealth)}";
        }

        // ��������� �������, ����� ������ ��������� ��������� player.Portrait
        UpdatePortrait();
    }

    /// <summary>
    /// ��������� � ��������� ������ �������� � �������� Image � ������ "image".
    /// </summary>
    private void UpdatePortrait()
    {
        // ���������, ��� player � _healthBar �� null
        if (_player == null || _healthBar == null)
        {
            return;
        }

        // ���� �������� ������ � ������ "image" � �������� _healthBar.transform
        Transform imageTransform = _healthBar.transform.Find("Image");
        if (imageTransform == null)
        {
            Debug.LogWarning("������ � ������ 'image' �� ������ ��� �������� ��������!", _healthBar);
            return;
        }

        // �������� ��������� Image � ���������� �������
        Image portraitImage = imageTransform.GetComponent<Image>();
        if (portraitImage == null)
        {
            Debug.LogWarning("��������� Image �� ������ �� ������� 'image' ��� �������� ��������!", imageTransform);
            return;
        }

        // ���������, ������ �� ���� � ��������
        if (string.IsNullOrEmpty(_player.Portrait))
        {
            Debug.LogWarning("player.Portrait ���� ��� null!", this);
            return;
        }

        // ��������� ������ �� Resources/PowerCheckPortraits
        Sprite portraitSprite = Resources.Load<Sprite>("PowerCheckPortraits/" + _player.Portrait);
        if (portraitSprite == null)
        {
            Debug.LogWarning($"�� ������� ��������� ������ �� ���� 'PowerCheckPortraits/{_player.Portrait}'!", this);
            return;
        }

        // ��������� ������, ���� ������� ����������
        if (portraitImage.sprite != portraitSprite)
        {
            portraitImage.sprite = portraitSprite;
        }
    }

    /// <summary>
    /// ������������ ������� ��������.
    /// </summary>
    public void RemoveHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ���������� ������� �������� � ��������� �������.
    /// </summary>
    public void AddHealthBar()
    {
        if (_healthBar == null)
        {
            Debug.LogWarning("������� �������� �� ��������� � AddHealthBar!", this);
            return;
        }

        _healthBar.gameObject.SetActive(true);
        UpdatePortrait();
    }
}