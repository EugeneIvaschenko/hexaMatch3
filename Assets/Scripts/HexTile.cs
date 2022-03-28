using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexTile : MonoBehaviour {
    private SpriteRenderer _sprite;
    private bool _isHighlighted = false;

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
        _isHighlighted = highlight;
        if (_isHighlighted) _sprite.color = highlightColor;
        else _sprite.color = _origColor;
    }

    public void SetText(Vector2 pos) {
        TMP_Text tmp = GetComponentInChildren<TMP_Text>();
        name = $"Tile {pos.x} {pos.y}";
        tmp.text = $"{pos.x} {pos.y}";
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