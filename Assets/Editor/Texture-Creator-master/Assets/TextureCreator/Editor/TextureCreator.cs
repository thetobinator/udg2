using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Serialization;

public class TextureCreator : EditorWindow {

    private Texture2D tex;
    private Texture2D texInput;
    private Texture2D texCropped;
    private Texture2D texDiffuse;
    private Texture2D texColor;
    private Texture2D texTmp;
    private RenderTexture tex2;

    private Texture2D seamless1;
    private Texture2D seamless2;

    private ShaderMat contrast;
    private ShaderMat blurHeight;
    private ShaderMat blurNormal;
    private ShaderMat blurOcclusion;
    private ShaderMat specShadows;
    private ShaderMat downSample;
    private ShaderMat HSV;
    private ShaderMat heightmap;
    private ShaderMat normal;
    private ShaderMat periodize;
    private ShaderMat HSV_diffuse;
    private ShaderMat occlusion;
    private ShaderMat specular;

    private bool justDidUndo = false;

    [SerializeField]
    private float xoff = 0;
    [SerializeField]
    private float yoff = 0;

    [SerializeField]
    private bool doSeamless = true;

    private string outputFileName = "";
    [SerializeField]
    private string outputDir = "";    
    private string assetName = "";

    private enum States { Load, Diffuse, Height, Normal, Occlusion, Specular };
    private States state;
    private States previousState;

    private enum FactorTwo { One, Two, Four, Height };
    [SerializeField]
    private FactorTwo factorTwo = FactorTwo.One;

    private enum SeamLessMaskIndices { s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16, s17 };
    [SerializeField]
    private SeamLessMaskIndices seamlessMaskIdx = SeamLessMaskIndices.s1;

    private enum SpecularValues { Coal, Brick, Concrete, Wood, Iron, Silver };
    private SpecularValues specValue = SpecularValues.Brick;

    [MenuItem("Window/Texture Creator")]
    public static void ShowWindow() {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(TextureCreator));
    }

    //Define data structure to be able to save parameters for each tab
    [System.Serializable]
    public class LoadData {
        public float xoff;
        public float yoff;
        public bool seamless;
        public float[] periodize;
        public int factorTwo;
        public int seamlessMaskIdx;
        public string outputDir;
    }
    [System.Serializable]
    public class DiffuseData {
        public float[] specShadows;
        public float[] HSV_diffuse;
        public float[] contrast;
    }

    [System.Serializable]
    public class HeightData {
        public float[] heightmap;
        public float[] blurHeight;
    }
    [System.Serializable]
    public class NormalData {
        public float[] normal;
        public float[] blurNormal;
    }
    [System.Serializable]
    public class OcclusionData {
        public float[] occlusion;
        public float[] blurOcclusion;

    }
    [System.Serializable]
    public class SpecularData {
        public float[] specular;
    }

    private void OnUndo() {
        justDidUndo = true;
    }

    void OnEnable() {

        hideFlags = HideFlags.HideAndDontSave;

        Undo.undoRedoPerformed += OnUndo;

        state = States.Load;
        previousState = States.Diffuse;

        outputDir = "";

        LoadSeamLessMask();

        periodize = ScriptableObject.CreateInstance<ShaderMat>();
        periodize.Setup(
            "Hidden/Periodize", 
            new List<string> { "x", "y" },
            minimum : new float[] { 0f, 0f },
            maximum : new float[] { 1f, 1f },
            defaultValues: new float[] { 0f, 0.0f }
        );

        contrast = ScriptableObject.CreateInstance<ShaderMat>();
        contrast.Setup(
            "Hidden/Contrast",
            new List<string> { "Contrast", "Center" },
            minimum: new float[] { -2.0f, 0.0f },
            maximum: new float[] {  2.0f, 1.0f },
            defaultValues: new float[] {0f,0.5f}
        );
       
        heightmap = ScriptableObject.CreateInstance<ShaderMat>();
        heightmap.Setup(
            "Hidden/Heightmap",
            new List<string> { "Coef", "Center", "Min", "Max" },
            new float[] { -3f, 0f, 0f, 0f },
            new float[] {  3f, 1f, 1f, 1f },
            new float[] {  0f, 0.5f, 0f, 1f }
        );
        occlusion = ScriptableObject.CreateInstance<ShaderMat>();
        occlusion.Setup(
            "Hidden/Heightmap",
            new List<string> { "Coef", "Center", "Min", "Max" },
            new float[] { -3f, 0f, 0f, 0f },
            new float[] {  3f, 1f, 1f, 1f },
            new float[] {  0f, 0.5f, 0f, 1f }
        );

        specular = ScriptableObject.CreateInstance<ShaderMat>();
        specular.Setup(
            "Hidden/Heightmap",
            new List<string> { "Coef", "Center", "Min", "Max" },
            new float[] { -3f, 0f, 0f, 0f },
            new float[] {  3f, 1f, 1f, 1f },
            new float[] {  0f, 0.5f, 0.05f, 0.5f }
        );

        blurHeight = ScriptableObject.CreateInstance<ShaderMat>();
        blurHeight.Setup(
            "Hidden/GaussianBlur",
            new List<string> { "BlurRadius" },
            new float[] { 0.0f }, new float[] { 0.5f }, new float[] { 0.0f }
        );

        blurNormal = ScriptableObject.CreateInstance<ShaderMat>();
        blurNormal.Setup(
            "Hidden/GaussianBlur",
            new List<string> { "BlurRadius" },
            new float[] { 0.0f }, new float[] { 0.3f }, new float[] { 0.05f }
        );
        blurOcclusion = ScriptableObject.CreateInstance<ShaderMat>();
        blurOcclusion.Setup(
            "Hidden/GaussianBlur",
            new List<string> { "BlurRadius" },
            new float[] { 0.0f }, new float[] { 0.5f }, new float[] { 0.05f }
        );
        
        specShadows = ScriptableObject.CreateInstance<ShaderMat>();
        specShadows.Setup(
            "Hidden/SpecShadows",
            new List<string> { "Specular", "Shadows" },
            new float[] { -0.1f, -0.1f },
            new float[] { 1f, 1f },
            new float[] { 1f, 1f }
        );

        downSample = ScriptableObject.CreateInstance<ShaderMat>();
        downSample.Setup("Hidden/DownSample", new List<string>(), new float[0], new float[0], new float[0]);

        HSV = ScriptableObject.CreateInstance<ShaderMat>();
        HSV.Setup("Hidden/HSV", new List<string> { "Saturation", "Value" }, new float[] { 0f, 0f }, new float[] { 1f, 1f }, new float[] { 1f, 1f });

        HSV_diffuse = ScriptableObject.CreateInstance<ShaderMat>();
        HSV_diffuse.Setup(
            "Hidden/HSV",
            new List<string> { "Hue", "Saturation", "Value"},
            new float[] {0f,-1f,-1f},
            new float[] {1f, 1f, 1f},
            new float[] {0f, 0f, 0f}
        );

        normal = ScriptableObject.CreateInstance<ShaderMat>();
        normal.Setup(
            "Hidden/Normal",
            new List<string> {"NormalCoef", "Offset"},
            new float[] { 0.0f, -1.0f },
            new float[] { 1.0f, 1f },
            new float[] { 1.0f, 0f }
        );

        //load parameters if saved file exists
        LoadAllData();
    }

    private void LoadSeamLessMask() {

        int idx = (int)seamlessMaskIdx;

        seamless1 = EditorGUIUtility.Load("seamless_mask_h_" + idx + ".png") as Texture2D;
        seamless2 = EditorGUIUtility.Load("seamless_mask_v_" + idx + ".png") as Texture2D;
    }


    void OnGUI() {

        Undo.RecordObject(this, "Texture Creator");
        EditorGUI.BeginChangeCheck();

        float y1 = 8;
        float y2 = 40;

        float x1 = 8;
        float x2 = 16;
        float x3 = 160;

        int buttonIndex = 0;
        foreach (States s in States.GetValues(typeof(States))) {
            DrawStateButton(70 * buttonIndex + x1, y1, s);
            buttonIndex++;
        }

        if (tex2 != null) { 
            EditorGUI.DrawPreviewTexture(new Rect(x3, y2, position.width - 190, position.width - 190), tex2);
            GUI.Label(new Rect(position.width-110, position.width - 140,100,20), tex2.width +  " x " + tex2.height);
        }

        bool hasStateChanged = previousState != state;
        previousState = state;

        if (justDidUndo) {
            hasStateChanged = true;
            justDidUndo = false;
        }
        
        switch(state) {
            case States.Load:

                UpdateLoad(y2, x1, x2, hasStateChanged);

                DrawMaterialButton(y1, x1, buttonIndex);
                break;

            case States.Diffuse:


                GUI.Label( new Rect(x2, y2,80,20), "HSV");                                
                DrawSliders(x2, y2 + 20, HSV_diffuse);
          
                DrawSliders(x2, y2 + 110, contrast);

                GUI.Label(new Rect(x2, y2+180, 100, 20), "Experimental"); 
                DrawSliders(x2, y2 + 200, specShadows);

                if (hasStateChanged)
                    GetColor();

                if (specShadows.CheckChange() || HSV_diffuse.CheckChange() || contrast.CheckChange() || hasStateChanged) {

                    //spec/shadow
                    specShadows.mat.SetTexture("_HSV", texColor); //texHSV is the color 

                    specShadows.ApplyShader(tex, tex2, 0);
                    ToTexture2D(tex2, ref texDiffuse);

                    specShadows.ApplyShader(texDiffuse, tex2, 1);
                    ToTexture2D(tex2, ref texDiffuse);
  
                    //HSV
                    HSV_diffuse.ApplyShader(texDiffuse, tex2, 0);//to HSV
                    ToTexture2D(tex2, ref texDiffuse);

                    HSV_diffuse.ApplyShader(texDiffuse, tex2, 3);// apply coef
                    ToTexture2D(tex2, ref texDiffuse);

                    HSV_diffuse.ApplyShader(texDiffuse, tex2, 1);// back to RGB
                    ToTexture2D(tex2, ref texDiffuse);

                    //contrast
                    contrast.ApplyShader(texDiffuse, tex2);
                    ToTexture2D(tex2, ref texDiffuse);

                    SaveDataDiffuse();
                }


                DrawWriteButton(y1, x1, buttonIndex);
                break;

            case States.Height:

                DrawSliders(x2, y2, heightmap);
                DrawSliders(x2, y2 + 100, blurHeight);

                if (heightmap.CheckChange() || blurHeight.CheckChange() || hasStateChanged) {

                        heightmap.ApplyShader(tex, tex2);
                        ToTexture2D(tex2, ref texTmp);

                        DoBlur(texTmp, tex2, blurHeight);

                        //ToTexture2D(dest, ref src);
                        //blurShader.ApplyShader(src, dest, 2);//verical pass

                        SaveDataHeight();
                }

                DrawWriteButton(y1, x1, buttonIndex);
                break;

            case States.Normal:

                DrawSliders(x2, y2, normal);
                DrawSliders(x2, y2 + 100, blurNormal);

                if (normal.CheckChange() || blurNormal.CheckChange() || hasStateChanged) {

                    normal.ApplyShader(tex, tex2);
                    ToTexture2D(tex2, ref texTmp);
                    DoBlur(texTmp, tex2, blurNormal);

                    SaveDataNormal();
                }

                DrawWriteButton(y1, x1, buttonIndex);
                break;

            case States.Occlusion:

                DrawSliders(x2, y2, occlusion);
                DrawSliders(x2, y2 + 100, blurOcclusion);

                if (occlusion.CheckChange() || blurOcclusion.CheckChange() || hasStateChanged) {

                    occlusion.ApplyShader(tex, tex2);
                    ToTexture2D(tex2, ref texTmp);
                    DoBlur(texTmp, tex2, blurOcclusion);

                    SaveDataOcclusion();
                }

                DrawWriteButton(y1, x1, buttonIndex);
                break;

            case States.Specular:

                DrawSliders(x2, y2, specular);

                GUI.Label(new Rect(x2, y2 + 130, 80, 20), "Preset");
                SpecularValues specValue_ = (SpecularValues)EditorGUI.EnumPopup(new Rect(x2, y2 + 150, 80, 20), specValue);

                if (specValue_ != specValue) {
                    specValue = specValue_;
                    SetSpecularValues();
                }

                if (specular.CheckChange() || hasStateChanged) {

                    specular.ApplyShader(tex, tex2);
                    SaveDataSpecular();
                }

                DrawWriteButton(y1, x1, buttonIndex);
                break;
        }

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(this);
        }

    }


    private void SetSpecularValues() { 

        switch(specValue) {

            case SpecularValues.Coal:
                specular.vars[2] = 0.05f;
                specular.vars[3] = 0.2f;
                break;
            case SpecularValues.Brick:
                specular.vars[2] = 0.08f;
                specular.vars[3] = 0.25f;
                break;
            case SpecularValues.Concrete:
                specular.vars[2] = 0.11f;
                specular.vars[3] = 0.32f;
                break;
            case SpecularValues.Wood:
                specular.vars[2] = 0.14f;
                specular.vars[3] = 0.4f;
                break;
            case SpecularValues.Iron:
                specular.vars[2] = 0.7f;
                specular.vars[3] = 0.8f;
                break;
            case SpecularValues.Silver:
                specular.vars[2] = 0.88f;
                specular.vars[3] = 0.99f;
                break;
        }
            
    }

    private void UpdateLoad(float y2, float x1, float x2, bool changedState) {

        bool doUpdate = false;
        bool reloadSeamless = false;

        Texture2D texInput_ = EditorGUI.ObjectField(new Rect(x2, y2, 110, 110), texInput, typeof(Texture2D), true) as Texture2D;

        doUpdate = OnTextureLoad(texInput_);//change the import settings if needed

        string outputDir_ = EditorGUI.TextField(new Rect(x2, y2 + 120, 110, 20), new GUIContent("","Name of the texture"), outputDir);
        bool savePara = HasChanged(ref outputDir, outputDir_, doUpdate);

        if (outputDir == "" && texInput)
            outputDir = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(texInput));

        if (texInput && outputDir != "") {

            GUI.Label(new Rect(160, position.width - 140, 400, 20), "Output directory:");
            GUI.Label(new Rect(160, position.width - 120, 400, 20), "Assets/Textures/" + outputDir + "/");
            
            FactorTwo factorTwo_ = (FactorTwo)EditorGUI.EnumPopup(new Rect(x2, y2 + 171, 60, 20), factorTwo);

            if (factorTwo_ != factorTwo) {
                factorTwo = factorTwo_;
                doUpdate = true;
            }

            int N = GetTextureSize();//compute target size

            int divFac = (int)factorTwo;
            divFac = (int)Mathf.Pow(2f, (float)divFac);

            GUI.Label(new Rect(x2, y2 + 190, 120, 20), new GUIContent("X offset","Horizontal offset in the texture"));
            float xoff2 = EditorGUI.Slider(new Rect(x2, y2 + 205, 120, 20), new GUIContent("","Horizontal offset in the texture"), xoff, 0f, texInput == null ? 1f : Mathf.Max(0f, texInput.width - N));

            GUI.Label(new Rect(x2, y2 + 225, 120, 20), new GUIContent("Y offset", "Vertical offset in the texture"));
            float yoff2 = EditorGUI.Slider(new Rect(x2, y2 + 240, 120, 20), new GUIContent("", "Vertical offset in the texture"), yoff, 0f, texInput == null ? 1f : Mathf.Max(0f, texInput.height - N));

            doUpdate = HasChanged(ref xoff, xoff2, doUpdate);
            doUpdate = HasChanged(ref yoff, yoff2, doUpdate);

            //RNG
            if (GUI.Button(new Rect(x2, y2 + 380, 60, 20), new GUIContent("RNG","Randomize parameters"))) {
                seamlessMaskIdx = GetRandomEnum<SeamLessMaskIndices>();
                xoff = Mathf.Floor( UnityEngine.Random.Range(0, Mathf.Max(0f, texInput.width  - N)) );
                yoff = Mathf.Floor( UnityEngine.Random.Range(0, Mathf.Max(0f, texInput.height - N)) );
                doUpdate = true;
                reloadSeamless = true;
            }

            //Crop
            GUI.Label(new Rect(x2, y2 + 150, 120, 20), new GUIContent("Down scale","Reduce resolution")); 
            if (doUpdate || changedState) {

                if (!texCropped || texCropped.width != N) {
                    DestroyImmediate(texCropped);
                    texCropped = new Texture2D(N, N);
                    texCropped.hideFlags = HideFlags.HideAndDontSave;
                }

                if (tex2 == null || tex2.width != texInput.width || tex2.height != texInput.height) {
                    DestroyImmediate(tex2);
                    tex2 = new RenderTexture(texInput.width, texInput.height, 16);
                    tex2.hideFlags = HideFlags.HideAndDontSave;
                }

                Graphics.Blit(texInput, tex2);

                RenderTexture.active = tex2;
                texCropped.ReadPixels(new Rect(xoff, yoff, N, N), 0, 0);
                texCropped.Apply();
                RenderTexture.active = null;

                DestroyImmediate(tex2);
                tex2 = new RenderTexture(N / divFac, N / divFac, 16);
                tex2.hideFlags = HideFlags.HideAndDontSave;

                Graphics.Blit(texCropped, tex2);

                ToTexture2D(tex2, ref texCropped);
                ToTexture2D(tex2, ref tex);//copy in case we don't use seamless version

                //Debug.Log("Target size: " + texCropped.width);

                //Graphics.Blit(texCropped, tex2);
            }



            GUI.Label(new Rect(x2, y2 + 280, 80, 20), "Seamless");
            bool doSeamless2 = EditorGUI.Toggle(new Rect(x2 + 85, y2 + 280, 60, 20), doSeamless);

            SeamLessMaskIndices seamlessMaskIdx_ = (SeamLessMaskIndices)EditorGUI.EnumPopup(new Rect(x2, y2 + 300, 60, 20), seamlessMaskIdx);

            if (seamlessMaskIdx_ != seamlessMaskIdx || reloadSeamless) {
                seamlessMaskIdx = seamlessMaskIdx_;
                doUpdate = true;
                LoadSeamLessMask();
            }

            //Make seamless
            DrawSliders(x2, y2 + 315, periodize);

            if (periodize.CheckChange() || doUpdate || changedState || HasChanged(ref doSeamless, doSeamless2, doUpdate) ) {

                doUpdate = true;

                if (doSeamless) {
                    periodize.mat.SetTexture("_MaskX", seamless1);
                    periodize.mat.SetTexture("_MaskY", seamless2);

                    periodize.ApplyShader(texCropped, tex2, 1);

                    ToTexture2D(tex2, ref tex);
                } 
                else { //just copy he cropped version    
                    Graphics.Blit(texCropped, tex2);     
                }

            }

            //Save parameters
            if (doUpdate || savePara) 
                SaveDataLoad();   
            
        }
    }

    //TODO: this is ugly, refactoring would be nice
    private void SaveDataLoad() {

        LoadData d = new LoadData();
        d.xoff = xoff;
        d.yoff = yoff;
        d.seamless = doSeamless;
        d.periodize = periodize.vars;
        d.factorTwo = (int)factorTwo;
        d.seamlessMaskIdx = (int)seamlessMaskIdx;
        d.outputDir = outputDir;
        WriteParams(d, assetName, "Load.xml");
    }

    private void LoadDataLoad() {

        if (File.Exists(GetSavePath(assetName) + "Load.xml")) {

            LoadData d = ReadParams<LoadData>(assetName, "Load.xml");
            xoff = d.xoff;
            yoff = d.yoff;
            doSeamless = d.seamless;
            periodize.vars = d.periodize;
            factorTwo = (FactorTwo)d.factorTwo;
            seamlessMaskIdx = (SeamLessMaskIndices)d.seamlessMaskIdx;
            outputDir = d.outputDir;
        }
        else {  //reset everything           
            xoff = 0f;
            yoff = 0f;
            doSeamless = true;
            periodize.ResetPara();
            factorTwo = FactorTwo.One;
            seamlessMaskIdx = SeamLessMaskIndices.s1;
            outputDir = "";
        }
    }

    private void SaveDataDiffuse() {

        DiffuseData d = new DiffuseData();
        d.specShadows = specShadows.vars;
        d.HSV_diffuse = HSV_diffuse.vars;
        d.contrast = contrast.vars;
        WriteParams(d, assetName, "Diffuse.xml");
    }

    private void LoadDataDiffuse() {

        if (File.Exists(GetSavePath(assetName) + "Diffuse.xml")) {
            
            DiffuseData d = ReadParams<DiffuseData>(assetName, "Diffuse.xml");
            specShadows.SetPara(d.specShadows);
            HSV_diffuse.SetPara(d.HSV_diffuse);
            contrast.SetPara(d.contrast);
        }
        else {
            specShadows.ResetPara();
            HSV_diffuse.ResetPara();
            contrast.ResetPara();
        }
    }

    private void SaveDataHeight() {

        HeightData d = new HeightData();
        d.blurHeight = blurHeight.vars;
        d.heightmap = heightmap.vars;
        WriteParams(d, assetName, "Height.xml");
    }

    private void LoadDataHeight() {

        if (File.Exists(GetSavePath(assetName) + "Height.xml")) {

            HeightData d = ReadParams<HeightData>(assetName, "Height.xml");
            blurHeight.SetPara(d.blurHeight);
            heightmap.SetPara(d.heightmap);
        }
        else {
            blurHeight.ResetPara();
            heightmap.ResetPara();
        }
    }

    private void SaveDataNormal() {

        NormalData d = new NormalData();
        d.normal = normal.vars;
        d.blurNormal = blurNormal.vars;
        WriteParams(d, assetName, "Normal.xml");
    }

    private void LoadDataNormal() {

        if (File.Exists(GetSavePath(assetName) + "Normal.xml")) {

            NormalData d = ReadParams<NormalData>(assetName, "Normal.xml");
            normal.SetPara(d.normal);
            blurNormal.SetPara(d.blurNormal);
        }
        else {
            normal.ResetPara();
            blurNormal.ResetPara();
        }
    }

    private void SaveDataOcclusion() {
        OcclusionData d = new OcclusionData();
        d.occlusion = occlusion.vars;
        d.blurOcclusion = blurOcclusion.vars;
        WriteParams(d, assetName, "Occlusion.xml");
    }
    private void LoadDataOcclusion() {
        if (File.Exists(GetSavePath(assetName) + "Occlusion.xml")) {
            OcclusionData d = ReadParams<OcclusionData>(assetName, "Occlusion.xml");
            occlusion.SetPara(d.occlusion);
            blurOcclusion.SetPara(d.blurOcclusion);
        }
        else {
            occlusion.ResetPara();
            blurOcclusion.ResetPara();
        }
    }

    private void SaveDataSpecular() {
        SpecularData d = new SpecularData();
        d.specular = specular.vars;
        WriteParams(d, assetName, "Specular.xml");
    }
    private void LoadDataSpecular() {
        if (File.Exists(GetSavePath(assetName) + "Specular.xml")) {
            SpecularData d = ReadParams<SpecularData>(assetName, "Specular.xml");
            specular.SetPara(d.specular);
        }
        else {
            specular.ResetPara();
        }
    }

    private void WriteParams<T>(T d, string folder, string fname) {

        string pth = GetSavePath(folder);
        Stream s = File.Create(pth + fname);

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(s, d);
        s.Close();
    }

    private T ReadParams<T>(string folder, string fname) {

        string pth = GetSavePath(folder);   
        Stream s = File.Open(pth + fname, FileMode.Open);

        XmlSerializer serializer = new XmlSerializer(typeof(T));        
        T d = (T)serializer.Deserialize(s);

        s.Close();

        return d;
    }

    private static string GetSavePath(string folder) {

        string pth = Application.dataPath + "/TextureCreator/Save/" + folder + "/";
        if (!Directory.Exists(pth))
            Directory.CreateDirectory(pth);
        return pth;
    }


    //TODO could use generic methods here
    private bool HasChanged(ref string x, string x_, bool doUpdate) {
        if (x_ != x) {
            doUpdate = true;
            x = x_;
        }
        return doUpdate;
    }

    private bool HasChanged(ref float x, float x_, bool doUpdate) {        
        if (x_ != x) {
            doUpdate = true;
            x = x_;
        }
        return doUpdate;
    }
    private bool HasChanged(ref bool x, bool x_, bool doUpdate) {
        if (x_ != x) {
            doUpdate = true;
            x = x_;
        }
        return doUpdate;
    }

    private T GetRandomEnum<T>() {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

    private bool OnTextureLoad(Texture2D texInput_) {

        bool doUpdate = false;

        if ((texInput_ != null && texInput == null) || (texInput_ != null && texInput_ != texInput) ) {

            doUpdate = true;

            texInput = texInput_;

            assetName = AssetDatabase.GetAssetPath(texInput);
            assetName = Animator.StringToHash(assetName).ToString();

            LoadAllData();
            LoadSeamLessMask();

            if (texInput.width != texInput.height) {

                string pth = AssetDatabase.GetAssetPath(texInput);
                TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(pth);

                if (importer.npotScale != TextureImporterNPOTScale.None) { 
                    importer.npotScale = TextureImporterNPOTScale.None;
                    importer.maxTextureSize = 8192;
                    Debug.Log("Texture" + pth + "settings have been changed to allow non-square import.");

                    AssetDatabase.ImportAsset(pth, ImportAssetOptions.ForceUpdate);
                }
            }
        }

        return doUpdate;
    }

    private void LoadAllData() {

        LoadDataLoad();
        LoadDataDiffuse();
        LoadDataHeight();
        LoadDataNormal();
        LoadDataSpecular();
        LoadDataOcclusion();
    }

    private int GetTextureSize() {
        int N = Mathf.Min(texInput.width, texInput.height);
        N = Mathf.NextPowerOfTwo(N);
        N = N / 2;
        return N;
    }

    private void DrawWriteButton(float y1, float x1, int i) {
        outputFileName = GUI.TextField(new Rect(70 * (i + 1) + x1, y1, 60, 20), outputFileName, 25);
        if (GUI.Button(new Rect(70 * i + x1, y1, 60, 20), new GUIContent("Write"))) {

            WriteImage(outputFileName);
            AssetDatabase.Refresh();
        }
    }

    private void DrawMaterialButton(float y1, float x1, int i) {        
        if (GUI.Button(new Rect(70 * i + x1, y1, 120, 20), new GUIContent("Create Material"))) {
            CreateMaterial();            
        }
    }

    private void CreateMaterial() {

        string pth = Application.dataPath + "/Textures/" + outputDir + "/";
        if (!Directory.Exists(pth))
            Directory.CreateDirectory(pth);

        pth = "Assets/Textures/" + outputDir + "/" + outputDir + ".mat";
        Material tmp = AssetDatabase.LoadAssetAtPath(pth, typeof(Material)) as Material;

        if (tmp == null) {
            tmp = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(tmp, pth);
            AssetDatabase.Refresh();
        }
        EditorGUIUtility.PingObject(tmp);
    }

    private void UpdateMaterial() {

        string pth = "Assets/Textures/" + outputDir + "/" + outputDir + ".mat";
        Material tmp = AssetDatabase.LoadAssetAtPath(pth, typeof(Material)) as Material;

        if (tmp == null) {
            CreateMaterial();
            tmp = AssetDatabase.LoadAssetAtPath(pth, typeof(Material)) as Material;
        }

        string[] names = new string[] {"Diffuse","Height","Normal","Occlusion","Specular"};
        string[] shader_names = new string[] { "_MainTex", "_ParallaxMap", "_BumpMap","_OcclusionMap","_MetallicGlossMap"};

        for (int i = 0; i < names.Length; i++) {

            pth = Application.dataPath + "/Textures/" + outputDir + "/";
            if (File.Exists(pth + GetImageName(outputDir, names[i]))) {

                pth = "Assets/Textures/" + outputDir + "/";
                Texture2D tmpTex = AssetDatabase.LoadAssetAtPath(pth + GetImageName(outputDir, names[i]), typeof(Texture2D)) as Texture2D;

                tmp.SetTexture(shader_names[i], tmpTex);

                if (names[i] == "Normal")
                    tmp.EnableKeyword("_NORMALMAP");
                if (names[i] == "Specular")
                    tmp.EnableKeyword("_METALLICGLOSSMAP");
                if(names[i] == "Height")
                    tmp.EnableKeyword("_PARALLAXMAP");

            }
        }
                        
        AssetDatabase.ImportAsset("Assets/Textures/" + outputDir + "/" + outputDir + ".mat");
        AssetDatabase.Refresh();
    }

    private void WriteImage(string name) {

        if (outputDir == "")
            Debug.LogError("You need to enter a name in the Load panel.");

        string pth = Application.dataPath + "/Textures/" + outputDir + "/";
        if (!Directory.Exists(pth))
            Directory.CreateDirectory(pth);

        Texture2D tmp = new Texture2D(tex2.width, tex2.height);
        
        ToTexture2D(tex2, ref tmp);
        var bytes = tmp.EncodeToPNG();
        File.WriteAllBytes(pth + GetImageName(outputDir, name), bytes);
        DestroyImmediate(tmp);

        AssetDatabase.Refresh();

        UpdateMaterial(); 
    }

    private string GetImageName(string outputDir, string name) {
        return outputDir + "_" + name + ".png";
    }

    private void DoBlur(Texture2D src, RenderTexture dest, ShaderMat blurShader) {

        blurShader.ApplyShader(src, dest, 1);//horizontal pass
        ToTexture2D(dest,  ref src);
        blurShader.ApplyShader(src, dest, 2);//verical pass
    }

    private void DrawStateButton(float posx, float posy, States s) {

        if (GUI.Button(new Rect(posx, posy, 65, 20), new GUIContent(s.ToString()))) {
            state = s;
            outputFileName = s.ToString();
        }
    }

    private void GetColor() {

        //convert to hsv
        RenderTexture tmp = RenderTexture.GetTemporary(tex.width, tex.height, 0);
        Graphics.Blit(tex, tmp, HSV.mat, 0);

        ToTexture2D(tmp, ref texColor);

        //compute mean hsv
        Color meanCol = DownSample(texColor);

        //convert back to rgb
        HSV.mat.SetFloat("_SaturationUniform", meanCol.g);
        HSV.mat.SetFloat("_ValueUniform", meanCol.b);
        Graphics.Blit(texColor, tmp, HSV.mat, 2);
        ToTexture2D(tmp, ref texColor);

        //smooth a bit ?
        blurHeight.mat.SetFloat("_BlurRadius", 1.0f);
        Graphics.Blit(texColor, tmp, blurHeight.mat);
        ToTexture2D(tmp,ref texColor);

        Graphics.Blit(texColor, tex2);

        RenderTexture.ReleaseTemporary(tmp);
    }

    private void CheckShader(ShaderMat s) {
        CheckShader(s, 0, tex);
    }

    private void CheckShader(ShaderMat s, int pass) {
        CheckShader(s, pass, tex);
    }

    private void CheckShader(ShaderMat s, int pass, Texture2D src, bool forceUpdate = false) {
        if (s.CheckChange() || forceUpdate) {
            if (!tex2 || tex2.width != tex.width) {
                tex2 = new RenderTexture(tex.width, tex.height, 16);
                tex2.hideFlags = HideFlags.HideAndDontSave;
            }
            s.ApplyShader(src, tex2, pass);
        }
    }

    private void DrawButtton(float posx, float posy, ShaderMat s, string name) {
        DrawButtton(posx, posy, s, name, 0);
    }

    private void DrawButtton(float posx, float posy, ShaderMat s, string name, int pass) {
        if (GUI.Button(new Rect(posx, posy, 60, 20), new GUIContent(name))) {

            if (!tex2) {
                tex2 = new RenderTexture(tex.width, tex.height, 16);
                tex2.hideFlags = HideFlags.HideAndDontSave;
            }
            s.ApplyShader(tex, tex2, pass);
        }
    }

    private Color DownSample(Texture2D t) {

        int downsample = (int)Mathf.Log(t.width, 2);

        int div = 2;
        var rts = new RenderTexture[downsample];
        for (int i = 0; i < downsample; i++) {
            rts[i] = RenderTexture.GetTemporary(t.width / div, t.width / div, 16);
            div *= 2;
        }

        // downsample pyramid
        Graphics.Blit(t, rts[0]);

        for (int i = 0; i < downsample - 1; i++) {
            Graphics.Blit(rts[i], rts[i + 1], downSample.mat);
            RenderTexture.ReleaseTemporary(rts[i]);
        }

        Texture2D colTex = new Texture2D(tex2.width, tex2.height);
        ToTexture2D(rts[downsample - 1], ref colTex);
        Color meanCol = colTex.GetPixel(1, 1);

        for (int i = 0; i < downsample; i++) 
            DestroyImmediate(rts[i]);


        return meanCol;

    }

    private void ToTexture2D(RenderTexture t, ref Texture2D tOut) {

        if (!tOut || tOut.width != t.width) {
            DestroyImmediate(tOut);
            tOut = new Texture2D(t.width, t.height, TextureFormat.RGB24, false);
        }
        
        RenderTexture.active = t;
        tOut.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0); // Pixels are read from current render target.
        tOut.Apply();
        RenderTexture.active = null;
        tOut.hideFlags = HideFlags.HideAndDontSave;
        
    }

    private void DrawSliders(float posx, float posy, ShaderMat s) {

        float dy = 12f;
        int j = 0;
        for (int i = 0; i < s.names.Count; i++) {
            GUI.Label(new Rect(posx, posy + j * dy, 120, 20), s.names[i]);
            j++;
            s.vars[i] = EditorGUI.Slider(new Rect(posx, posy + j * dy, 120, 20), "", s.vars[i], s.mins[i], s.maxs[i]);
            j++;
        }
    }

}