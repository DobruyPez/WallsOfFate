using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class Repository
{
    private const string GAME_STATE_FILE = "GameState.json";
    private const string USER_PROGRESS_KEY = "UserProgress"; // ����, ������������, ��� ������������ ������������� ����� ����

    private static Dictionary<string, string> currentState = new();

    private static string FilePath => Path.Combine(Application.persistentDataPath, GAME_STATE_FILE);

    public static void LoadState()
    {
        if (File.Exists(FilePath))
        {
            var serializedState = File.ReadAllText(FilePath);
            currentState = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedState)
                           ?? new Dictionary<string, string>();
        }
        else
        {
            currentState = new Dictionary<string, string>();
        }
    }

    public static void SaveState()
    {
        var serializedState = JsonConvert.SerializeObject(currentState, Formatting.Indented);
        File.WriteAllText(FilePath, serializedState);
    }

    public static T GetData<T>()
    {
        if (currentState.TryGetValue(typeof(T).Name, out var serializedData))
        {
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        throw new KeyNotFoundException($"Data for type {typeof(T).Name} not found.");
    }

    public static void SetData<T>(string key, T value)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All // ��������� ���������� � ����
        };
        var serializedData = JsonConvert.SerializeObject(value, settings);
        currentState[key] = serializedData;
    }

    public static bool TryGetData<T>(string key, out T value)
    {
        if (currentState.TryGetValue(key, out var serializedData))
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            value = JsonConvert.DeserializeObject<T>(serializedData, settings);
            return true;
        }

        value = default;
        return false;
    }

    // ��������� ����� �������� ������� ��������� ����������������� ����������.
    public static bool HasAnyData()
    {
        // ��������� ����, ���������� �� ������� ��������� ��������� ������������
        if (currentState.TryGetValue(USER_PROGRESS_KEY, out var progressData))
        {
            if (bool.TryParse(progressData, out bool hasProgress))
            {
                return hasProgress;
            }
        }
        return false;
    }

    // ����� ��� ��������� ����� ����������������� ���������.
    public static void SetUserProgress(bool progress)
    {
        currentState[USER_PROGRESS_KEY] = progress.ToString();
    }

    public static void ClearSaveData()
    {
        try
        {
            // ������� ������ � ������
            currentState.Clear();

            // ������� ���� ����������, ���� �� ����������
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log("Save file deleted successfully");
            }

            // ������� ����� ������ ������� ��� ����������� ��������
            currentState = new Dictionary<string, string>();
        }
        catch (IOException ex)
        {
            Debug.LogError($"Error clearing save data: {ex.Message}");
        }
    }
}
