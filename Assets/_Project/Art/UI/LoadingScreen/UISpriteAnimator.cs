using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    [Tooltip("������ �������� ��� ��������")]
    public Sprite[] frames;

    [Tooltip("������� ����� ������ (������ � �������)")]
    public float framesPerSecond = 10f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("UISpriteAnimator: ��������� Image �� ������!");
        }
    }

    private void Update()
    {
        if (frames.Length == 0) return;

        // ��������� ������� ���� �� ������ �������
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        image.sprite = frames[index];
    }
}
