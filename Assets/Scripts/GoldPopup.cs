using TMPro;
using UnityEngine;

public class GoldPopup : MonoBehaviour
{
    private const float Lifetime = 0.65f;
    private const float RiseDistance = 0.45f;
    private const float VerticalOffset = 0.2f;
    private const float WorldScale = 0.45f;

    private TextMeshPro textMesh;
    private Vector3 startPosition;
    private Color baseColor;
    private float lifetimeTimer;

    public static void Spawn(Vector3 position, int amount, int sortingLayerId, int sortingOrder)
    {
        GameObject popupObject = new GameObject("GoldPopup");
        popupObject.transform.position = position + Vector3.up * VerticalOffset;

        TextMeshPro popupText = popupObject.AddComponent<TextMeshPro>();
        GoldPopup popup = popupObject.AddComponent<GoldPopup>();
        popup.Initialize(popupText, amount, sortingLayerId, sortingOrder);
    }

    private void Initialize(TextMeshPro popupText, int amount, int sortingLayerId, int sortingOrder)
    {
        textMesh = popupText;
        startPosition = transform.position;
        baseColor = new Color(1f, 0.92f, 0.1f, 1f);

        textMesh.font = TMP_Settings.defaultFontAsset;
        textMesh.text = $"+{amount}";
        textMesh.fontSize = 5.5f;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = baseColor;
        textMesh.enableWordWrapping = false;
        textMesh.fontStyle = FontStyles.Bold;
        textMesh.transform.localScale = Vector3.one * WorldScale;

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerID = sortingLayerId;
            meshRenderer.sortingOrder = sortingOrder + 20;
        }
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;
        float normalizedLifetime = Mathf.Clamp01(lifetimeTimer / Lifetime);

        transform.position = startPosition + Vector3.up * (RiseDistance * normalizedLifetime);

        if (textMesh != null)
        {
            Color currentColor = baseColor;
            currentColor.a = 1f - normalizedLifetime;
            textMesh.color = currentColor;
        }

        if (lifetimeTimer >= Lifetime)
        {
            Destroy(gameObject);
        }
    }
}
