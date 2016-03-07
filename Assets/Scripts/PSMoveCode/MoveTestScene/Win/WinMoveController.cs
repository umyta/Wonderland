
using System;
using UnityEngine;

namespace MoveServerNS
{
    public enum MoveButton
    {
        // Controller Button IDS (use bitwise checks)
        BTN_CROSS = 1 << 7,
        BTN_SQUARE = 1 << 6,
        BTN_TRIANGLE = 1 << 5,
        BTN_CIRCLE = 1 << 4,
        BTN_MOVE = 1 << 3,
        BTN_START = 1 << 2,
        BTN_SELECT = 1 << 1,
        BTN_PS = 1
    }

    public class MoveColor
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }

        public MoveColor()
        {
            r = 0;
            g = 0;
            b = 0;
        }
    }

    public class SmoothedVector3
    {
        private Vector3[] rawVectors;
        private Vector3 smoothedVector;
        private bool smoothOkay;
        private int totalElements;
        private int currElement;

        public SmoothedVector3 (int smoothingAmount)
        {
            if (smoothingAmount < 1) throw new System.ArgumentException("Parameter must be larger than 0.", "smoothingAmount");
            rawVectors = new Vector3[smoothingAmount];
            totalElements = smoothingAmount;
            currElement = 0;
        }

        public void updateRaw(Vector3 rawVector)
        {
            rawVectors[currElement] = rawVector;
            currElement += 1;
            if (currElement >= totalElements)
            {
                smoothOkay = true;
                currElement = 0;
            }
            if (smoothOkay)
            {
                smoothedVector.Set(0, 0, 0);
                for (int i = 0; i<totalElements;i++)
                {
                    smoothedVector.x += rawVectors[i].x;
                    smoothedVector.y += rawVectors[i].y;
                    smoothedVector.z += rawVectors[i].z;
                }
                smoothedVector.x /= totalElements;
                smoothedVector.y /= totalElements;
                smoothedVector.z /= totalElements;
            }
        }

        public Vector3 getSmoothedVector()
        {
            return smoothedVector;
        }
    }

    public class WinMoveController {

        // A controller is considered active after a valid packet is received.
        public bool active { get; private set; }
        public int controllerNumber { get; private set; }

        // Raw data from Move.
        private Vector3 gyroscopeRaw;
        private Vector3 accelerometerRaw;
        private Vector3 magnetometerRaw;
        private Quaternion quaternion;
        private Vector3 positionRaw;
        private Vector3 positionNorm;

        // Smoothed data from Move.
        private SmoothedVector3 gyroscopeSmoothed;
        private SmoothedVector3 accelerometerSmoothed;
        private SmoothedVector3 magnetometerSmoothed;
        private SmoothedVector3 positionSmoothed;
        private SmoothedVector3 positionNormSmooothed;

        // Calculated data from Move.
        private Vector3 velocityRaw;
        private Vector3 velocitySmoothed;
        private Vector3 accelerationRaw;
        private Vector3 accelerationSmoothed;

        public bool orientationEnabled { get; private set; }
        public MoveColor moveColor { get; set; }

        private double currTime_a = -1;
        private double currTime_b = -1;
        public double delta_t_a { get; private set; }
        public double delta_t_b { get; private set; }

        private byte prevButtons;
        private byte currButtons;
        private byte pressedButtons;
        private byte releasedButtons;

        public int triggerValue { get; private set; }

        //Tells us whether the controller was properly tracked in the last update.
        public bool currentlyTracked { get; private set; }

        public WinMoveController(int _controllerNumber)
        {
            controllerNumber = _controllerNumber;
            moveColor = new MoveColor();
            currentlyTracked = false;
            active = false;

            // Initialise raw vectors.
            accelerometerRaw = new Vector3();
            gyroscopeRaw = new Vector3();
            magnetometerRaw = new Vector3();
            quaternion = new Quaternion();
            positionRaw = new Vector3();
            positionNorm = new Vector2();

            // Initialise Smoothed Vectors 
            // TODO: ALLOW USER TO DECIDE SMOOTHING AMOUNTS
            positionSmoothed = new SmoothedVector3(5);
            positionNormSmooothed = new SmoothedVector3(5);
            gyroscopeSmoothed = new SmoothedVector3(5); 
            accelerometerSmoothed = new SmoothedVector3(5); 
            magnetometerSmoothed = new SmoothedVector3(5); 
        }

        /*
        * Desc: Used by the MoveServer to parse 'a' packets to the controller.
        * msg format: a, msgNo, c, currButtons, analogVal, ax, ay, az, gx, gy, gz, mx, my, mz, orientationEnabled, qw, qx, qy, qz, r, g, b
        */
        public Boolean updatePhysicalData(String[] physicalData)
        {
            int c = int.Parse(physicalData[2]);
            if (c == controllerNumber)
            {
                if (!active) active = true;

                // Set our new values from the packet.
                prevButtons = currButtons;
                currButtons = byte.Parse(physicalData[3]);
                triggerValue = int.Parse(physicalData[4]);

                accelerometerRaw.Set(float.Parse(physicalData[5]), float.Parse(physicalData[6]), float.Parse(physicalData[7]));
                gyroscopeRaw.Set(float.Parse(physicalData[8]), float.Parse(physicalData[9]), float.Parse(physicalData[10]));
                magnetometerRaw.Set(float.Parse(physicalData[11]), float.Parse(physicalData[12]), float.Parse(physicalData[13]));

                orientationEnabled = (physicalData[14] == "1" ? true : false);

                if (orientationEnabled == true)
                {
                    quaternion.Set(-float.Parse(physicalData[16]), -float.Parse(physicalData[17]), float.Parse(physicalData[18]), float.Parse(physicalData[15]));
                }
                moveColor.r = int.Parse(physicalData[19]);
                moveColor.g = int.Parse(physicalData[20]);
                moveColor.b = int.Parse(physicalData[21]);

                // Process the new values.
                double newTime = System.DateTime.Now.Ticks * .0000001;
                if (currTime_a != -1) delta_t_a = newTime - currTime_a;
                currTime_a = newTime;

                gyroscopeSmoothed.updateRaw(gyroscopeRaw);
                accelerometerSmoothed.updateRaw(accelerometerRaw);
                magnetometerSmoothed.updateRaw(magnetometerRaw);

                pressedButtons = (byte)(pressedButtons|(~prevButtons & currButtons));
                releasedButtons = (byte)(releasedButtons|(prevButtons & ~currButtons));

                return true;
            }
            return false;
        }

        /*
        * Desc: Used by the MoveServer to parse 'b' packets to the controller.
        * msg format: b msgNo controller tx ty tz ux uy trackingMove
        */
        public Boolean updatePositionData(String[] positionData)
        {
            int c = int.Parse(positionData[2]);
            if (c == controllerNumber)
            {
                if (!active) active = true;
                // Used to set the controllers properties

                // Set our new values from the packet.
                Vector3 prevPosRaw = positionRaw;

                positionRaw.Set(float.Parse(positionData[3])/100f, float.Parse(positionData[4]) / 100f, float.Parse(positionData[5]) / 100f);
                //Debug.Log(positionRaw.ToString());
                positionNorm.Set(float.Parse(positionData[6]), float.Parse(positionData[7]), 0);

                currentlyTracked = positionData[8] == "0" ? false : true;

                // Process the new values.
                double newTime = System.DateTime.Now.Ticks * .0000001;
                if (currTime_b != -1) delta_t_b = newTime - currTime_b;
                currTime_b = newTime;

                // Store the last smoothed position for velocity calc.
                Vector3 prevPosSmooth = positionSmoothed.getSmoothedVector();

                positionSmoothed.updateRaw(positionRaw);
                positionNormSmooothed.updateRaw(positionNorm);

                // Calculate velocity.
                velocityRaw.Set((float)((positionRaw.x - prevPosRaw.x) / delta_t_b), (float)((positionRaw.x - prevPosRaw.x) / delta_t_b), (float)((positionRaw.x - prevPosRaw.x) / delta_t_b));
                if (prevPosSmooth != null)
                {
                    Vector3 posSmooth = positionSmoothed.getSmoothedVector();
                    velocitySmoothed.Set((float)((posSmooth.x - prevPosSmooth.x) / delta_t_b), (float)((posSmooth.x - prevPosSmooth.x) / delta_t_b), (float)((posSmooth.x - prevPosSmooth.x) / delta_t_b));
                }
                return true;
            }
            return false;
        }

        // USED BY API ONLY.
        // Used to clear the pressed and released button flags. 
        // This allows us to sync the Unity loop with the receive thread.
        public void clearBtnFlags()
        {
            pressedButtons = 0;
            releasedButtons = 0;
        }

        // ------ User GET functions ------
        public Vector3 getGyroscopeRaw() { return gyroscopeRaw; }
        public Vector3 getGyroscopeSmooth() { return gyroscopeSmoothed.getSmoothedVector(); }
        public Vector3 getAccelerometerRaw() { return accelerometerRaw; }
        public Vector3 getAccelerometerSmooth() { return accelerometerSmoothed.getSmoothedVector(); }
        public Vector3 getMagnetometerRaw() { return magnetometerRaw; }
        public Vector3 getMagnetometerSmooth() { return magnetometerSmoothed.getSmoothedVector(); ; }
        public Quaternion getQuaternion() { return quaternion; }
        public Vector3 getPositionRaw() { return positionRaw; }
        public Vector3 getPositionSmooth() { return positionSmoothed.getSmoothedVector(); }
        public Vector3 getPositionNorm() { return positionNorm; }
        public Vector3 getPositionNormSmooth() { return positionNormSmooothed.getSmoothedVector();  }
        public Vector3 getVelocityRaw(){ return velocityRaw; }
        public Vector3 getVelocitySmooth() { return velocitySmoothed; }

        public Boolean btnOnPress(MoveButton BTN_CODE)
        {
            if (((byte)BTN_CODE & pressedButtons) != 0) return true;
            return false;
        }

        public Boolean btnOnRelease(MoveButton BTN_CODE)
        {
            if (((byte)BTN_CODE & releasedButtons) != 0) return true;
            return false;
        }

        public Boolean btnPressed(MoveButton BTN_CODE)
        {
            if (((byte)BTN_CODE & currButtons) != 0) return true;
            return false;
        }

        public Boolean btnReleased(MoveButton BTN_CODE)
        {
            if (((byte)BTN_CODE & currButtons) == 0) return true;
            return false;
        }

        public String toString()
        {
            return "Move: " + controllerNumber.ToString();
        }
    }
}
