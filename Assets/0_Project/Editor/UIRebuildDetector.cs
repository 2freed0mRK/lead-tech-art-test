#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIRebuildDetector : MonoBehaviour
{
    private IList<ICanvasElement> m_LayoutRebuildQueue;
    private IList<ICanvasElement> m_GraphicRebuildQueue;
    private bool m_Initialized;

    [Header("Настройки логирования")]
    [SerializeField] private bool logLayoutRebuilds = true;
    [SerializeField] private bool logGraphicRebuilds = true;

    private void OnEnable()
    {
        InitializeReflection();
    }

    private void InitializeReflection()
    {
        try
        {
            Type registryType = typeof(CanvasUpdateRegistry);
            
            FieldInfo layoutField = registryType.GetField("m_LayoutRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo graphicField = registryType.GetField("m_GraphicRebuildQueue", BindingFlags.NonPublic | BindingFlags.Instance);

            if (layoutField != null && graphicField != null)
            {
                m_LayoutRebuildQueue = layoutField.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;
                m_GraphicRebuildQueue = graphicField.GetValue(CanvasUpdateRegistry.instance) as IList<ICanvasElement>;
                
                if (m_LayoutRebuildQueue != null && m_GraphicRebuildQueue != null)
                {
                    m_Initialized = true;
                    Debug.Log("<color=#00FF00><b>[UIRebuildDetector]</b> Успешно подключен!</color>");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[UIRebuildDetector] Ошибка: {e.Message}");
        }
    }

    private void LateUpdate()
    {
        if (!m_Initialized || !Application.isPlaying) return;

        CheckAndLogQueue(m_LayoutRebuildQueue, "Layout Rebuild", logLayoutRebuilds, "#FF5555");
        CheckAndLogQueue(m_GraphicRebuildQueue, "Graphic Rebuild", logGraphicRebuilds, "#55FF55");
    }

    private void CheckAndLogQueue(IList<ICanvasElement> queue, string rebuildType, bool isEnabled, string colorHex)
    {
        if (!isEnabled || queue == null || queue.Count == 0) return;

        for (int i = 0; i < queue.Count; i++)
        {
            var element = queue[i];
            
            if (element != null && !element.IsDestroyed() && element.transform != null)
            {
                GameObject targetGo = element.transform.gameObject;
                string path = GetHierarchyPath(element.transform);
                
                string message = $"<color={colorHex}><b>[{rebuildType}]</b></color> Вызван объектом: <b>{targetGo.name}</b>\n<color=#888888>Путь: {path}</color>";
                
                Debug.Log(message, targetGo);
            }
        }
    }

    private string GetHierarchyPath(Transform transform)
    {
        string path = transform.name;
        // Исправленный блок: сначала добавляем имя родителя к пути, потом сдвигаем transform
        while (transform.parent != null)
        {
            path = transform.parent.name + "/" + path;
            transform = transform.parent;
        }
        return path;
    }
}
#endif