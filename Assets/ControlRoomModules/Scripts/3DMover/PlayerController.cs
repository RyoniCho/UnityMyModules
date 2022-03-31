using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4;
    public float turnSpeed = 100;
    public Transform playerAttackPos;
    public GameObject bullet;
    public float AttackSpeed = 1f;

    float angleY = 0f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Transform camTransform;

    float horizontalValue;
    float verticalValue;

    private ControlRoom.PlayerInputAction playerInputAction;
    bool holdInteractionPerformed = false;

    private void Awake()
    {
        SetPlayerControl();
    }

    public void OnEnable()
    {
        playerInputAction.Enable();
    }

    public void OnDisable()
    {
        playerInputAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.camTransform = Camera.main.transform;
        //InvokeRepeating("Attack", 0f, AttackSpeed);
   
    }

    // Update is called once per frame
    void Update()
    {
        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalValue, 0f, verticalValue).normalized;

        if(direction.magnitude>=0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            targetAngle += camTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //this.transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            this.transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            this.transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime,Space.World);
        }

        //this.transform.position += this.transform.forward * v * moveSpeed * Time.deltaTime; //Self Movement

        //Vector3 moveVec = new Vector3(h*moveSpeed*Time.deltaTime, 0, v * moveSpeed * Time.deltaTime);
        //this.transform.Translate(moveVec, Space.World);

        //angleY += turnSpeed * h * Time.deltaTime;
        //this.transform.rotation = Quaternion.Euler(0, angleY, 0f);

        //if(!Mathf.Approximately(0f,h)&&!Mathf.Approximately(0f,v))
        //    this.transform.rotation = Quaternion.LookRotation(moveVec * turnSpeed * Time.deltaTime);


        

    }

    //Player Input Event

    private void OnMovement(InputValue inputValue)
    {
        Vector2 movementValue = inputValue.Get<Vector2>();
        horizontalValue = movementValue.x;
        verticalValue = movementValue.y;
    }

    private void OnFire(InputValue value)
    {
       
        Debug.Log($"Fire Button Down");
    }



    private void SetPlayerControl()
    {
        playerInputAction = new ControlRoom.PlayerInputAction();
       
        playerInputAction.Player.LongFire.started += (context) =>
        {
            
        };

        playerInputAction.Player.LongFire.performed += (context) =>
        {
            if (context.interaction is UnityEngine.InputSystem.Interactions.HoldInteraction)
            {


                holdInteractionPerformed = true;
            }
        };

        playerInputAction.Player.LongFire.canceled += (context) =>
        {
            if (holdInteractionPerformed)
            {
                Debug.Log("Hold Release");
            }
            else
            {
                Debug.Log("Hold Canceled");
            }

            holdInteractionPerformed = false;
        };
    }


    void Attack()
    {
       
        //ControlRoom.PoolManager.Instance.SpawnObject("PlayerBullet", this.playerAttackPos.position, this.playerAttackPos.rotation);
    }
}
