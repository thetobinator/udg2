using UnityEngine;
using System.Collections;

public class CamMovement_SRTSPack: MonoBehaviour
{
    public Vector2 moveSpeed;
    Vector2 moveSpeedCur;
    public int modifier;
    public Vector2 scrollMinMax;
    public float scrollSpeed;
    public Transform cam;
    [HideInInspector]
    public float scrollCur = 0;
    public Rect movementBorders;
    float amount = 0;

    public Rect mouseBorders;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MouseBorderMovement();
        KeyMovement();
        ScrollWheel();
        if (moveSpeedCur.x < 0 && moveSpeedCur.x * Time.deltaTime + transform.position.x < movementBorders.x)
        {
            transform.position = new Vector3(movementBorders.x, transform.position.y, transform.position.z);
            moveSpeedCur = new Vector2(0, moveSpeedCur.y);
        }
        else if (moveSpeedCur.x > 0 && moveSpeedCur.x * Time.deltaTime + transform.position.x > movementBorders.width)
        {
            transform.position = new Vector3(movementBorders.width, transform.position.y, transform.position.z);
            moveSpeedCur = new Vector2(0, moveSpeedCur.y);
        }
        if (moveSpeedCur.y < 0 && moveSpeedCur.y * Time.deltaTime + transform.position.z < movementBorders.y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, movementBorders.y);
            moveSpeedCur = new Vector2(moveSpeedCur.x, 0);
        }
        else if (moveSpeedCur.y > 0 && moveSpeedCur.y * Time.deltaTime + transform.position.z > movementBorders.height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, movementBorders.height);
            moveSpeedCur = new Vector2(moveSpeedCur.x, 0);
        }
        transform.Translate(moveSpeedCur.x * Time.deltaTime, 0, moveSpeedCur.y * Time.deltaTime);
    }

    // Movement For if the Mouse Goes to a certain area oncreen
    void MouseBorderMovement()
    {
        if (Input.mousePosition.x < mouseBorders.x)
        {
            moveSpeedCur = new Vector2(-moveSpeed.x, moveSpeedCur.y);
        }
        else if (Input.mousePosition.x > mouseBorders.width)
        {
            moveSpeedCur = new Vector2(moveSpeed.x, moveSpeedCur.y);
        }
        if (Input.mousePosition.y < mouseBorders.y)
        {
            moveSpeedCur = new Vector2(moveSpeedCur.x, -moveSpeed.y);
        }
        else if (Input.mousePosition.y > mouseBorders.height)
        {
            moveSpeedCur = new Vector2(moveSpeedCur.x, moveSpeed.y);
        }
    }

    // Keyboard based movement
    void KeyMovement()
    {
        moveSpeedCur = new Vector2(Input.GetAxis("Horizontal") * moveSpeed.x, moveSpeedCur.y);
        moveSpeedCur = new Vector2(moveSpeedCur.x, Input.GetAxis("Vertical") * moveSpeed.y);
        if (Input.GetButton("IncreaseMoveSpeed"))
        {
            moveSpeedCur = moveSpeedCur * modifier;
        }
    }

    // Used for Scrollwheel functionality
    void ScrollWheel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll * scrollSpeed + scrollCur > scrollMinMax.y)
        {
            amount = scrollMinMax.y - scrollCur;
        }
        else if (scroll * scrollSpeed + scrollCur < scrollMinMax.x)
        {
            amount = scrollMinMax.x - scrollCur;
        }
        else
        {
            amount = scroll * scrollSpeed;
        }
        scrollCur += amount;
        cam.transform.position = cam.transform.position + cam.transform.forward * amount;
    }
}
