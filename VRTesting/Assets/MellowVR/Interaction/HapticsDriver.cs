using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.InputSystem;

namespace MellowVR
{
    public class HapticsDriver : MonoBehaviour
    {
        public InputDevice[] XRControllers { get; private set; }
        public int channel = 0;
        public float defaultAmplitude = 0.5f;
        public float defaultDuration = 0.1f;

        private void Awake()
        {
            PopulateXRControllers();
        }

        private void PopulateXRControllers()
        {
            XRControllers = new InputDevice[2];

            InputSystem.onDeviceChange += (device, change) => 
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        if (device.usages.Contains(CommonUsages.LeftHand))
                        {
                            Debug.Log("Left InputDevice: " + device);
                            XRControllers[(int)Side.Left] = device;
                        }
                        if (device.usages.Contains(CommonUsages.RightHand))
                        {
                            Debug.Log("Right InputDevice: " + device);
                            XRControllers[(int)Side.Right] = device;
                        }
                        break;
                    case InputDeviceChange.Removed:                        
                        if (device.usages.Contains(CommonUsages.LeftHand))
                        {
                            XRControllers[(int)Side.Left] = null;
                        }
                        if (device.usages.Contains(CommonUsages.RightHand))
                        {
                            XRControllers[(int)Side.Right] = null;
                        }
                        break;
                    case InputDeviceChange.Reconnected:
                        if (device.usages.Contains(CommonUsages.LeftHand))
                        {
                            XRControllers[(int)Side.Left] = device;
                        }
                        if (device.usages.Contains(CommonUsages.RightHand))
                        {
                            XRControllers[(int)Side.Right] = device;
                        }
                        break;
                    case InputDeviceChange.Disconnected:                        
                        if (device.usages.Contains(CommonUsages.LeftHand))
                        {
                            XRControllers[(int)Side.Left] = null;
                        }
                        if (device.usages.Contains(CommonUsages.RightHand))
                        {
                            XRControllers[(int)Side.Right] = null;
                        }
                        break;
                }
            };
        }

        public void SendHaptics(int side)
        {
            SendHaptics(side, defaultAmplitude, defaultDuration);
        }

        public void SendHaptics(int side, float amplitude, float duration)
        {
            if (XRControllers[side] != null)
            {
                var hapticsRequest = SendHapticImpulseCommand.Create(channel, amplitude, duration);
                XRControllers[side].ExecuteCommand(ref hapticsRequest);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}