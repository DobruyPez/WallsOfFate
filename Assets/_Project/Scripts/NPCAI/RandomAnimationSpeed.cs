using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    // ����������� � ������������ �������� ��� ���������� ��������� ��������.
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // ��������� ��������� �������� �������� ��������.
            animator.speed = Random.Range(minSpeed, maxSpeed);
        }
    }
}
