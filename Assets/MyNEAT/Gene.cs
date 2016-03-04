using UnityEngine;
using System.Collections;

public class Gene
{
    public int inno = 0;
    public int inNodeID = 0;
    public int outNodeID = 0;
    public float weight = 0f;
    public bool enabled = true;

    public Gene(int inno, int inNodeID, int outNodeID, float weight, bool enabled)
    {
        this.inno = inno;
        this.inNodeID = inNodeID;
        this.outNodeID = outNodeID;
        this.weight = weight;
        this.enabled = enabled;
    }
	
}
