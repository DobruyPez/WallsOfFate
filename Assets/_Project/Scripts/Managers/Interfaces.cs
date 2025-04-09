using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    void Triggered();

}

// �������������� ��������� ��� �������� ���������
public interface ICheckableTrigger : ITriggerable
{
    bool IsDone { get; }
}