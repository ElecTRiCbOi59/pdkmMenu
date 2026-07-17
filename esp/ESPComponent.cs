using GameNetcodeStuff;
using pdkmMenu;
using UnityEngine;
using UnityEngine.UI;

public class ESPComponent<T> : MonoBehaviour where T : MonoBehaviour
{
    protected T targetObject;
    protected Text NameText;
    protected Camera mainCamera;
    protected Canvas labelsCanvas;
    protected RectTransform PosRect;

    protected virtual Color TextColor => Color.white;
    protected virtual string CanvasName => typeof(T).Name + "_LabelCanvas";

    private const float ContentUpdateInterval = 0.1f;

    private Aura aura;
    private float nextContentUpdateTime;

    private bool isPlayerType;
    private bool identityConfirmed;
    private PlayerControllerB cachedPlayer;

    private void Awake()
    {
        targetObject = GetComponent<T>();

        if (targetObject == null)
        {
            Debug.LogError(
                $"{typeof(T).Name}_ESP: {typeof(T).Name} component is missing!"
            );

            Destroy(this);
            return;
        }

        isPlayerType = targetObject is PlayerControllerB;

        if (isPlayerType)
        {
            cachedPlayer = targetObject as PlayerControllerB;
        }

        nextContentUpdateTime =
            Time.unscaledTime + Random.Range(0f, ContentUpdateInterval);
    }

    private void Update()
    {
        if (targetObject == null)
        {
            Destroy(this);
            return;
        }

        ESPConfig settings = Plugin.ESPSettings;

        if (settings == null || !settings.ESP.Value)
        {
            HideLabel();
            RemoveAura();
            return;
        }

        PlayerControllerB localPlayer =
            GameNetworkManager.Instance?.localPlayerController;

        if (!UpdatePlayerIdentity(localPlayer))
        {
            return;
        }

        if (Time.unscaledTime < nextContentUpdateTime)
        {
            return;
        }

        nextContentUpdateTime =
            Time.unscaledTime + ContentUpdateInterval;

        UpdateLabelContent();
        UpdateAura();
    }

    private void LateUpdate()
    {
        if (targetObject == null)
        {
            return;
        }

        ESPConfig settings = Plugin.ESPSettings;

        if (settings == null || !settings.ESP.Value)
        {
            HideLabel();
            return;
        }

        PlayerControllerB localPlayer =
            GameNetworkManager.Instance?.localPlayerController;

        mainCamera = localPlayer?.gameplayCamera;

        UpdateLabelPosition();
    }

    private bool UpdatePlayerIdentity(PlayerControllerB localPlayer)
    {
        if (!isPlayerType || identityConfirmed)
        {
            return true;
        }

        if (cachedPlayer == null || !cachedPlayer.isPlayerControlled)
        {
            return false;
        }

        if (cachedPlayer == localPlayer || cachedPlayer.IsLocalPlayer)
        {
            Destroy(this);
            return false;
        }

        identityConfirmed = true;
        return true;
    }

    private void UpdateLabelPosition()
    {
        if (!ShouldShowLabel() || mainCamera == null)
        {
            HideLabel();
            return;
        }

        EnsureLabelExists();

        if (!WorldToScreen(
            mainCamera,
            GetWorldPos(),
            out Vector3 screenPosition
        ))
        {
            HideLabel();
            return;
        }

        NameText.enabled = true;
        PosRect.position = screenPosition;
    }

    private void UpdateLabelContent()
    {
        if (
            NameText == null ||
            !NameText.enabled ||
            !ShouldShowLabel()
        )
        {
            return;
        }

        UpdateText();
    }

    private void UpdateAura()
    {
        if (!ShouldShowAuras())
        {
            RemoveAura();
            return;
        }

        if (aura == null)
        {
            aura = targetObject.gameObject.AddComponent<Aura>();
            aura.Initialize(targetObject);
        }

        aura.auraColor = GetauraColor();
    }

    private void RemoveAura()
    {
        if (aura == null)
        {
            return;
        }

        Destroy(aura);
        aura = null;
    }

    private void EnsureLabelExists()
    {
        if (NameText != null)
        {
            return;
        }

        EnsureCanvasExists();

        GameObject nameObject =
            new GameObject($"{typeof(T).Name}_ESPLabel");

        nameObject.transform.SetParent(labelsCanvas.transform, false);

        NameText = nameObject.AddComponent<Text>();
        NameText.font =
            Resources.GetBuiltinResource<Font>("Arial.ttf");
        NameText.fontSize = 12;
        NameText.alignment = TextAnchor.MiddleCenter;
        NameText.color = TextColor;
        NameText.text = GetEntityLabel();
        NameText.raycastTarget = false;

        PosRect = NameText.rectTransform;
        PosRect.localPosition = Vector3.zero;
    }

    private void EnsureCanvasExists()
    {
        if (labelsCanvas != null)
        {
            return;
        }

        labelsCanvas =
            GameObject.Find(CanvasName)?.GetComponent<Canvas>();

        if (labelsCanvas == null)
        {
            CreateCanvas();
        }
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject(CanvasName);

        labelsCanvas = canvasObject.AddComponent<Canvas>();
        labelsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        labelsCanvas.sortingOrder = 0;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform =
            labelsCanvas.GetComponent<RectTransform>();

        rectTransform.sizeDelta =
            new Vector2(Screen.width, Screen.height);
    }

    private void HideLabel()
    {
        if (NameText != null)
        {
            NameText.enabled = false;
        }
    }

    protected virtual void UpdateText()
    {
        NameText.fontSize = 12;
        NameText.text = GetEntityLabel();
        NameText.fontStyle = FontStyle.Normal;
        NameText.color = TextColor;
    }

    protected virtual string GetEntityLabel()
    {
        return targetObject.name;
    }

    protected virtual Vector3 GetWorldPos()
    {
        return targetObject.transform.position;
    }

    protected virtual bool ShouldShowLabel()
    {
        return false;
    }

    protected virtual bool ShouldShowAuras()
    {
        return false;
    }

    protected virtual Color GetauraColor()
    {
        return Color.white;
    }

    public static bool WorldToScreen(
        Camera camera,
        Vector3 worldPosition,
        out Vector3 screenPosition
    )
    {
        Vector3 viewportPosition =
            camera.WorldToViewportPoint(worldPosition);

        screenPosition = viewportPosition;

        if (viewportPosition.z <= 0f)
        {
            return false;
        }

        if (
            viewportPosition.x < 0f ||
            viewportPosition.x > 1f ||
            viewportPosition.y < 0f ||
            viewportPosition.y > 1f
        )
        {
            return false;
        }

        screenPosition.x *= Screen.width;
        screenPosition.y *= Screen.height;

        return true;
    }

    private void OnDestroy()
    {
        if (NameText != null)
        {
            Destroy(NameText.gameObject);
            NameText = null;
        }

        RemoveAura();
    }
}