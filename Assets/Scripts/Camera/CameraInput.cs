using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Flower
{
    /// <summary>
    /// Abstract base input scheme for schemes that control the CameraRig
    /// </summary>
    public class CameraInput : MonoBehaviour, IPause
    {
        private bool pause;

        /// <summary>
        /// Camera rig to control
        /// </summary>
        public CameraControl cameraControl;

        /// <summary>
        /// Pan speed factor when fully zoomed-in
        /// </summary>
        public float nearZoomPanSpeedModifier = 0.2f;

        /// <summary>
        /// Pan threshold (how near to the edge before we pan. Also the denominator for RMB pan)
        /// </summary>
        public float screenPanThreshold = 40f;

        /// <summary>
        /// Pan speed for edge panning
        /// </summary>
        public float mouseEdgePanSpeed = 30f;

        /// <summary>
        /// Drag pan speed multiplier
        /// </summary>
        public float dragPanSpeed = 5.0f;

        /// <summary>
        /// Whether we're currently dragging
        /// </summary>
        private bool isDragging = false;

        /// <summary>
        /// Last mouse position for drag calculation
        /// </summary>
        private Vector2 lastMousePosition;

        /// <summary>
        /// Gets our pan speed multiplier for the given zoom level
        /// </summary>
        /// <returns></returns>
        protected float GetPanSpeedForZoomLevel()
        {
            return cameraControl != null ?
                Mathf.Lerp(nearZoomPanSpeedModifier, 1, cameraControl.CalculateZoomRatio()) :
                1.0f;
        }

        /// <summary>
        /// Perform drag panning - drag the screen to move camera
        /// </summary>
        private void DoScreenEdgePan()
        {
            Vector2 mousePos = UnityInput.mousePosition;

            // Check if mouse is inside screen bounds
            bool mouseInside = (mousePos.x >= 0) &&
                               (mousePos.x < Screen.width) &&
                               (mousePos.y >= 0) &&
                               (mousePos.y < Screen.height);

            // Handle drag input
            if (UnityInput.GetMouseButtonDown(0)) // Left mouse button down
            {
                if (mouseInside)
                {
                    isDragging = true;
                    lastMousePosition = mousePos;
                    cameraControl.StopTracking(); // Stop tracking when starting to drag
                }
            }
            else if (UnityInput.GetMouseButtonUp(0)) // Left mouse button up
            {
                isDragging = false;
            }

            // Handle drag movement
            if (isDragging && mouseInside)
            {
                Vector2 mouseDelta = mousePos - lastMousePosition;
                
                if (mouseDelta.magnitude > 0.1f) // Only move if there's significant movement
                {
                    // Calculate zoom ratio for pan speed
                    float zoomRatio = GetPanSpeedForZoomLevel();
                    
                    // Convert screen delta to world space movement
                    // We need to project the screen movement to the ground plane
                    Vector3 worldDelta = ConvertScreenDeltaToWorldDelta(mouseDelta, zoomRatio);
                    
                    // Apply the pan movement
                    cameraControl.PanCamera(worldDelta * dragPanSpeed);
                }
                
                lastMousePosition = mousePos;
            }
        }

        /// <summary>
        /// Convert screen delta to world space delta for camera panning
        /// </summary>
        /// <param name="screenDelta">Screen space mouse movement</param>
        /// <param name="zoomRatio">Current zoom ratio for speed adjustment</param>
        /// <returns>World space movement vector</returns>
        private Vector3 ConvertScreenDeltaToWorldDelta(Vector2 screenDelta, float zoomRatio)
        {
            if (cameraControl == null || cameraControl.cachedCamera == null)
                return Vector3.zero;

            // Get current camera
            Camera cam = cameraControl.cachedCamera;
            
            // Calculate the movement based on screen percentage
            float screenWidthPercent = screenDelta.x / Screen.width;
            float screenHeightPercent = screenDelta.y / Screen.height;
            
            // Get the current view frustum at the ground plane
            Vector3 currentLookPos = cameraControl.currentLookPosition;
            
            // Calculate world space movement based on current zoom level and camera angle
            float worldMovementScale = cameraControl.zoomDist * 0.5f; // Adjust this multiplier as needed
            
            // Calculate movement in world space
            Vector3 rightMovement = cam.transform.right * screenWidthPercent * worldMovementScale * -1f; // Invert for natural feel
            Vector3 forwardMovement = cam.transform.forward * screenHeightPercent * worldMovementScale * -1f; // Invert for natural feel
            
            // Project to ground plane (remove Y component)
            rightMovement.y = 0;
            forwardMovement.y = 0;
            
            return (rightMovement + forwardMovement) * zoomRatio;
        }

        private void Update()
        {
            if (cameraControl != null && !pause)
            {
                DoScreenEdgePan();
            }
        }

        public void Pause()
        {
            pause = true;
            isDragging = false; // Stop dragging when paused
        }

        public void Resume()
        {
            pause = false;
        }
        
    }
}