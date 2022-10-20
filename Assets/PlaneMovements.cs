using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaneMovements : MonoBehaviour
{
    //Variable for checking the distance to the start of the landing zone
    public Transform platform5;

    //Variable for checking the distance to the middle of the landing zone
    public Transform platform3;

    //It is 700 km/h in m/s - initial speed
    public float speed = 194.44444444444f;

    //Variable for rotating the plane when landing
    private bool rotateZero = true;

    //Variable for starting landing
    private bool moveDown = false;

    private bool slowDown = false;

    //Check if speed is 
    private bool slowDowned = false;

    //When the plane will land it should start slowing down to total stop
    private bool totalStop = false;

    //Is plane stopped or not on landing zone
    private bool totalStopped = false;

    //Counter for screenshots
    private int counter = 0;
    void Start()
    {
        Application.targetFrameRate = 60;
        
        //Create the folder 'screenshots' when the game first time started
        try
        {
            if (!Directory.Exists(Application.dataPath + "/screenshots/"))
            {
                Directory.CreateDirectory(Application.dataPath + "/screenshots/");
            }
 
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        
        //Get the distance to the start of the landing zone
        float distance5 = Vector3.Distance(platform5.position, pos);

        //Get the distance to the middle of the landing zone
        float distance3 = Vector3.Distance(platform3.position, pos);

        //Rotate the plane when landing
        RotatePlane();

        //If the distance to the start of the landing zone is or less than 1500 meters than is should slowdown to
        //250 km/h or 69.4(4) m/s. It could happen earlier if the player push the 'G' button
        if (distance5 <= 1500.0f || Input.GetKeyDown(KeyCode.G))
        {
            slowDown = true;
        }

        //First slowdown to 250 km/h or 69.4(4) m/s
        if (slowDown && !slowDowned)
        {
            float changeSpeed = Random.Range(-15.0f, -9.5f) * Time.deltaTime;
            speed += changeSpeed;
            if (speed <= 69.44444444444f)
            {
                speed = 69.44444444444f;
                slowDowned = true;
            }
        }

        //Second slowdown to total stop at landing zone
        if (totalStop && !totalStopped)
        {
            float changeSpeed = Random.Range(-8.0f, -6.5f) * Time.deltaTime;
            speed += changeSpeed;
            if (speed <= 0f)
            {
                speed = 0f;
                totalStopped = true;
            }
        }

        //If the plane near the middle of the landing zone, than it should movedown
        if (distance3 <= 190f)
        {
            moveDown = true;
        }

        //If the plane is on the landing zone than it should stop moving down
        if (transform.position.y <= 245f)
        {
            moveDown = false;
        }

        //Function for moving the plane
        MovePlane();
        
        //Function for making screenshots
        StartCoroutine(TakeScreenShot());
    }

    void RotatePlane()
    {
        if (moveDown && rotateZero)
        {
            Vector3 rot = new Vector3(-1.8f,0,0) * Time.deltaTime;
            
            transform.Rotate(rot);

            if (transform.rotation.x <= 0)
            {
                rotateZero = false;
                Vector3 newRotation = new Vector3(0, 0, 0);
                transform.eulerAngles = newRotation;
                totalStop = true;
            }
        }
    }

    void MovePlane()
    {
        if (moveDown)
        {
            transform.Translate(Vector3.down * 100 * 0.15f * Time.deltaTime);
        }
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    //Function for making screenshots
    IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();
        counter++;
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/screenshots/" + 
                                        string.Format(String.Format("{0:D6}", counter)) + ".png");
    }
}