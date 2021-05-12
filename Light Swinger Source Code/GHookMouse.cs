using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.InputSystem;

////////////////////////////////////////////////////////////////////////////////
// File name:		<GHookMouse>
//
// Purpose:			<Grappling hook>
//
// Related Files:	<File list>
//
// Author:			<Chris Severino>
//
// Created:			<Creation data>
//
// Last Modified:	<7/14/2020>
////////////////////////////////////////////////////////////////////////////////

public class GHookMouse : MonoBehaviour
{
    Vector3 targetPos;
    RaycastHit2D hit;

    DistanceJoint2D joint;
    public Transform hookTarget;
    public ParticleSystem breaking;
    public float maxdistance = 5f;
    public LayerMask hookMask;
    public LineRenderer line;
    public LineRenderer enemyLine;
    public float step = 0.05f;
    public float reelLength = 2f;
    public Transform guntip = null;

    //enemy grappling variables
    public LayerMask enemyMask;
    public float maxGrappleTimer = 1f;
    public float currentGrappleTimer = 0f;
    private bool isGrappleEnemy = false;
    private bool isGrappleMiniHook = false;
    RaycastHit2D enemyHit;
    GameObject enemy;
    GameObject miniBossHook;

    public int angleBreakLeft = 180;
    public int angleBreakRight = 0;
    private float angle = 0;
    private Vector2 anglecheck;

    private float grappleDelay = 0;
    private float maxGrappleDelay = 1.1f;
    private bool isGrapple = false;

    DeviceDetectionTest deviceCurrent;
    public Animator anim;


    #region controller setup
    PlayerControls controls;
    Vector2 inputVector;
    Vector2 inputVectorEnemy;
    Vector2 mousePos;


    private void Awake()
    {
        deviceCurrent = GetComponent<DeviceDetectionTest>();
        controls = new PlayerControls();
        controls.GrappleControls.Grapple.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        controls.GrappleControls.GrappleEnemy.performed += ctxEnemy => inputVectorEnemy = ctxEnemy.ReadValue<Vector2>();
        controls.GrappleControls.MousePosition.performed += ctx => mousePos = ctx.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        controls.GrappleControls.Enable();
    }

    private void OnDisable()
    {
        controls.GrappleControls.Enable();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(inputVectorEnemy);
        if (GetComponent<PlayerScript>().GetHealth() <= 0)
        {
            joint.enabled = false;
            line.enabled = false;
        }

        if (joint.distance > reelLength)
        {
            //reels player in towards target
            joint.distance -= step;
        }
        else if (joint.distance < reelLength)
        {
            joint.distance = reelLength; //Keeps it so the grapple only reels to a certain length
        }
        else if (isGrapple == true && grappleDelay <= 0 && angle <= angleBreakRight || angle >= angleBreakLeft)
        {
            GrappleBreak(); //Break Function if the player exceeds the max angle they can swing
        }
        else if (inputVector.x == 0)
        {
            //releases the grappling hook is ceratin conditions are met
            if (isGrapple)
            {
                GetComponent<AudioManager>().effectsAudio[5].Play();
            }
            anim.SetBool("isGrapple", false);
            isGrapple = false;
            line.enabled = false;
            joint.enabled = false;
        }


        #region grapple fire
        //grappling hook fire code
        if (inputVector.x > 0 && isGrapple == false && grappleDelay < 0.5f)
        {
            if (deviceCurrent.GetDevice() == "Gamepad")
            {
                targetPos = Camera.main.ScreenToWorldPoint(hookTarget.position);
                targetPos.z = 0;
            }
            else if(deviceCurrent.GetDevice() == "Keyboard")
            {
                targetPos = Camera.main.ScreenToWorldPoint(mousePos);
                targetPos.z = 0;
            }

            //Checks to see if the player shot the hook at a hook
            hit = Physics2D.Raycast(guntip.position, targetPos - guntip.position, maxdistance, hookMask);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null && hit.collider.CompareTag("Hook"))
            {
                anim.SetBool("isGrapple", true);
                isGrapple = true;
                joint.enabled = true;
                joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                joint.distance = Vector2.Distance(guntip.position, hit.point);

                gameObject.GetComponent<AudioManager>().effectsAudio[3].Play(0);

                line.enabled = true;
                line.SetPosition(0, guntip.position);
                line.SetPosition(1, hit.point);
            }
        }
        if (inputVector.x > 0 && grappleDelay <= 0)
        {
            line.SetPosition(0, guntip.position);

            //checks to make sure the angle the player is at doesn't break the grapple
            anglecheck = guntip.position - targetPos;
            if (hit)
            {
                angle = Vector3.Angle(hit.collider.gameObject.transform.right, anglecheck);
            }
        }
        if (isGrapple == true && grappleDelay <= 0 && angle <= angleBreakRight || angle >= angleBreakLeft)
        {
            GrappleBreak(); //Grapple breaks if certain conditions are met
        }
        else if (inputVector.x == 0)
        {
            anim.SetBool("isGrapple", false);
            isGrapple = false;
            joint.enabled = false;
            line.enabled = false;
        }
        #endregion

        #region grapple enemies
        //grappling hook pull for enemy objects
        if (inputVectorEnemy.x > 0 && isGrappleEnemy == false && isGrappleMiniHook == false)
        {
            if (deviceCurrent.GetDevice() == "Gamepad")
            {
                targetPos = Camera.main.ScreenToWorldPoint(hookTarget.position);
                targetPos.z = 0;
            }
            else if (deviceCurrent.GetDevice() == "Keyboard")
            {
                targetPos = Camera.main.ScreenToWorldPoint(mousePos);
                targetPos.z = 0;
            }

            if (enemyHit = Physics2D.Raycast(guntip.position, targetPos - guntip.position, maxdistance, enemyMask))
            {
                if (enemyHit.collider != null && enemyHit.collider.gameObject.CompareTag("Fly Boi"))
                {
                    if (enemyHit.collider.gameObject.CompareTag("Fly Boi"))
                    {
                        if (enemyHit.collider.gameObject.GetComponent<DropperEnemy>().IsProned())
                        {
                            return;
                        }
                    }
                    isGrappleEnemy = true;
                    anim.SetBool("isGrappleEnemy", true);
                    enemy = enemyHit.collider.gameObject;
                    enemy.GetComponent<DistanceJoint2D>().enabled = true;
                    enemy.GetComponent<DistanceJoint2D>().connectedBody = guntip.GetComponent<Rigidbody2D>();
                    enemy.GetComponent<DistanceJoint2D>().connectedAnchor = guntip.position;
                    enemy.GetComponent<DistanceJoint2D>().distance = Vector2.Distance(enemyHit.point, guntip.position);


                    enemy.gameObject.GetComponentInChildren<LineRenderer>().enabled = true;
                    enemy.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(0, enemy.transform.position);
                    enemy.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(1, guntip.position);
                }
            }
            else
            {
                hit = Physics2D.Raycast(guntip.position, targetPos - guntip.position, maxdistance, hookMask);

                if (hit.collider != null && hit.collider.CompareTag("MiniBossHook") && grappleDelay < 0.5f)
                {
                    isGrappleMiniHook = true;
                    anim.SetBool("isGrappleEnemy", true);
                    gameObject.GetComponent<AudioManager>().effectsAudio[3].Play(0);
                    miniBossHook = hit.collider.gameObject;
                    miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().enabled = true;
                    miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(0, miniBossHook.transform.position);
                    miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(1, guntip.position);
                }
            }
        }
        if (enemy)
        {
            if (inputVectorEnemy.x > 0 && isGrappleEnemy == true && enemy.gameObject.CompareTag("Fly Boi"))
            {
                enemy.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(0, enemy.transform.position);
                enemy.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(1, guntip.position);
                currentGrappleTimer -= Time.deltaTime;
            }
            if (inputVectorEnemy.x == 0 || currentGrappleTimer <= 0 && enemy.gameObject.CompareTag("Fly Boi"))
            {
                if (isGrappleEnemy && currentGrappleTimer <= 0)
                    GameObject.Instantiate(breaking, transform.position, transform.rotation);
                isGrappleEnemy = false;
                anim.SetBool("isGrappleEnemy", false);
                enemy.GetComponent<DistanceJoint2D>().enabled = false;
                enemy.gameObject.GetComponentInChildren<LineRenderer>().enabled = false;
                currentGrappleTimer = maxGrappleTimer;
            }
        }
        else if (isGrappleMiniHook == true && miniBossHook != null)
        {
            if (inputVectorEnemy.x > 0)
            {
                miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(0, miniBossHook.transform.position);
                miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().SetPosition(1, guntip.position);
            }
            if (inputVectorEnemy.x == 0)
            {
                isGrappleMiniHook = false;
                anim.SetBool("isGrappleEnemy", false);
                miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().enabled = false;
            }
        }
        #endregion


        if (grappleDelay > 0)
        {
            grappleDelay -= Time.deltaTime;
        }
    }

    public bool IsGrappleEnemy()
    {
        return isGrappleEnemy;
    }

    public bool IsGrapple()
    {
        return isGrapple;
    }

    public bool IsGrappleMiniHook()
    {
        return isGrappleMiniHook;
    }

    void GrappleBreak()
    {
        //Breaks the grabble and turns off objects
        anim.SetBool("isGrapple", false);
        isGrapple = false;
        grappleDelay = maxGrappleDelay;
        GetComponent<AudioManager>().effectsAudio[5].Play();
        GameObject.Instantiate(breaking, transform.position, transform.rotation);
        joint.enabled = false;
        line.enabled = false;
        angle = 90;
    }

    public void GrappleBreakMini()
    {
        //Function to pull off miniBoss hooks
        if (miniBossHook != null)
        {
            anim.SetBool("isGrappleEnemy", false);
            isGrappleMiniHook = false;
            grappleDelay = maxGrappleDelay;
            GameObject.Instantiate(breaking, transform.position, transform.rotation);
            miniBossHook.gameObject.GetComponentInChildren<LineRenderer>().enabled = false;
        }
    }
}
