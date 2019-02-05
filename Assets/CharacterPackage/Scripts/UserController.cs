using System;
using JetBrains.Annotations;
using UnityEngine;
using VehiclePackage;

namespace CharacterPackage.Scripts
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HandsIKHandler))]
    [RequireComponent(typeof(FeetIKHandler))]
//    Main Collider
    [RequireComponent(typeof(CapsuleCollider))]
    public class UserController : MonoBehaviour {
        [SerializeField] private float runMultiplierSpeed = 2.1f;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        [SerializeField] private float jumpForce = 2.1f;
        public float baseSpeed = 2.0F;
        public float reachDistance = 20.0F;
        public float rotationSpeed = 100.0F;
        //for vehicles
        [CanBeNull] private VehicleController vehicleController;
        private bool inVehicle;
        private Seat assignedSeat;
        //Animation & IK
        private static Animator anim;
        private Rigidbody rb;
        private HandsIKHandler handsIK;
        private FeetIKHandler feetIK;
        private CapsuleCollider capsuleCollider;
        //Running
        private bool isRunningToggled;
        private bool isFocused = false;
        private bool isRunning;
        private bool hasWeapon = false;
        private float currentSpeed;

        //Animator properties
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsTurningLeft = Animator.StringToHash("isTurningLeft");
        private static readonly int IsTurningRight = Animator.StringToHash("isTurningRight");
        private static readonly int InCar = Animator.StringToHash("inCar");
        private static readonly int YPos = Animator.StringToHash("yPos");
        private static readonly int IsFocused = Animator.StringToHash("isFocused");

        //Main Camera
        private Camera m_Camera;

        public void Start ()
        {
            inVehicle = false;
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            handsIK = GetComponent<HandsIKHandler>();
            feetIK = GetComponent<FeetIKHandler>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }
	
        // Update is called once per frame
        private void Update () {
            if (inVehicle)
            {
                if (IsDriver())
                {
                    handsIK.enabled = true;
                }
                
                feetIK.enabled = false;
                UpdateSeatTransform();
            }
            else
            {
                handsIK.enabled = false;
//                feetIK.enabled = true;
                UpdateMovement();
                UpdateRotation();
            }
            UseHandler();
            FocusUpdate();
        }

        private void UpdateMovement()
        {
            var translation = GetUserBaseTranslation();
            /**TODO::
             * JUMP
             */
//            Debug.Log("anim ypos:" + anim.GetFloat(YPos));
            if (Input.GetButtonDown("Jump") && Math.Abs(translation) < 0.01f) {
                Debug.Log("jump force:" + jumpForce);
                rb.AddForce(transform.up * anim.GetFloat(YPos) * 1000 * jumpForce, ForceMode.VelocityChange);
                anim.SetTrigger(IsJumping);
            }

            /**
            * On caps trigger to run
            */
            if (Input.GetButtonDown("RunTrigger"))
            {
                isRunningToggled = !isRunningToggled;
            }
        
            /**
            * WALK
            */
            if (Math.Abs(translation) < 0.01f)
            {
                //No translation - no walking and running
                anim.SetBool(IsWalking, false);
                anim.SetBool(IsRunning, false);
            }
            else
            {
                anim.SetBool(IsWalking, true);
                currentSpeed = translation;

                //RUN
                if (Input.GetButton("Run"))
                {
                    isRunning = !isRunningToggled;
                }
                else 
                {
                    isRunning = isRunningToggled;
                }
                anim.SetBool(IsRunning, isRunning);

                if (isRunning)
                {
                    currentSpeed *= runMultiplierSpeed;
                }

                /**
             * JUMP ON THE RUN
             */
                if (Input.GetButtonDown("Jump"))
                {
                    anim.SetTrigger(IsJumping);
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                }
                transform.Translate(0, 0, currentSpeed);
            }
       
        }

        private void UpdateRotation()
        {
            var rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            if (Math.Abs(rotation) < 0.01)
            {
                anim.SetBool(IsTurningLeft,false);
                anim.SetBool(IsTurningRight,false);
                return;
            }
            //- = left + = right
            if (rotation > 0)
            {
                anim.SetBool(IsTurningRight,true);
            }
            else
            {
                anim.SetBool(IsTurningLeft,true);
            }

            transform.Rotate(0, rotation, 0);
        }

        private void FocusUpdate()
        {
            if (Input.GetButton("Focus"))
            {
                anim.SetTrigger(IsFocused);
            }
        }

        private void UseHandler()
        {
            if (Input.GetButtonDown("Use"))
            {
                if (inVehicle)
                {
                    ExitVehicle();
                    return;
                }

                var cam = m_Camera.transform;
                var ray = new Ray(cam.position, cam.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, reachDistance))
                {
                    if (hit.transform.CompareTag("Vehicle") && !inVehicle)
                    {
                        EnterVehicle(hit.transform.gameObject);
                    }
                    //Debug.Log(hit.transform.tag);
                    //Debug.DrawLine(ray.origin, hit.point, Color.green,2f); 
                }
            }
        }

        private void UpdateSeatTransform()
        {
            var seatPos = assignedSeat.placePosition.position;
            var seatRot = assignedSeat.placePosition.rotation;
            var playerTransform = transform;
            playerTransform.position = new Vector3(seatPos.x, seatPos.y, seatPos.z);
            playerTransform.rotation = seatRot;
            
        }

        internal float GetSpeed()
        {
            return currentSpeed;
        }
        
        internal VehicleController GetVehicleController()
        {
            return vehicleController;
        }
        
        internal bool IsInVehicle()
        {
            return inVehicle;
        }

        private bool IsDriver()
        {
 
            if (!inVehicle || !vehicleController) return false;
            return vehicleController.IsDriver(gameObject);

        }

        [CanBeNull]
        public Seat GetSeat()
        {
            return assignedSeat;
        }

        private void EnterVehicle(GameObject veh)
        {
            //TODO:: refactor
            vehicleController = veh.GetComponent<VehicleController>();
            assignedSeat = vehicleController.SetPlayer(gameObject);
            if (assignedSeat == null)
            {
                Debug.Log("No place in vehicle");
            }
            else
            {
                anim.SetBool(InCar,true);
                capsuleCollider.enabled = false;
                inVehicle = true;
                anim.SetBool(IsWalking,false);
                anim.SetBool(IsRunning,false);
            }
        }
        
        private void ExitVehicle()
        {
            if (!vehicleController) return;
            vehicleController.RemovePlayer(gameObject);
            capsuleCollider.enabled = true;
            inVehicle = false;
            anim.SetBool(IsWalking, false);
            anim.SetBool(IsRunning, false);
            anim.SetBool(InCar, false);
            
            gameObject.transform.position = vehicleController.getExitPosition();

        }

        internal float GetUserBaseTranslation()
        {
            return Input.GetAxis("Vertical") * baseSpeed * Time.deltaTime;
        }

    }
}
