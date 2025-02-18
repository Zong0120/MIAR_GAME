using UnityEngine;

[ExecuteInEditMode] // 讓腳本可在編輯模式下執行
public class MapSorting : MonoBehaviour
{
    public Texture2D sourceTexture; // 原始 Texture，非 SpriteRenderer
    public int columns = 4; // 切割列數
    public int rows = 4; // 切割行數
    public float pixelsPerUnit = 10f; // PPU，需與原圖一致

    private void Start()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("請指定 Source Texture！");
            return;
        }

        // 自動載入 Multiple 切割後的 Sprites
        string textureName = sourceTexture.name;
        Sprite[] sprites = Resources.LoadAll<Sprite>(textureName);

        if (sprites.Length == 0)
        {
            Debug.LogError($"無法加載 {textureName} 內的切割 Sprites，請確保該圖片已設為 Multiple！");
            return;
        }

        if (sprites.Length != columns * rows)
        {
            Debug.LogError($"Sprite 數量 ({sprites.Length}) 與行列數 ({columns * rows}) 不匹配！");
            return;
        }

        // 清除舊的拼接物件
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // 計算每個 Tile 的大小（世界單位）
        float tileWidth = sourceTexture.width / columns / pixelsPerUnit;
        float tileHeight = sourceTexture.height / rows / pixelsPerUnit;

        // 生成拼接物件
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;
                if (index >= sprites.Length) continue;

                GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                tileObj.transform.SetParent(transform);

                SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
                sr.sprite = sprites[index];

                // 計算相對位置（左上角開始拼接）
                float posX = x * tileWidth - (columns * tileWidth) / 2 + tileWidth / 2;
                float posY = -(y * tileHeight) + (rows * tileHeight) / 2 - tileHeight / 2;
                tileObj.transform.localPosition = new Vector3(posX, posY, 0);
            }
        }

        Debug.Log($"拼接完成！共生成 {sprites.Length} 塊");
    }
}