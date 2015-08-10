using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ShaderMat : Editor {

    public List<string> names;
    [SerializeField]
    public float[] vars;
    [SerializeField]
    public float[] varsPrevious;

    public float[] mins;
    public float[] maxs;
    public float[] defs;

    public Shader shader;
    public Material mat;

  
    public ShaderMat(string shaderName, List<string> n, float[] minimum, float[] maximum) {

        shader = Shader.Find(shaderName);
        if (!shader)
            Debug.Log("Can't find shader named " + shaderName);

        Setup(shader, n, minimum, maximum, maximum);
    }

    public ShaderMat(string shaderName, List<string> n, float[] minimum, float[] maximum, float[] defaultValues) {

        shader = Shader.Find(shaderName);
        if (!shader)
            Debug.Log("Can't find shader named " + shaderName);

        Setup(shader, n, minimum, maximum, defaultValues);
    }

    public ShaderMat(Shader s, List<string> n, float[] minimum, float[] maximum) {
        Setup(s, n, minimum, maximum, maximum);
    }

    public ShaderMat(Shader s, List<string> n, float[] minimum, float[] maximum, float[] defaultValues) {
        Setup(s, n, minimum, maximum, defaultValues);
    }


    public void Setup(string shaderName, List<string> n, float[] minimum, float[] maximum, float[] defaultValues) {
        shader = Shader.Find(shaderName);
        if (!shader)
            Debug.Log("Can't find shader named " + shaderName);

        Setup(shader, n, minimum, maximum, defaultValues);
    }

    public void Setup(Shader s, List<string> n, float[] minimum, float[] maximum, float[] defaultValues) {

        shader = s;
        names = n;
        mins = minimum;
        maxs = maximum;
        vars = new float[minimum.Length];
        varsPrevious = new float[minimum.Length];
        defs = new float[minimum.Length];

        mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;

        for (int i = 0; i < vars.Length; i++) {
            vars[i] = defaultValues[i];
            varsPrevious[i] = defaultValues[i];
            defs[i] = defaultValues[i];
        }

        hideFlags = HideFlags.HideAndDontSave;
    }

    public void ApplyShader(Texture2D tex, RenderTexture tex2) {
        ApplyShader(tex, tex2, 0);
    }

    public void ApplyShader(Texture2D tex, RenderTexture tex2, int pass) {
        SetMatPara();
        Graphics.Blit(tex, tex2, mat, pass);
    }

    private void SetMatPara() {

        for (int i = 0; i < names.Count; i++) {
            mat.SetFloat("_" + names[i], vars[i]);
        }
    }

    public void SetPara(float[] p) {

        if (p == null) {
            Debug.LogError(shader.name + " para is null");
        }

        if (p.Length != vars.Length)
            Debug.LogError("input array length does not match vars's.");

        for (int i = 0; i < vars.Length; i++) {
            vars[i] = p[i];
            varsPrevious[i] = p[i];
        }
    }

    public void ResetPara() {

        if (defs != null) {
            for (int i = 0; i < defs.Length; i++) {
                vars[i] = defs[i];
                varsPrevious[i] = defs[i];
            }
        } else {

            Debug.Log(shader.name + " no defaults");

            for (int i = 0; i < maxs.Length; i++) {
                vars[i] = maxs[i];
                varsPrevious[i] = maxs[i];
            }
        }

    }

    public bool CheckChange() {

        Undo.RecordObject(this, shader.name);

        bool changed = false;
        for (int i = 0; i < vars.Length; i++) {
            if (vars[i] != varsPrevious[i])
                changed = true;
            varsPrevious[i] = vars[i];
        }

        if (changed) {
            EditorUtility.SetDirty(this);
        }

        return changed;
    }


}
