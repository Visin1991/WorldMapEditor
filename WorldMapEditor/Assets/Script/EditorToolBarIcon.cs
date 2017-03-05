using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToolBarIcon : MonoBehaviour {
    public enum BarType
    {
        Brush,
        Erazer,
        Line,
    }
    public BarType type = BarType.Brush;
    public string itemName = "";
}
