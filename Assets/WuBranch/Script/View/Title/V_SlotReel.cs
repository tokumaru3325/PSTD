using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class V_SlotReel : MonoBehaviour, ISlotReelView
{
    /// <summary>
    /// 生成する図の大きさ
    /// </summary>
    private Vector2 _imageSize;
    public Vector2 ImageSize => _imageSize;

    /// <summary>W
    /// 一ロールにあるの全部の図
    /// </summary>
    private List<RectTransform> _reelItems = new List<RectTransform>();

    /// <summary>
    /// 一ロールの高さ
    /// </summary>
    private float _totalHeight;

    /// <summary>
    /// この値より低くと一番高いところに移動(循環させる)
    /// </summary>
    private float _threshold;

    /// <summary>
    /// 停止時の高さ、背景によって必ず0ではないため
    /// </summary>
    private float _stopOffset;
    public float StopOffset => _stopOffset;

    /// <summary>
    /// 初期化、もらったテクスチャを使ってロールを生成
    /// </summary>
    /// <param name="size">画像サイズ</param>
    /// <param name="sprites">テクスチャ</param>
    /// <param name="offset">停止時の高さ</param>
    public void Initialize(Vector2 size, Sprite[] sprites, float offset)
    {
        // あるものをクリア
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        _reelItems.Clear();

        _imageSize = size;
        _stopOffset = offset;

        // テクスチャがない場合、何もしない
        if (sprites == null || sprites.Length == 0)
            return;

        // 高さを計算
        _totalHeight = sprites.Length * _imageSize.y;
        // 循環するための高さを計算
        _threshold = -(2 * _imageSize.y) + _stopOffset;

        // Imageを生成
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject obj = new GameObject($"Symbol_{i}");
            obj.transform.SetParent(transform, false);

            Image img = obj.AddComponent<Image>();
            img.sprite = sprites[i];

            RectTransform rt = obj.GetComponent<RectTransform>();
            // アンカーを先に設定してから位置を設置
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = _imageSize;
            rt.anchoredPosition = new Vector2(0, (i - 1) * _imageSize.y + _stopOffset);

            _reelItems.Add(rt);
        }
    }

    /// <summary>
    /// スクロール
    /// </summary>
    /// <param name="distance">移動距離</param>
    public void MoveReel(float distance)
    {
        foreach (var rt in _reelItems)
        {
            // 下に移動
            Vector2 pos = rt.anchoredPosition;
            pos.y -= distance;

            if (pos.y <= _threshold)
            {
                pos.y += _totalHeight;
            }

            rt.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// 結果となった画像を見つける
    /// </summary>
    /// <returns>画像のインデックス</returns>
    public int FindTarget()
    {
        int max = _reelItems.Count;
        for (int index = 0; index < max; index++)
        {
            if ((int)GetItemY(index) == _stopOffset)
                return index;
        }
        return -1;
    }

    /// <summary>
    /// n番目の画像の高さ
    /// </summary>
    /// <param name="index">n番目</param>
    /// <returns>画像の高さ</returns>
    public float GetItemY(int index) => _reelItems[index].anchoredPosition.y;

    /// <summary>
    /// 0番目の画像の高さ
    /// </summary>
    /// <returns>画像の高さ</returns>
    public float GetFirstItemY() => _reelItems[0].anchoredPosition.y;

    /// <summary>
    /// 画像の数
    /// </summary>
    /// <returns></returns>
    public int GetItemCount() => _reelItems.Count;
}
