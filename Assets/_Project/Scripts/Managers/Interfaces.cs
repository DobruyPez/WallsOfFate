using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    void Trrigered();

}

// �������������� ��������� ��� �������� ���������
public interface ICheckableTrigger : ITriggerable
{
    bool IsDone { get; }
}