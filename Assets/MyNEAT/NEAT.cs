using UnityEngine;
using System.Collections;

public class NEAT : MonoBehaviour
{
    Genome genome;

	// Use this for initialization
	void Start ()
    {
        genome = new Genome();
        genome.MakeRandom();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
