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
    protected virtual string DefaultText => "TEMP";

    private Aura aura;

    private bool isPlayerType;
    private bool identityConfirmed;
    private PlayerControllerB cachedPlayer;

    void Awake()
    {
        targetObject = GetComponent<T>();
        if (targetObject == null)
        {
            Debug.LogError($"{typeof(T).Name}_ESP: {typeof(T).Name} component is missing!");
            Destroy(this);
            return;
        }

        isPlayerType = targetObject is PlayerControllerB;
        if (isPlayerType) cachedPlayer = targetObject as PlayerControllerB;

        labelsCanvas = GameObject.Find(CanvasName)?.GetComponent<Canvas>();
        if (labelsCanvas == null)
        {
            CreateCanvas();
        }

        GameObject NameObject = new GameObject("NameObject");
        NameObject.transform.SetParent(labelsCanvas.transform);
        NameText = NameObject.AddComponent<Text>();
        NameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        NameText.fontSize = 13;
        NameText.alignment = TextAnchor.MiddleCenter;
        NameText.color = Color.white;
        NameText.text = DefaultText;

        PosRect = NameText.GetComponent<RectTransform>() ?? NameText.gameObject.AddComponent<RectTransform>();
        PosRect.localPosition = Vector3.zero;
        NameText.raycastTarget = false;
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject(CanvasName);
        labelsCanvas = canvasObject.AddComponent<Canvas>();
        labelsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        labelsCanvas.sortingOrder = 0;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        RectTransform rt = labelsCanvas.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
       

        if (targetObject == null)
        {
            Destroy(gameObject);
            return;
        }

        mainCamera = GameNetworkManager.Instance.localPlayerController?.gameplayCamera;

        if (ShouldShowLabel() && mainCamera != null)
        {
            // 2. Try to calculate screen position
            if (WorldToScreen(mainCamera, GetWorldPos(), out Vector3 screenPos))
            {
                // Only enable and move if the object is in front of the camera (screenPos.z > 0)
                NameText.enabled = true;
                UpdateText();
                PosRect.position = screenPos;
            }
            else
            {
                // Object is behind the camera or out of bounds
                NameText.enabled = false;
            }
        }
        else
        {
            NameText.enabled = false;
        }

        if (isPlayerType && !identityConfirmed)
        {
            if (cachedPlayer.isPlayerControlled)
            {
                if (cachedPlayer == GameNetworkManager.Instance.localPlayerController || cachedPlayer.IsLocalPlayer)
                {
                    Destroy(this);
                    return;
                }
                identityConfirmed = true;
            }
            else return;
        }
        if (ShouldShowAuras())
        {
            if (aura == null)
            {
                aura = targetObject.gameObject.AddComponent<Aura>();
                aura.Initialize(targetObject);
            }
            aura.auraColor = GetauraColor();
        }
        else
        {
            if (aura != null)
            {
                Destroy(aura);
                aura = null;
            }
        }
    }

    protected virtual void UpdateText()
    {
        NameText.fontSize = 12;
        NameText.text = GetEntityLabel();
        NameText.fontStyle = FontStyle.Normal;
        NameText.color = TextColor;
    }

    protected virtual string GetEntityLabel() => targetObject.name;
    protected virtual Vector3 GetWorldPos() => targetObject.transform.position;

    protected virtual bool ShouldShowLabel() => false;
    protected virtual bool ShouldShowAuras() => false;
    protected virtual Color GetauraColor() => Color.white;

public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
{
    Vector3 viewportPos = camera.WorldToViewportPoint(world);
    screen = viewportPos;

    // Check if the object is in front of the camera
    if (viewportPos.z <= 0) return false;

    // Check if the object is within the viewport bounds (0 to 1 range)
    if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1) return false;

    // Convert viewport (0-1) to screen pixels
    screen.x *= Screen.width;
    screen.y *= Screen.height;
    return true;
}

    void OnDestroy()
    {
        if (NameText != null)
        {
            Destroy(NameText.gameObject);
        }
        if (aura != null)
        {
            Destroy(aura);
            aura = null;
        }
    }
}