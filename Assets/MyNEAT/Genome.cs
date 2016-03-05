using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Genome
{
    public List<Node> nodeList = new List<Node>();
    public List<Gene> geneList = new List<Gene>();
    public Genome()
    {

    }

    public void MakeRandom()
    {
        Node node1 = new Node(0, Node.TYPE_INPUT, 0);
        Node node2 = new Node(1, Node.TYPE_INPUT, 0);
        Node node3 = new Node(2, Node.TYPE_INPUT, 0);
        Node node4 = new Node(3, Node.TYPE_INPUT, 0);
        Node node5 = new Node(4, Node.TYPE_INPUT, 0);
        Node node6 = new Node(5, Node.TYPE_INPUT_BIAS, 1f);
        Node node7 = new Node(6, Node.TYPE_OUTPUT, 0);

        nodeList.Add(node1);
        nodeList.Add(node2);
        nodeList.Add(node3);
        nodeList.Add(node4);
        nodeList.Add(node5);
        nodeList.Add(node6);
        nodeList.Add(node7);

        Mutate(); 
        //print();
        //feedforward();
    }

    public void Mutate()
    {
        int randomNodeIndex = Random.Range(-1, nodeList.Count);
        if (randomNodeIndex == -1)
        {
            Gene gene;
            Node node = new Node(nodeList.Count, Node.TYPE_HIDDEN, 0);
            int inIndex = node.nodeID;
            int outIndex = Random.Range(0, nodeList.Count);
            float weight = Random.Range(-10f, 10f);
            bool enabled = Random.Range(0, 4) != 0 ? true : false;
            int inno = 1;

            nodeList.Add(node);

            if(!geneConnectionExists(inIndex, outIndex))
            {
                gene = new Gene(inno,inIndex,outIndex,weight,enabled);
                geneList.Add(gene);
            }
            
        }
        else
        {
            Gene gene;
            Node node = nodeList[randomNodeIndex];
            int inIndex = node.nodeID;

            int outMin = 0;
            if (node.type == Node.TYPE_INPUT || node.type == Node.TYPE_INPUT_BIAS)
                outMin = 6;

            int outIndex = Random.Range(outMin, nodeList.Count);
            float weight = Random.Range(-10f, 10f);
            bool enabled = Random.Range(0, 4) != 0 ? true : false;
            int inno = 1;

            
            if (!geneConnectionExists(inIndex, outIndex))
            {
                gene = new Gene(inno, inIndex, outIndex, weight, enabled);
                geneList.Add(gene);
            }
        }
    }

    /*
        LINKS
        -When the GA mutates a link, it randomly chooses two nodes and inserts a new link gene 
        with an initial weight of one. 

        -If a link already existed between the chosen nodes but was disabled, the GA re-enables 
        it. 

        -Finally if there is no link between the chosen nodes and an equivalent link has already 
        been created by another genome in this population this link is created with the same 
        innovation number as the previously created link as it is not a newly emergent 
        innovation.
    */
    public void LinkMutation()
    {

    }

    /*
        NODE
        -A node mutation is similar to a link mutation but differs from it in that instead of 
        choosing two nodes and inserting a link,   the GA chooses and disables an existing 
        link and inserts a node. 
    */
    public void NodeMutation()
    {

    }

    public bool geneConnectionExists(int inIndex, int outIndex)
    {
        for(int i = 0; i < geneList.Count; i++)
        {
            if(geneList[i].inNodeID == inIndex && geneList[i].outNodeID == outIndex)
            {
                return true;
            }
        }

        return false;
    }



    public void feedforward()
    {
        Node[] tempNode = new Node[nodeList.Count];

        for(int i = 0; i < tempNode.Length; i++)
        {
            tempNode[i] = nodeList[i].Copy();
            tempNode[i].nodeValue = 0;
        }

        for (int i = 0; i < geneList.Count; i++)
        {
            int inNode = geneList[i].inNodeID;
            int outNode = geneList[i].outNodeID;
            float weight = geneList[i].weight;
            if (geneList[i].enabled)
            {
                tempNode[outNode].nodeValue += nodeList[inNode].nodeValue * weight;
            }
        }

        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeList[i].nodeValue = tanh(tempNode[i].nodeValue);
            if (nodeList[i].type == Node.TYPE_INPUT_BIAS)
                nodeList[i].nodeValue = 1f;
        }


    }

    public void print()
    {
        for(int i = 0 ; i <nodeList.Count; i++)
        {
            Node node = nodeList[i];
            Debug.Log(node.nodeID+" "+node.type);
        }
        Debug.Log("-----------------------------------------------------");
        for (int i = 0; i < geneList.Count; i++)
        {
            Gene gene = geneList[i];
            if(gene.enabled)
            Debug.Log(gene.inNodeID+" "+gene.outNodeID+" "+" "+gene.weight+" "+gene.enabled);
        }
    }

    public float tanh(float x)
    {
        if (x > 20f)
            return 1;
        else if (x < -20f)
            return -1f;
        else
        {
            float a = Mathf.Exp(x);
            float b = Mathf.Exp(-x);
            return (a - b) / (a + b);
        }
    }
}
