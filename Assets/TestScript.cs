using UnityEditor;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject obj;

    void Start()
    {
        Texture2D preview = AssetPreview.GetMiniThumbnail(obj);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(preview, new Rect(0, 0, preview.width, preview.height), Vector2.zero);
    }
}
