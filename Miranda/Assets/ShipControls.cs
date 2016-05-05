using UnityEngine;
using System.Collections;

public class ShipControls : MonoBehaviour
{
    public Rigidbody rb;

    public bool moveForwardOrBack = true;
    public float speedForward = 0.0f;
    public float speedForwardCap = 25.0f;
    public float speedForwardAcc = 4.0f;
    public float speedForwardDec = 4.0f;
    public float speedBack = 0.0f;
    public float speedBackCap = 25.0f;
    public float speedBackAcc = 4.0f;
    public float speedBackDec = 4.0f;
    public bool moveLeftOrRight = true;
    public float speedLeft = 0.0f;
    public float speedLeftCap = 25.0f;
    public float speedLeftAcc = 4.0f;
    public float speedLeftDec = 4.0f;
    public float speedRight = 0.0f;
    public float speedRightCap = 25.0f;
    public float speedRightAcc = 4.0f;
    public float speedRightDec = 4.0f;
    public bool rotateLeftOrRight = true;
    public float rotateSpeedLeft = 0.0f;
    public float rotateSpeedLeftCap = 25.0f;
    public float rotateSpeedLeftAcc = 4.0f;
    public float rotateSpeedLeftDec = 4.0f;
    public float rotateSpeedRight = 0.0f;
    public float rotateSpeedRightCap = 25.0f;
    public float rotateSpeedRightAcc = 4.0f;
    public float rotateSpeedRightDec = 4.0f;
    public float defaultSpeedDec = 4.0f;

    public float mousespeed = 5.0f;
    public GameObject interactFeild = null;
    public int CurrentLight = 0;
    public GameObject ambeintLight = null;
    public GameObject spotLight = null;

    void Start ()
    {
        //Screen.lockCursor = true;
        //Cursor.visible = false;

        rb = GetComponent <Rigidbody> ();
	}
	
	void Update ()
    {

        {//ALL MOVEMENT

            //PITCH AND YAW MOVEMENT
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), 0, Input.GetAxis("Mouse X")) * -1 * mousespeed);

            //FORWARD AND BACK MOVEMENT
            if (speedForward >= speedForwardCap)
            {
                speedForward = speedForwardCap;
            }
            if (speedForward <= 0.0f)
            {
                speedForward = 0.0f;
            }
            if (speedBack >= speedBackCap)
            {
                speedBack = speedBackCap;
            }
            if (speedBack <= 0.0f)
            {
                speedBack = 0.0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                speedBackDec = defaultSpeedDec * 2;
            }
            else
            {
                speedBackDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.W) && speedBack <= 0)
            {
                moveForwardOrBack = true;
            }
            if (moveForwardOrBack == true)
            {
                MoveForward();
            }
            if (Input.GetKey(KeyCode.S))
            {
                speedForwardDec = defaultSpeedDec * 2;
            }
            else
            {
                speedForwardDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.S) && speedForward <= 0)
            {
                moveForwardOrBack = false;
            }
            if (moveForwardOrBack == false)
            {
                MoveBackward();
            }

            //LEFT AND RIGHT MOVEMENT
            if (speedLeft >= speedLeftCap)
            {
                speedLeft = speedLeftCap;
            }
            if (speedLeft <= 0.0f)
            {
                speedLeft = 0.0f;
            }
            if (speedRight >= speedRightCap)
            {
                speedRight = speedRightCap;
            }
            if (speedRight <= 0.0f)
            {
                speedRight = 0.0f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                speedRightDec = defaultSpeedDec * 2;
            }
            else
            {
                speedRightDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.A) && speedRight <= 0)
            {
                moveLeftOrRight = true;
            }
            if (moveLeftOrRight == true)
            {
                MoveLeft();
            }
            if (Input.GetKey(KeyCode.D))
            {
                speedLeftDec = defaultSpeedDec * 2;
            }
            else
            {
                speedLeftDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.D) && speedLeft <= 0)
            {
                moveLeftOrRight = false;
            }
            if (moveLeftOrRight == false)
            {
                MoveRight();
            }

            //ROLL MOVEMENT
            if (rotateSpeedLeft >= rotateSpeedLeftCap)
            {
                rotateSpeedLeft = rotateSpeedLeftCap;
            }
            if (rotateSpeedLeft <= 0.0f)
            {
                rotateSpeedLeft = 0.0f;
            }
            if (rotateSpeedRight >= rotateSpeedRightCap)
            {
                rotateSpeedRight = rotateSpeedRightCap;
            }
            if (rotateSpeedRight <= 0.0f)
            {
                rotateSpeedRight = 0.0f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                rotateSpeedRightDec = defaultSpeedDec * 2;
            }
            else
            {
                rotateSpeedRightDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.Q) && rotateSpeedRight <= 0)
            {
                rotateLeftOrRight = true;
            }
            if (rotateLeftOrRight == true)
            {
                RollLeft();
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotateSpeedLeftDec = defaultSpeedDec * 2;
            }
            else
            {
                rotateSpeedLeftDec = defaultSpeedDec;
            }
            if (Input.GetKey(KeyCode.E) && rotateSpeedLeft <= 0)
            {
                rotateLeftOrRight = false;
            }
            if (rotateLeftOrRight == false)
            {
                RollRight();
            }
        }

        //Interact Action
        if (Input.GetKey(KeyCode.F))
        {
            interactFeild.SetActive(true);
        }
        else
        {
            interactFeild.SetActive(false);
        }

        //LIGHT ACTION
        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentLight++;
        }
        if (CurrentLight == 0)
        {
            ambeintLight.SetActive(false);
            spotLight.SetActive(false);
        }
        if (CurrentLight == 1)
        {
            ambeintLight.SetActive(true);
            spotLight.SetActive(false);
        }
        if (CurrentLight == 2)
        {
            ambeintLight.SetActive(false);
            spotLight.SetActive(true);
        }
        if (CurrentLight >= 3)
        {
            CurrentLight = 0;
        }

        //STABALIZE ACTION
        if (Input.GetKey(KeyCode.X))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }

    void MoveForward()
    {//FORWARD MOVEMENT

        if (Input.GetKey(KeyCode.W))
        {
            rb.MovePosition (rb.position + transform.up * speedForward * Time.deltaTime);
            speedForward = speedForward + Time.deltaTime * speedForwardAcc;
        }
        else
        {
            rb.MovePosition (rb.position + transform.up * speedForward * Time.deltaTime);
            speedForward = speedForward - Time.deltaTime * speedForwardDec;
        }
    }

    void MoveBackward()
    {//BACKWARD MOVEMENT
        
        if (Input.GetKey(KeyCode.S))
        {
            rb.MovePosition(rb.position + transform.up * speedBack * Time.deltaTime * -1);
            speedBack = speedBack + Time.deltaTime * speedBackAcc;
        }
        else
        {
            rb.MovePosition (rb.position + transform.up * speedBack * Time.deltaTime * -1);
            speedBack = speedBack - Time.deltaTime * speedBackDec;
        }
    }

    void MoveLeft()
    {//LEFT MOVEMENT

        if (Input.GetKey(KeyCode.A))
        {
            rb.MovePosition(rb.position + transform.right * speedLeft * Time.deltaTime * -1);
            speedLeft = speedLeft + Time.deltaTime * speedLeftAcc;
        }
        else
        {
            rb.MovePosition(rb.position + transform.right * speedLeft * Time.deltaTime * -1);
            speedLeft = speedLeft - Time.deltaTime * speedLeftDec;
        }
    }

    void MoveRight()
    {//RIGHT MOVEMENT

        if (Input.GetKey(KeyCode.D))
        {
            rb.MovePosition(rb.position + transform.right * speedRight * Time.deltaTime);
            speedRight = speedRight + Time.deltaTime * speedRightAcc;
        }
        else
        {
            rb.MovePosition(rb.position + transform.right * speedRight * Time.deltaTime);
            speedRight = speedRight - Time.deltaTime * speedRightDec;
        }
    }

    void RollLeft()
    {//ROTATE LEFT
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, 1, 0) * rotateSpeedLeft * Time.deltaTime);
            rotateSpeedLeft = rotateSpeedLeft + Time.deltaTime * rotateSpeedLeftAcc;
        }
        else
        {
            transform.Rotate(new Vector3(0, 1, 0) * rotateSpeedLeft * Time.deltaTime);
            rotateSpeedLeft = rotateSpeedLeft - Time.deltaTime * rotateSpeedLeftDec;
        }
    }

    void RollRight()
    {//ROTATE RIGHT
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, -1, 0) * rotateSpeedRight * Time.deltaTime);
            rotateSpeedRight = rotateSpeedRight + Time.deltaTime * rotateSpeedRightAcc;
        }
        else
        {
            transform.Rotate(new Vector3(0, -1, 0) * rotateSpeedRight * Time.deltaTime);
            rotateSpeedRight = rotateSpeedRight - Time.deltaTime * rotateSpeedRightDec;
        }
    }
}