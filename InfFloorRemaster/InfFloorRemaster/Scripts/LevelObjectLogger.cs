using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class LevelObjectLogger
{
    public void CreateReadableLevelObjectFile(LevelObject level)
    {
        if (level == null)
        {
            Debug.LogError("LevelObject is null!");
            return;
        }

        // Путь к папке с игрой
        string gameFolderPath = Application.dataPath;
        string parentFolderPath = Directory.GetParent(gameFolderPath).FullName;
        string logFilePath = Path.Combine(parentFolderPath, "LevelObject_Log.log");

        // Собираем информацию о всех публичных полях
        string logContent = GenerateLogContent(level);

        // Записываем в файл
        File.WriteAllText(logFilePath, logContent);
        Debug.Log($"LevelObject log saved to: {logFilePath}");
    }

    private string GenerateLogContent(LevelObject level)
    {
        string logContent = $"=== LevelObject Log: {level.name} ===\n";
        logContent += $"Generated at: {DateTime.Now}\n\n";

        // Получаем все публичные поля класса LevelObject
        FieldInfo[] fields = typeof(LevelObject).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            try
            {
                object fieldValue = field.GetValue(level);
                logContent += $"{field.Name}: {FormatFieldValue(fieldValue)}\n";
            }
            catch (Exception ex)
            {
                logContent += $"{field.Name}: [Error reading value: {ex.Message}]\n";
            }
        }

        return logContent;
    }

    private string FormatFieldValue(object value)
    {
        if (value == null)
        {
            return "null";
        }

        // Обработка массивов и списков
        if (value is Array array)
        {
            return $"[Array: Length={array.Length}]";
        }
        else if (value is System.Collections.IList list)
        {
            return $"[List: Count={list.Count}]";
        }
        // Обработка Unity-типов
        else if (value is Vector2 vec2)
        {
            return $"({vec2.x}, {vec2.y})";
        }
        else if (value is Vector3 vec3)
        {
            return $"({vec3.x}, {vec3.y}, {vec3.z})";
        }
        else if (value is Color color)
        {
            return $"RGBA({color.r}, {color.g}, {color.b}, {color.a})";
        }
        else if (value is IntVector2 intVec2)
        {
            return $"({intVec2.x}, {intVec2.z})";
        }
        // Обработка перечислений
        else if (value is Enum enumValue)
        {
            return enumValue.ToString();
        }
        // Обработка стандартных типов
        else if (value is string || value.GetType().IsPrimitive)
        {
            return value.ToString();
        }
        // Для сложных объектов
        else
        {
            return value.ToString();
        }
    }
}
