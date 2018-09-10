using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
using UnityEngine.UI;
public class GridAllocate : MonoBehaviour {

	private Material mat;
    private int rowID;
	private int columnID;

	void Awake () 
    {
		rowID = Shader.PropertyToID("_Rows");
		columnID = Shader.PropertyToID("_Columns");
        mat = GetComponent<Image>().material;
        SetGrid(6, 6);
	}

    void Update () 
    {

	}

    public void SetGrid(int r, int c)
    {
		mat.SetFloat(rowID, r);
		mat.SetFloat(columnID, c);
    }
}
