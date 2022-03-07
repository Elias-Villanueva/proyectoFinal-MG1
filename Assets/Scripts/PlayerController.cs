using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    public CharacterController charCon;

    private Vector3 moveInput;

    public Transform camTrans;

    public Animator anim;

    [Header("Gravity")]
    public float gravityModifier;

    [Header("Movement Control")]
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    private bool canJump, canDobleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    [Header("Camara Control")]
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;

    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    public int currentGun;

    public Transform aimPoint, gunHolder;
    private Vector3 gunStartPos;
    public float aimSpeed;

    
   

    void Awake() 
   {
       instance = this;
   }
    void Start()
    {
        currentGun--;
        SwitchGun();

        gunStartPos = gunHolder.localPosition;
    }

    
    void Update()
    {
        if(!UIController.instance.pauseScreen.activeInHierarchy)
        {
        
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // guardar Y velocity

        float yStore = moveInput.y;

        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horiMove + vertMove;
        //moveInput.Normalize();

        if(Input.GetButton("Run"))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }
        

        moveInput.y = yStore;


        // Gravedad
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
        
        if(charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        

        //canJump = charCon.isGrounded;
        

        //if(canJump)
        //{
         //   canDobleJump = false;
        //}

        canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

        // Salto del jugador

        if(Input.GetButtonDown("Jump") && canJump)
        {
            moveInput.y = jumpPower;

            canDobleJump = true;

            AudioManager.instance.PlaySFX(15);
        } 
        else if(canDobleJump && (Input.GetButtonDown("Jump")))
        {
            moveInput.y = jumpPower;
            canDobleJump = false;
            AudioManager.instance.PlaySFX(15);
        }

        charCon.Move(moveInput * Time.deltaTime);

        //Control Rotacion Camara
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        if(invertX)
        {
            mouseInput.x = -mouseInput.x;
        }

        if(invertY)
        {
            mouseInput.y = -mouseInput.y;
        }
        

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
        

        //Shooting
        // Disparo Singular
        if(Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;

            if(Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
            {
                if(Vector3.Distance(camTrans.position, hit.point) > 2f)
                {
                    activeGun.firePoint.LookAt(hit.point);
                }
            }else
            {
                activeGun.firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
            }

            FireShot();
            
        }

        //Disparo Automatico
        if(Input.GetMouseButton(0) && activeGun.canAutoFire)
        {
            if(activeGun.fireCounter <= 0)
            {
                FireShot();
            }
        }

        if(Input.GetButtonDown("Switch Gun"))
        {
            SwitchGun();
        }

        if(Input.GetMouseButtonDown(1))
        {
            CamaraController.instance.ZoomIn(activeGun.zoomAmount);
        }

        if(Input.GetMouseButton(1))
        {
            gunHolder.position = Vector3.MoveTowards(gunHolder.position, aimPoint.position, aimSpeed * Time.deltaTime);
        }else
        {
            gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, aimSpeed * Time.deltaTime);
        }

        if(Input.GetMouseButtonUp(1))
        {
            CamaraController.instance.ZoomOut();
        }

        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);
        }
    }
    

    public void FireShot()
    {
        if(activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;

            Instantiate(activeGun.bullet, activeGun.firePoint.position, activeGun.firePoint.rotation);

            activeGun.fireCounter = activeGun.fireRate;

            UIController.instance.ammoText.text = "Ammo : " + activeGun.currentAmmo;

        }
        
    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);

        currentGun++;

        if(currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "Ammo : " + activeGun.currentAmmo;
    }

    public void AddGun(string gunToAdd)
    {
        bool gunUnlucked = false;

        if(unlockableGuns.Count > 0)
        {
            for(int i = 0; i < unlockableGuns.Count; i++)
            {
                gunUnlucked = true;

                allGuns.Add(unlockableGuns[i]);

                unlockableGuns.RemoveAt(i);

                i = unlockableGuns.Count;
            }
        }

        if(gunUnlucked)
        {
            currentGun = allGuns.Count - 2;

            SwitchGun();
        }
    }
}
