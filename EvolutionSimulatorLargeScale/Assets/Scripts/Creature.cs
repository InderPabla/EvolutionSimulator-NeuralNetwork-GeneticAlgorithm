using UnityEngine;
using System.Collections;

public class Creature : MonoBehaviour {

    public TextMesh text;

	// Use this for initialization
	void Start () {
        int nameSize = Random.Range(3,11);
        char[] name = new char[nameSize];

        int[] vowels = new int[] {97,101,105,111,117};
        int vowelCounter = 1;
        for (int i = 0; i < name.Length; i++) {
            int charNum = Random.Range(97, 123);

            bool isVowel = false;
            for (int j = 0; j < vowels.Length; j++)
            {
                if (charNum == vowels[j])
                {
                    isVowel = true;
                    break;
                }
            }

            if (isVowel)
            {
                vowelCounter = 1;
            }
            else if (vowelCounter == 3)
            {
                vowelCounter = 1;
                charNum = vowels[Random.Range(0, vowels.Length)];
            }

            vowelCounter++;
            name[i] = (char)charNum;
        }
        text.text = new string(name);

        
	}
	
	// Update is called once per frame
	void Update () {
        text.transform.eulerAngles = Vector3.zero;
    }

    
}
