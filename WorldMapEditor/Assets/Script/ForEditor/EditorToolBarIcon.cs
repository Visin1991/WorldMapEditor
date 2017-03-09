using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorToolBarIcon : MonoBehaviour {
#if UNITY_EDITOR
    public enum BarType
    {
        Brush,
        Erazer,
        Line,
    }
    public BarType type = BarType.Brush;
    public Texture2D preview;
#endif
}
