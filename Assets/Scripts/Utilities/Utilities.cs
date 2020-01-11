using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontsize = 40, Color? color = null, 
        TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if(color == null)
        {
            color = Color.white;
        }
        return CreateWorldText(parent, text, localPosition, fontsize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontsize, Color color, 
        TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.Rotate(new Vector3(90, -90, 0));
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontsize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
