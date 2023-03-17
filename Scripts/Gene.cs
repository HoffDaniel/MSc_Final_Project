using UnityEngine;
public struct Gene
{
    public int GeneID { get; set; }

    public Vector2 GeneValue { get; set; }

    public Gene(int ID, Vector2 Value)
    {
        GeneID = ID;
        GeneValue = Value;
    }

}
