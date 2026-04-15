using UnityEngine;

[System.Serializable]
public class ShapeCell
{
    public int x;
    public int y;
}

[System.Serializable]
public class ShapeData
{
    public ShapeCell[] cells;
}