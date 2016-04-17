using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Genome
{
    public List<Node> nodeList = new List<Node>();
    public List<Gene> geneList = new List<Gene>();
    int numberOfInputNodes = 0, numberOfOutputNodes = 0, numberOfHiddenNodes = 0;
    int inputStartIndex = 0, outputStartIndex = 0, hiddenStartIndex = 0;
    int mutationChance = 50; //to mutate or not to mutate 
    int linkMutationChance = 50; //mutate chance for link  (100-linkChance) = node mutation chance

    const int NOT_EXIST = -1;

    public Genome()
    {

    }

    public Genome(int numberOfInput, int numberOfOutput)
    {
        this.numberOfInputNodes = numberOfInput;
        this.numberOfOutputNodes = numberOfOutput;


        for(int i = 0;i< numberOfInputNodes; i++)
        {
            Node node = new Node(i, Node.TYPE_INPUT, 0);
            if(i == numberOfInputNodes-1)
                node = new Node(i, Node.TYPE_INPUT_BIAS, 1f);
            nodeList.Add(node);
        } 

        for (int i = numberOfInputNodes; i < numberOfOutputNodes+ numberOfInputNodes; i++)
        {
            Node node = new Node(i, Node.TYPE_OUTPUT, 0);
            nodeList.Add(node);
        }

        inputStartIndex = 0;
        outputStartIndex = numberOfInputNodes;
        hiddenStartIndex = outputStartIndex + numberOfOutputNodes;


        for (int i = 0; i < numberOfInputNodes; i++)
        {
            for (int j = outputStartIndex; j < outputStartIndex+numberOfOutputNodes; j++)
            {
                Gene gene = new Gene(0, i, j, 1f, true);
                geneList.Add(gene);
            }
        }

        print();
    
    }

    public void Mutate()
    {
        /*int randomNodeIndex = Random.Range(-1, nodeList.Count);
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
        }*/

        int randomMutationChance = Random.Range(0,100);
        if (randomMutationChance <= mutationChance)
        {
            int randomLinkChance = Random.Range(0, 100);
            if (randomLinkChance <= linkMutationChance)
            {
                LinkMutation();
            }
            else
            {
                NodeMutation();
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
        int inNodeIndex = Random.Range(0, nodeList.Count);
        int outNodeIndex = Random.Range(0, nodeList.Count);

        Node inNode = nodeList[inNodeIndex];
        Node outNode = nodeList[outNodeIndex];

        if ((inNodeIndex == outNodeIndex) ||
            (inNode.type <= Node.TYPE_INPUT && outNode.type <= Node.TYPE_INPUT) ||
            (inNode.type == Node.TYPE_OUTPUT && outNode.type == Node.TYPE_OUTPUT))
        {
            //in and out node are both the samme, or both are outputs or inputs 
            //this will cause issues with feed forward so we must ignore it
        }
        else
        {
            //in and out node are both different 

            int geneIndex = geneConnectionExists(inNodeIndex, outNodeIndex);
            if (geneIndex == NOT_EXIST)
            {
                //if gene does not exist

                Gene gene = new Gene(0,inNodeIndex,outNodeIndex,1f,true); //new gene 
                geneList.Add(gene); //add gene to the gene list
            }
            else
            {
                //if gene already exists with this creature

                if (geneList[geneIndex].enabled == false)
                {
                    geneList[geneIndex].enabled = true; //if gene is disabled, enable it
                }
                else
                {
                    geneList[geneIndex].weight = Random.Range(-10f,10f); //if gene is enabled, pick a random weight
                }
            }
        }
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

    public int geneConnectionExists(int inIndex, int outIndex)
    {
        for(int i = 0; i < geneList.Count; i++)
        {
            if(geneList[i].inNodeID == inIndex && geneList[i].outNodeID == outIndex)
            {
                return i;
            }
        }
        return -1;
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
