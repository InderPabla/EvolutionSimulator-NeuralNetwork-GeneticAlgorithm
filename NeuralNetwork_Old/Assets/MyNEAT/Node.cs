using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node  {

    public static int TYPE_INPUT = 0;
    public static int TYPE_INPUT_BIAS = 1;
    public static int TYPE_HIDDEN = 2;
    public static int TYPE_OUTPUT = 3;

    public int nodeID = 0;
    public int type = 0; // 1 input, 3 = bian input, 0 = hidden node
    public float nodeValue = 0;
    

	public Node(int nodeID, int type, float nodeValue)
    {
        this.type = type;
        this.nodeValue = nodeValue;
        this.nodeID = nodeID;

        if (this.type == TYPE_INPUT_BIAS)
            this.nodeValue = 1f;
    }

    public Node Copy()
    {
        return new Node(nodeID,type,nodeValue);
    }
}