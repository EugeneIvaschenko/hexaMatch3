using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexTile : MonoBehaviour {
    private SpriteRenderer _sprite;

    public Action<HexTile> OnHexMouseClick;
    public Action<HexTile> OnHexMouseEnter;
    public Action<HexTile> OnHexMouseExit;

    [HideInInspector] public Gem content;
    [HideInInspector] public Vector2 axialPos;

    private Color _origColor;
    public Color highlightColor;

    private void Start() {
        _sprite = GetComponent<SpriteRenderer>();
        _origColor = _sprite.color;
    }

    public void SetHighlight(bool highlight) {
        if (highlight) _sprite.color = highlightColor;
        else _sprite.color = _origColor;
    }

    public void SetText(Vector2 pos) {
        TMP_Text tmp = GetComponentInChildren<TMP_Text>();
        name = $"Tile {pos.x} {pos.y}";
        tmp.text = $"{pos.x} {pos.y}";
    }

    public void SwapGems(HexTile otherTile) {
        Gem tempGem = this.content;
        this.content = otherTile.content;
        otherTile.content = tempGem;
    }

    private void OnMouseEnter() {
        OnHexMouseEnter?.Invoke(this);
    }

    private void OnMouseExit() {
        OnHexMouseExit?.Invoke(this);
    }

    private void OnMouseUpAsButton() {
        OnHexMouseClick?.Invoke(this);
    }
}