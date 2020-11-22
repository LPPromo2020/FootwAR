using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/**
 * Romuald
 */

/// <summary>
///
/// </summary>
public class NotificationsManager : Singleton<NotificationsManager>
{
    [Header("Display Elements for Notifications")]
    [SerializeField] private Text m_tName;
    [SerializeField] private Text m_tMessage;

    [Header("Components")]
    [SerializeField] private RectTransform m_rtPosition;
    [SerializeField] private Button m_bClose;
    [SerializeField] private CanvasScaler m_csCanvasScaler;

    [Header("Settings")]
    [Range(0.5f, 3f)]
    [SerializeField] private float m_fTimeBetweenNotifications = 1f;
    [Range(0f, 5f)]
    [SerializeField] private float m_fTimeAnimationOnSeconde = 0.2f;
    [Range(0f, 100f)]
    [SerializeField] private float m_fPercentageWidthScreen = 10f;
    [Range(0f, 100f)]
    [SerializeField] private float m_fPercentageHeightScreen = 10f;
    [Range(0, 20)]
    [SerializeField] private int m_iOffSetPosition;
    [Range(0f, 100f)]
    [SerializeField] private float m_fTitleSpaceTake = 30f;

    private Queue<Notification> m_queueNotifications = new Queue<Notification>();
    private Coroutine m_cDisplayNotification;

    private void Start()
    {
        m_csCanvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        float width = Screen.width * m_fPercentageWidthScreen / 100,
            height = Screen.height * m_fPercentageHeightScreen / 100;
        m_rtPosition.sizeDelta = new Vector2(width, height);
        m_rtPosition.localPosition = new Vector3(Screen.width / 2 - width / 2 - m_iOffSetPosition, Screen.height / 2 - height / 2 - m_iOffSetPosition, 0);

        RectTransform title = m_tName.rectTransform;
        title.sizeDelta = new Vector2(width, height * (m_fTitleSpaceTake / 100));
        Debug.Log($"Position: {title.localPosition}");
        title.localPosition = new Vector3(0, height / 2 - title.sizeDelta.y / 2, 0);
        Debug.Log($"Position: {title.localPosition}");

        RectTransform message = m_tMessage.rectTransform;
        message.sizeDelta = new Vector2(width, height * ((100 - m_fTitleSpaceTake) / 100));
        Debug.Log($"Position: {message.localPosition}");
        message.localPosition = new Vector3(0, -(height / 2 - message.sizeDelta.y / 2), 0);
        Debug.Log($"Position: {message.localPosition}");

        m_bClose.onClick.AddListener(CloseNotification);
    }

    public void AddNotification(string n, string message, float duration = 1f)
    {
        m_queueNotifications.Enqueue(new Notification(n, message, duration));
        if (m_cDisplayNotification != null) return;

        m_cDisplayNotification = StartCoroutine(DisplayNotification(m_queueNotifications.Dequeue(), 0));
    }

    private IEnumerator DisplayNotification(Notification notif, float time)
    {
        while ((time -= Time.deltaTime) > 0) yield return null;

        m_tName.text = notif.Name;
        m_tMessage.text = notif.Message;

        LeanTween.scale(m_rtPosition, Vector3.one, m_fTimeAnimationOnSeconde);
        yield return new WaitForSeconds(m_fTimeAnimationOnSeconde + notif.Duration);

        CloseNotification();
    }

    private void CloseNotification()
    {
        if (m_cDisplayNotification != null)
        {
            StopCoroutine(m_cDisplayNotification);
            m_cDisplayNotification = null;
        }

        LeanTween.scale(m_rtPosition, Vector3.zero, m_fTimeAnimationOnSeconde).setOnComplete(() => {
            if (m_queueNotifications.Count == 0) return;

            m_cDisplayNotification = StartCoroutine(DisplayNotification(m_queueNotifications.Dequeue(), m_fTimeBetweenNotifications));
        });
    }

    private struct Notification
    {
        private string m_sName;
        private string m_sMessage;
        private float m_fDuration;

        public string Name => m_sName;
        public string Message => m_sMessage;
        public float Duration => m_fDuration;

        public Notification(string n, string m, float duration)
        {
            m_sName = n;
            m_sMessage = m;
            m_fDuration = duration;
        }
    }
}
