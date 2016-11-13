using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class RandomTextMovement : MonoBehaviour {
    // Coloring the GUI
    private float rr = 0.0f, gg = 0.0f, bb = 0.0f;
    private int rv = 1, gv = 1, bv = 1;
    private float slowColorTime = 0.0f;

    //textmesh location
    private float xMove, yMove, zMove;

   // public string timerText;
    private float startTime;

    void randomize()
    {
        UnityEngine.Random.InitState((UnityEngine.Random.Range(UnityEngine.Random.Range(UnityEngine.Random.Range(UnityEngine.Random.Range(0, 25), UnityEngine.Random.Range(324, 5673)), UnityEngine.Random.Range(UnityEngine.Random.Range(53, 2378), UnityEngine.Random.Range(50, 423))), UnityEngine.Random.Range(UnityEngine.Random.Range(UnityEngine.Random.Range(23, 2354), UnityEngine.Random.Range(1, 3456)), UnityEngine.Random.Range(UnityEngine.Random.Range(7, 32421), UnityEngine.Random.Range(8, 23472))))));

        xMove = Random.Range(-5, 5);
        yMove = Random.Range(-5, 5);
        zMove = Random.Range(-5, 5);
        rr = Random.Range(0.00f, 1.0f);
        gg = Random.Range(0.00f, 1.0f);
        bb = Random.Range(0.00f, 1.0f);
    }

    public Color slowColor()
    {
        Color GUITextColor = new Color(rr, gg, bb);
        slowColorTime = (Time.deltaTime * 0.05f);
        // this little bit here cycles through colors very very slowly
        if (rr <= 0.0f) { rv = 1; }
        if (gg <= 0.0f) { gv = 1; }
        if (bb <= 0.0f) { bv = 1; }
        if (rr >= 0.99f) { rv = -1; }
        if (gg >= 0.99f) { gv = -1; }
        if (bb >= 0.99f) { bv = -1; }
        if (rr <= 1.0f && gg <= 0.9f) { rr += (slowColorTime * rv); }
        if (gg <= 1.0f && rr >= 0.5f) { gg += (slowColorTime * gv); }
        if (bb <= 1.0f && gg >= 0.5f) { bb += (slowColorTime * bv); }
        return GUITextColor;
    }

   public void randomTextConcepts()
    {
        float timePassed = Time.time - startTime;
        /* string minutes = ((int)timePassed / 60).ToString();
         string seconds = (timePassed % 60).ToString("f2");
         timerText = minutes + ":" + seconds;*/
        if (timePassed >= 10.0f)
        {
            xMove = xMove * Random.Range(-1, 1);
            yMove = yMove * Random.Range(-1, 1);
            zMove = zMove * Random.Range(-1, 1);
            startTime = Time.time;
        }
        if (Random.Range(0, 100) >= 99) { randomize(); }
        Vector3 myPos = this.gameObject.transform.position;
        myPos.x = myPos.x + xMove * Time.deltaTime;
        myPos.y = myPos.y + yMove * Time.deltaTime;
        myPos.z = myPos.z + zMove * Time.deltaTime;
        Vector3 camPos = GameObject.Find("Camera").GetComponent<Transform>().position;
        if (myPos.z <= camPos.z) { myPos.z = Random.Range(0,50); }
        if (myPos.x >= 20 || myPos.x <= -20) { myPos.x = 0; }
        if (myPos.y >= 10 || myPos.y <= -10) { myPos.y = myPos.y*-1; }
        this.gameObject.transform.position = new Vector3(myPos.x, myPos.y, myPos.z);
    }
    void Start()
    {      
        startTime = Time.time;
        randomize();
    }
    void Update()
    {
       this.gameObject.GetComponent<TextMesh>().color = slowColor();
        randomTextConcepts();
    }

}
