using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>PreviewSystem</c> defines the system to handle visual preview system for floorplan construction.
/// </summary>
public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewOffset = 0.06f;
    
    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    /// <summary>
    /// Method handles logic for beginning preview on <c>PlacementState</c>.
    /// </summary>
    public void StartShowingPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview();
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    /// <summary>
    /// Method handles logic for beginning preview on <c>RemovingState</c>.
    /// </summary>
    public void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    private void PreparePreview()
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    private void PrepareCursor(Vector2Int size)
    {
        cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
        cellIndicatorRenderer.material.mainTextureScale = size;
    }

    /// <summary>
    /// Method handles logic for ending preview.
    /// </summary>
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject == null) { return; }
        Destroy(previewObject);
    }

    /// <summary>
    /// Helper method handles logic for updating preview object position.
    /// </summary>
    public void UpdatePosition(Vector3 position, bool validity)
    {
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    /// <summary>
    /// Method handles logic for rotating preview object.
    /// </summary>
    public void RotatePreview(Vector2Int cursorSize)
    {
        previewObject.transform.Rotate(0, 90, 0);
        PrepareCursor(cursorSize);
    }

    /// <summary>
    /// Method handles logic for scaling preview object.
    /// </summary>
    public void ScalePreview(Vector2Int cursorSize, Vector2Int scale)
    {
        Vector3 newScale = previewObject.transform.localScale;
        newScale.x = (scale.x > 1 || newScale.x > 1) ? scale.x : newScale.x;
        newScale.z = (scale.y > 1 || newScale.z > 1) ? scale.y : newScale.z;
        previewObject.transform.localScale = newScale;

        PrepareCursor(cursorSize);
    }

    /// <summary>
    /// Method handles logic for updating preview.
    /// </summary>
    public void UpdatePreview(Vector3 objectPosition, Vector3 cursorPosition, bool validity, Color previewColor)
    {
        UpdatePosition(cursorPosition, validity);
        if (previewObject == null) { return; }
        MovePreview(objectPosition);
        ApplyFeedbackToPreview(validity, previewColor);
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewOffset, position.z);
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void ApplyFeedbackToPreview(bool valid, Color previewColor)
    {
        Color c = valid ? previewColor : Color.red;
        c.a = 0.9f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool valid)
    {
        cellIndicatorRenderer.material.color = valid ? Color.white : Color.red;
    }

}
