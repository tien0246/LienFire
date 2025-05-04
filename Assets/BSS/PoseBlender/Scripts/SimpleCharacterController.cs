using System;
using UnityEngine;

namespace BSS.PoseBlender
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleCharacterController : MonoBehaviour
    {
        [Header("Animators")]
        [SerializeField] private Animator animator;
        [SerializeField] private Animator weaponAnimator;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float sprintSpeed = 5f;
        [SerializeField] private float leanSpeed = 3f;

        [Header("Mouse Look Settings")]
        [SerializeField] private float mouseSensitivity = 2f;

        [Header("Pose Blender Settings")]
        [SerializeField] private PoseBlenderLite poseBlenderLite; // Reference to your PoseBlenderLite component
        [SerializeField] private float offsetToRootSpeed = 15f; // Speed at which offset transfers to root
        [SerializeField] private float lookAtSmoothSpeed = 5f; // Speed for smoothly looking at target

        [Header("Root Rotation Settings")]
        [SerializeField] private bool alwaysRotateRoot = false; // New boolean to control root rotation
        [SerializeField] private float maxOffsetRotationSpeed = 50f; // Speed at which to rotate when at max offset

        [Header("Look Offset Clamps")]
        [SerializeField] private Vector2 verticalClamp = new Vector2(-90f, 90f);
        [SerializeField] private Vector2 horizontalClamp = new Vector2(-30f, 30f);
        [SerializeField] private Vector2 leaningClamp = new Vector2(-30f, 30f);

        private CharacterController controller;

        // Track offsets
        private float verticalOffset = 0f;
        private float horizontalOffset = 0f;
        private float leaningOffset = 0f;

        // Movement and look input
        Vector2 moveInput;
        Vector2 lookInput;
        bool sprintInput;
        [Range(-1, 1)] float leanInput;

        // Movement state
        private bool isMoving = false;

        // Previous rotation values for calculations
        private float previousRootYRotation = 0f;

        // Flag to indicate we are in the process of rotating back to zero
        private bool isRotatingToZero = false;
        // Direction of the rotation (positive or negative)
        private float rotationDirection = 0f;

        // Flag to prevent animation from playing multiple times during a single turn
        private bool hasTurnAnimationPlayed = false;

        private void Start()
        {
            // Get the CharacterController component
            controller = GetComponent<CharacterController>();

            // Ensure animator is assigned
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            // If no PoseBlenderLite is assigned, try to get it from this GameObject
            if (poseBlenderLite == null)
            {
                poseBlenderLite = GetComponent<PoseBlenderLite>();
            }

            // Initialize values
            if (poseBlenderLite != null)
            {
                poseBlenderLite.lookVerticalOffset = 0f;
                poseBlenderLite.lookHorizontalOffset = 0f;
                poseBlenderLite.leaningOffset = 0f;
            }

            // Store initial rotation
            previousRootYRotation = transform.eulerAngles.y;

            // Lock and hide the cursor for typical FPS control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleInput();

            // Check if look-at mode is enabled in PoseBlenderLite
            if (poseBlenderLite != null && poseBlenderLite.enableLookAtMode && poseBlenderLite.lookAtTarget != null)
            {
                HandleLookAtTarget();
            }
            else
            {
                HandleMouseLook();
            }

            HandleRootRotation();
            HandleMovement();
            HandleWeaponAnimations();
        }

        private void HandleWeaponAnimations()
        {
            if (Input.GetKeyDown(KeyCode.R))
                if (weaponAnimator != null)
                    weaponAnimator.SetTrigger("Reload");
        }

        private void PlayTurnAnimation(bool turnRight)
        {
            // Only play animation if we haven't already played one for this turn
            if (!hasTurnAnimationPlayed)
            {
                // Play appropriate turn animation
                string animationName = turnRight ? "Turn Right" : "Turn Left";
                animator.Play(animationName);

                // Set flag to prevent replaying during this turn
                hasTurnAnimationPlayed = true;
            }
        }

        private void HandleInput()
        {
            // Skip updating look and move input if look-at mode is enabled
            if (!(poseBlenderLite != null && poseBlenderLite.enableLookAtMode && poseBlenderLite.lookAtTarget != null))
            {
                // Capture mouse input
                lookInput.x = Input.GetAxis("Mouse X") * mouseSensitivity;
                lookInput.y = Input.GetAxis("Mouse Y") * mouseSensitivity;
                // Capture movement input
                moveInput.x = Input.GetAxis("Horizontal");
                moveInput.y = Input.GetAxis("Vertical");
            }

            // Determine if we're moving
            isMoving = moveInput.sqrMagnitude > 0.1f;

            // Capture leaning input
            leanInput = Input.GetKey(KeyCode.E) ? 1 : 0;
            leanInput = Input.GetKey(KeyCode.Q) ? -1 : leanInput;

            sprintInput = Input.GetKey(KeyCode.LeftShift);

            // Skip updating offsets if in look-at mode
            if (!(poseBlenderLite != null && poseBlenderLite.enableLookAtMode && poseBlenderLite.lookAtTarget != null))
            {
                // Update the vertical offset (pitch)
                verticalOffset -= lookInput.y;
                verticalOffset = Mathf.Clamp(verticalOffset, verticalClamp.x, verticalClamp.y);

                // If we're using alwaysRotateRoot, apply horizontal input directly to root rotation
                if (alwaysRotateRoot)
                {
                    // Apply horizontal look directly to root rotation
                    transform.Rotate(0, lookInput.x, 0);

                    // Keep horizontalOffset at zero when always rotating root
                    horizontalOffset = 0f;
                }
                // Otherwise use the standard offset logic
                else
                {
                    // Update the horizontal offset (yaw) - we now allow input even during rotation if it's in opposite direction
                    if (!isMoving)
                    {
                        if (!isRotatingToZero || Mathf.Sign(lookInput.x) != rotationDirection)
                        {
                            horizontalOffset += lookInput.x;
                            horizontalOffset = Mathf.Clamp(horizontalOffset, horizontalClamp.x, horizontalClamp.y);

                            // If we're rotating to zero but got input in opposite direction, cancel the auto-rotation
                            if (isRotatingToZero && Mathf.Sign(lookInput.x) != rotationDirection && Mathf.Abs(lookInput.x) > 0.1f)
                            {
                                isRotatingToZero = false;
                                // Reset animation flag when we cancel a turn
                                hasTurnAnimationPlayed = false;
                            }
                        }
                    }
                }
            }

            // Calculate leaning
            float targetLean = leanInput * leaningClamp.y;
            leaningOffset = Mathf.Lerp(leaningOffset, targetLean, Time.deltaTime * leanSpeed);
        }

        private void HandleLookAtTarget()
        {
            if (poseBlenderLite == null || poseBlenderLite.lookAtTarget == null) return;

            // Get the target position
            Vector3 targetPosition = poseBlenderLite.lookAtTarget.position;

            // Calculate direction to target
            Vector3 targetDirection = targetPosition - transform.position;

            // Calculate horizontal and vertical angles to the target
            float horizontalAngle = Vector3.SignedAngle(transform.forward, new Vector3(targetDirection.x, 0, targetDirection.z).normalized, Vector3.up);

            // For vertical angle, we need to get the angle in the local space of the character
            Vector3 localTargetDir = transform.InverseTransformDirection(targetDirection);
            float verticalAngle = Mathf.Atan2(localTargetDir.y, new Vector2(localTargetDir.z, localTargetDir.x).magnitude) * Mathf.Rad2Deg;

            // Check if we need to rotate the root because the target is outside our view range
            if (Mathf.Abs(horizontalAngle) > horizontalClamp.y)
            {
                // Calculate how much we need to rotate
                float excessAngle = Mathf.Sign(horizontalAngle) * (Mathf.Abs(horizontalAngle) - horizontalClamp.y);

                // Rotate the root by this amount (with smoothing)
                transform.Rotate(0, excessAngle * Time.deltaTime * offsetToRootSpeed, 0);

                // After rotating, we need to recalculate horizontal angle
                targetDirection = targetPosition - transform.position;
                horizontalAngle = Vector3.SignedAngle(transform.forward, new Vector3(targetDirection.x, 0, targetDirection.z).normalized, Vector3.up);
            }

            // Smooth the transition to these angles
            horizontalOffset = Mathf.Lerp(horizontalOffset, Mathf.Clamp(horizontalAngle, horizontalClamp.x, horizontalClamp.y), Time.deltaTime * lookAtSmoothSpeed);
            verticalOffset = Mathf.Lerp(verticalOffset, Mathf.Clamp(verticalAngle, verticalClamp.x, verticalClamp.y), Time.deltaTime * lookAtSmoothSpeed);

            // Apply the offsets to the PoseBlenderLite
            poseBlenderLite.lookHorizontalOffset = horizontalOffset;
            poseBlenderLite.lookVerticalOffset = verticalOffset;
            poseBlenderLite.leaningOffset = leaningOffset;
        }

        private void HandleRootRotation()
        {
            // If alwaysRotateRoot is enabled, we've already applied rotation in HandleInput
            if (alwaysRotateRoot && !(poseBlenderLite != null && poseBlenderLite.enableLookAtMode && poseBlenderLite.lookAtTarget != null))
            {
                // Just update previous rotation for next frame
                previousRootYRotation = transform.eulerAngles.y;
                return;
            }

            // For look-at mode, handle it like we're at max offset if target is outside our view range
            bool inLookAtMode = poseBlenderLite != null && poseBlenderLite.enableLookAtMode && poseBlenderLite.lookAtTarget != null;

            // Standard root rotation logic when alwaysRotateRoot is false
            // Check if we should start rotating back to zero
            if (!isRotatingToZero && !isMoving)
            {
                bool atMaxPositiveOffset = horizontalOffset >= horizontalClamp.y - 0.1f && (inLookAtMode || lookInput.x > 0);
                bool atMaxNegativeOffset = horizontalOffset <= horizontalClamp.x + 0.1f && (inLookAtMode || lookInput.x < 0);

                if (atMaxPositiveOffset || atMaxNegativeOffset)
                {
                    // Begin the rotation to zero process
                    isRotatingToZero = true;
                    rotationDirection = Mathf.Sign(horizontalOffset);
                    hasTurnAnimationPlayed = false; // Reset animation flag at the start of a new turn

                    // Play turn animation based on rotation direction (only when not moving)
                    PlayTurnAnimation(rotationDirection > 0);
                }
            }

            // If we're in the process of rotating to zero
            if (isRotatingToZero)
            {
                // Apply rotation regardless of other conditions
                float rotationAmount = rotationDirection * maxOffsetRotationSpeed * Time.deltaTime;

                // Apply rotation to the character's root
                transform.Rotate(0, rotationAmount, 0);

                // Counter-rotate the horizontal offset to keep camera steady
                horizontalOffset -= rotationAmount;

                // Check if we've completed the rotation
                if ((rotationDirection > 0 && horizontalOffset <= 0) ||
                    (rotationDirection < 0 && horizontalOffset >= 0))
                {
                    // We've reached zero or passed it
                    horizontalOffset = 0f;
                    isRotatingToZero = false;
                    hasTurnAnimationPlayed = false; // Reset animation flag when rotation completes
                }

                // If in look-at mode, we need to recalculate look target after rotating
                if (inLookAtMode)
                {
                    // We'll call HandleLookAtTarget again after rotation
                    HandleLookAtTarget();
                }
                // Allow additional mouse input to influence the root rotation during this process
                else if (Mathf.Abs(lookInput.x) > 0.1f)
                {
                    // If the input is in the opposite direction of our rotation
                    if (Mathf.Sign(lookInput.x) != rotationDirection)
                    {
                        // We allow this to affect the root rotation directly
                        transform.Rotate(0, lookInput.x, 0);
                    }
                    else
                    {
                        // Same direction as before, speed up the rotation
                        transform.Rotate(0, lookInput.x, 0);
                    }
                }
            }
            else if (isMoving)
            {
                // Only transfer horizontal offset to root rotation if we're moving
                // and not in the process of rotating to zero
                float rotationAmount = horizontalOffset * offsetToRootSpeed * Time.deltaTime;

                // Limit rotation amount to prevent overshooting
                rotationAmount = Mathf.Sign(rotationAmount) * Mathf.Min(Mathf.Abs(rotationAmount), Mathf.Abs(horizontalOffset));

                // Apply rotation to the root
                transform.Rotate(0, rotationAmount, 0);

                // Reduce horizontal offset by the amount we rotated
                horizontalOffset -= rotationAmount;

                // If horizontal offset is very small, reset to zero
                if (Mathf.Abs(horizontalOffset) < 0.1f)
                {
                    horizontalOffset = 0f;
                }

                // For mouse input while moving, apply directly to root rotation
                if (!inLookAtMode)
                {
                    transform.Rotate(0, lookInput.x, 0);
                }

                // If in look-at mode and moving, recalculate after rotation
                if (inLookAtMode)
                {
                    HandleLookAtTarget();
                }
            }

            // Update previous rotation for next frame
            previousRootYRotation = transform.eulerAngles.y;
        }

        private void HandleMouseLook()
        {
            // Apply the current offsets to PoseBlenderLite
            if (poseBlenderLite != null)
            {
                poseBlenderLite.lookVerticalOffset = verticalOffset;
                poseBlenderLite.lookHorizontalOffset = horizontalOffset;
                poseBlenderLite.leaningOffset = leaningOffset;
            }
        }

        private void HandleMovement()
        {
            // Move based on this transform's forward/right
            Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

            float movementSpeed = sprintInput ? sprintSpeed : moveSpeed;

            controller.SimpleMove(moveDirection * movementSpeed);

            animator.SetFloat("velY", movementSpeed * moveDirection.magnitude);
        }
    }
}