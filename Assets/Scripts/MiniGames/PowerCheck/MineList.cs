using System.Collections.Generic;
using UnityEngine;

public class MineList
{
    // ����� ������ ���
    public int Length { get; private set; }
    // ������ ���
    public List<Mine> Minelist { get; private set; }

    public MineList(int length)
    {
        Length = length;
        Minelist = new List<Mine>();
    }

    // ������������� ������ � ������ ������������� ����
    public void InitializeMines(GameObject prefab, float cooldown, System.Func<uint, float, GameObject, Mine> createMine)
    {
        for (int i = 0; i < Length; i++)
        {
            uint number = (uint)i;
            GameObject mineGameObject = Object.Instantiate(prefab);
            Mine newMine = createMine(number, cooldown, mineGameObject);
            Minelist.Add(newMine);
        }
    }

    // ������������� ������ � ������ ������������� ����
    public void InitializeSpeedBuffMines(GameObject prefab, float cooldown, float speedbuff, float time)
    {
        for (int i = 0; i < Length; i++)
        {
            uint number = (uint)i;
            GameObject mineGameObject = Object.Instantiate(prefab);
            Mine newMine = new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, time);
            Minelist.Add(newMine);
        }
    }
}
