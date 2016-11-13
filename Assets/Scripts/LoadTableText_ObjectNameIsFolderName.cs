using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[ExecuteInEditMode]
public class LoadTableText_ObjectNameIsFolderName : MonoBehaviour
{ 
    public bool importDirectory = false;
    private string appPath;
    private bool hasInit = false;

   public void Init()
    {
        appPath =  Application.dataPath  + "/Resources/" + this.gameObject.name ;
        string[] dirs = Directory.GetDirectories(appPath, "*"); // appPath, "*", SearchOption.TopDirectoryOnly);  
        foreach (string dir in dirs)
        {
            string folder = dir.Replace(appPath + "\\", "");
            //folders.Add(folder);
            ListFoldersAsObjects(dir);
            importDirectory = false;
            hasInit = true;
         }
      }

    public void Rename(string newName)
    {
        this.gameObject.name = newName;
    }

    void AttachTextMeshScripts(GameObject newObject , TextAsset textFile)
    {
        // ListFromTextAsset uses file IO to populate item list with strings.;
        newObject.AddComponent<ListFromTextAsset>();
        newObject.GetComponent<ListFromTextAsset>().TextFile = textFile;
        newObject.GetComponent<ListFromTextAsset>().Init();

        if (newObject.GetComponent<ListFromTextAsset>().item != null)
        {
            newObject.AddComponent<List_TextMesh>();
            List_TextMesh listTextMesh = newObject.GetComponent<List_TextMesh>();
            if (listTextMesh != null)
            {
                // transfer list items from file to self contained list display script
                listTextMesh.Init();
                if (listTextMesh.item.Count >= 1)
                {
                    newObject.tag = "Category";
                    //   Destroy ListFromTextAssets so that the objects can travel independnet of files original text files.
                    DestroyImmediate(newObject.GetComponent<ListFromTextAsset>());
                }
            }

        }
    }
    // create an object for each folder, scan the folder for .txt files
    void ListFoldersAsObjects(string currentFolder)
    {      
        GameObject folderObject = new GameObject();
        folderObject.tag = "Directory";
        folderObject.transform.parent = this.transform;
        folderObject.name = currentFolder.Replace(appPath + "\\", "");
        DirectoryInfo dir = new DirectoryInfo(currentFolder);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {          
            if (!f.Name.Contains(".meta") && f.Name.Contains("_Table.txt"))
            {
                //create new object for each .txt file
                GameObject newObject = new GameObject();
                string newName = f.Name.Replace("_Table.txt", "");
                newObject.name = newName;
                newObject.transform.parent = folderObject.transform;            
                string shortPath = this.gameObject.name + "/" + folderObject.name + "/" + f.Name;
                shortPath = shortPath.Replace(".txt", "");
                TextAsset textFile = (TextAsset)Resources.Load<TextAsset>(shortPath);          
                if (textFile != null)
                {
                    AttachTextMeshScripts(newObject, textFile);
                }
            }
        }
     
    }
    void Update()
    {
        if (importDirectory && !hasInit)
        {
            Init();
        }
    }

}
