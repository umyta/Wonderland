using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MoveServerNS
{
    public class WinMoveServer : MonoBehaviour
    {
        Thread receiveThread;
        UdpClient recvClient;
        UdpClient sendClient;

        public String ipAddress = "127.0.0.1";

        const int MAX_CONTROLLERS = 6;
        private WinMoveController[] moveControllers = new WinMoveController[MAX_CONTROLLERS];

        // ------- Unity Builtin Functions -------
        public void Start()
        {
            for (int i = 0; i < MAX_CONTROLLERS; i++)
            {
                moveControllers[i] = new WinMoveController(i);
            }
            sendClient = new UdpClient(ipAddress, 23460);
            startReceiveThread();
            sendMessageToServer("connect");
        }

        public void LateUpdate()
        {
            for (int i = 0; i < MAX_CONTROLLERS; i++)
            {
                if (moveControllers[i].active)
                {
                    moveControllers[i].clearBtnFlags();
                }
            }
        }

        /* 
        * Desc: Returns a reference of the move controller (vals 0-5) if it is available.
        * If the move isn't 'active' (that is hasn't received any packets) the function
        * returns null.
        */
        public WinMoveController getController(int controllerNumber)
        {
            if (controllerNumber >= 0 && controllerNumber < MAX_CONTROLLERS)
            {
                if (moveControllers[controllerNumber].active) return moveControllers[controllerNumber];
            }
            return null;
        }

        // ------- 'Send' public functions. Talks to server -------

        /* 
        * Desc: Sets the rumble (0-255) on a controller. 
        * You must continually update this to keep rumble on (or server will timeout).
        * IMPORTANT: Always remember to send a rumble of 0 when you want the rumble to stop.
        * If you don't, all other sent packets will send your last rumble value.
        * Hacky as, gotta fix.
        */
        public void Send_rumbleController(WinMoveController move, int amount)
        {
            if (amount < 0 || amount > 255) print("WARNING: Rumble amount is out of bounds: " + amount);
            String msg = "d " + move.controllerNumber + " 1 " + amount + " 0 0 0 0 0 0" + '\0';
            sendMessageToServer(msg);
        }
        public void Send_rumbleController(int controllerID, int amount)
        {
            WinMoveController move = getController(controllerID);
            if (move != null)
            {
                Send_rumbleController(move, amount);
            } else
            {
                print("WARNING: Invalid move: " + controllerID);
            }
        }

        /* 
        * Desc: Resets the orientation of the controller. 
        * The server will interpret the orientation of the controller as the 'home' position.
        * That is, facing the screen, no roll and no pitch.
        * DON'T use this continously.
        */
        public void Send_calibrateOrientation(WinMoveController move)
        {
            String msg = "d " + move.controllerNumber + " 0 0 1 0 0 0 0 0" + '\0';
            sendMessageToServer(msg);
        }
        public void Send_calibrateOrientation(int controllerID)
        {
            WinMoveController move = getController(controllerID);
            if (move != null)
            {
                Send_calibrateOrientation(move);
            }
            else
            {
                print("WARNING: Invalid move: " + controllerID);
            }
        }

        /* 
        * Desc: Sets color of the controller using r, g, b values (all between 0-255).
        * NOTE: Changing the color of a tracked controller will most likely cause the tracker to lose it.
        *       Call 'Send_resetMoveLight' to get the controller back to the tracked color.
        */
        public void Send_setMoveLight(WinMoveController move, int r, int g, int b)
        {

            String msg = "d " + move.controllerNumber + " 0 0 0 0 1 " + r + " " + g + " " + b + '\0';
            sendMessageToServer(msg);
        }
        public void Send_setMoveLight(int controllerID, int r, int g, int b)
        {
            WinMoveController move = getController(controllerID);
            if (move != null)
            {
                Send_setMoveLight(move, r, g, b);
            }
            else
            {
                print("WARNING: Invalid move: " + controllerID);
            }
        }
        /* 
        * Desc: Resets the move controller's light to the one it was assigned by the tracker.
        * This of course requires the tracker to be working.
        */
        public void Send_resetMoveLight(WinMoveController move)
        {
            String msg = "d " + move.controllerNumber + " 0 0 0 1 0 0 0 0" + '\0';
            sendMessageToServer(msg);
        }
        public void Send_resetMoveLight(int controllerID)
        {
            WinMoveController move = getController(controllerID);
            if (move != null)
            {
                Send_resetMoveLight(move.controllerNumber);
            }
            else
            {
                print("WARNING: Invalid move: " + controllerID);
            }
        }

        // ------- Receive Thread -------
        private void startReceiveThread()
        {
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));

            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveData()
        {
            int apiPort = 23459;
            IPAddress loopbackIP = IPAddress.Parse(ipAddress);
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(loopbackIP, apiPort);
            try
            {
                recvClient = new UdpClient(apiPort, AddressFamily.InterNetwork);
                while (Thread.CurrentThread.IsAlive)
                {
                    Byte[] receiveBytes = recvClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    parsePacket(returnData);
                }
            }
            catch (Exception e)
            {
                print(e);
            }
        }

        // msgType controller rumble resetOrientation trackerLight changeLight r g b
        private void sendMessageToServer(String message)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            try
            {
                sendClient.Send(sendBytes, message.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void parsePacket(string packet)
        {
            String[] splitData = packet.Split(' ');
            String packetType = splitData[0];
            switch (packetType)
            {
                case "a":
                    parsePhysicalPacket(splitData);
                    break;

                case "b":
                    parsePositionPacket(splitData);
                    break;

                default:
                    print("Warning: Unknown packet encountered: '" + packet + "'");
                    break;
            }
        }

        // a, msgNo, c, currButtons, analogVal, ax, ay, az, gx, gy, gz, mx, my, mz, orientationEnabled, qw, qx, qy, qz, r, g, b
        private void parsePhysicalPacket(String[] splitData)
        {
            if (splitData.Length == 22)
            {
                //Check the controller number is between 0 and MAX_CONTROLLERS
                int c = int.Parse(splitData[2]);
                if (c >= 0 && c < MAX_CONTROLLERS)
                {
                    moveControllers[c].updatePhysicalData(splitData);
                }
            } else
            {
                print("Warning: Packet type 'a' has wrong length: " + string.Join(", ", splitData));
            }
        }

        // b, posUpdateNumber, c, tx, ty, tz, ux, uy, trackingMove
        private void parsePositionPacket(String[] splitData)
        {
            if (splitData.Length == 9)
            {
                int c = int.Parse(splitData[2]);
                if (c >= 0 && c < MAX_CONTROLLERS)
                {
                    moveControllers[c].updatePositionData(splitData);
                }
            }
            else
            {
                print("Warning: Packet type 'b' has wrong length: " + string.Join(", ", splitData));
            }
        }

        // -------------------------------------------------------------------------
        public void OnApplicationQuit()
        {
            // end of application
            if (receiveThread != null)
            {
                receiveThread.Abort();
            }

            if (recvClient != null)
            {
                recvClient.Close();
            }

            if (sendClient != null)
            {
                sendClient.Close();
            }

            print("Stop");
        }
    }
}