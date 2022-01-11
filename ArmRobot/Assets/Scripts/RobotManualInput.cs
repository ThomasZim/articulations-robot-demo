using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotManualInput : MonoBehaviour
{
    public GameObject robot;
    float[] jointsArray;
    private List<float[]> jointsList;
    public bool generatePathFile;

    private void Start()
    {
        Debug.Log("start");
        jointsList = new List<float[]>();
    }

    void Update()
    {
        // check for robot movement
        RobotController robotController = robot.GetComponent<RobotController>();
        for (int i = 0; i < robotController.joints.Length; i++)
        {
            float inputVal = Input.GetAxis(robotController.joints[i].inputAxis);
            if (Mathf.Abs(inputVal) > 0)
            {
                RotationDirection direction = GetRotationDirection(inputVal);
                robotController.RotateJoint(i, direction);
                return;
            }
        }
        robotController.StopAllJointRotations();
        if (generatePathFile)
        {
            jointsArray = robotController.GetCurrentJointRotations();
            if (jointsArray[0] > 180)
            {
                jointsArray[0] -= 360;
            }

            jointsArray[0] *= -1;

            // Shoulder
            //Debug.Log(robotController.joints[1].robotPart.transform.rotation.eulerAngles.z/180*Math.PI);
            //jointsArray[1] = robotController.joints[1].robotPart.transform.rotation.eulerAngles.z; /// 180 * Math.PI;
            if (jointsArray[1] > 180)
            {
                jointsArray[1] -= 360;
            }

            jointsArray[1] *= -1;
            jointsArray[1] -= 90;

            // Elbow
            //Debug.Log(robotController.joints[2].robotPart.transform.rotation.eulerAngles.z / 180 * Math.PI);
            //jointsArray[2] = robotController.joints[2].robotPart.transform.rotation.eulerAngles.z; /// 180 * Math.PI;
            if (jointsArray[2] > 180)
            {
                jointsArray[2] -= 360;
            }
            
            // Off, model not accurate
            // Wrist01
            //Debug.Log(robotController.joints[3].robotPart.transform.rotation.eulerAngles.y/180*Math.PI);
            //joints[3] = robotController.joints[3].robotPart.transform.rotation.eulerAngles.y-90 / 180 * Math.PI;
                        
            // Wrist02 here, wrist01 for real robot
            //Debug.Log(robotController.joints[4].robotPart.transform.rotation.eulerAngles.z/180*Math.PI);
            //jointsArray[3] = (robotController.joints[4].robotPart.transform.rotation.eulerAngles.z);// / 180 * Math.PI;
            if (jointsArray[3] > 180)
            {
                jointsArray[3] -= 360;
            }

            jointsArray[3] -= 90;
            // Wrist03 here, wrist02 for real robot
            //Debug.Log(robotController.joints[5].robotPart.transform.rotation.eulerAngles.y/180*Math.PI);
            //jointsArray[4] = robotController.joints[5].robotPart.transform.rotation.eulerAngles.y; /// -180 * Math.PI;
            if (jointsArray[4] > 180)
            {
                jointsArray[4] -= 360;
            }

            jointsArray[4] *= -1;
            // Hand rotation, wrist03 for real robot
            //jointsArray[4] = robotController.joints[5].robotPart.transform.rotation.eulerAngles.y; /// -180 * Math.PI;
            if (jointsArray[5] > 180)
            {
                jointsArray[5] -= 360;
            }

            jointsArray[5] *= -1;
            jointsArray[5] = 0;         // Hand behaving weirdly in unity, 0 for safety atm
            
            
            
            jointsList.Add(jointsArray);
            
        }

        //check for robot reset
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed reset!");
            float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
            robotController.ForceJointsToRotations(defaultRotations);
        }
        //save position in Json file
        if (Input.GetKeyDown(KeyCode.F))
        {
            
            Debug.Log("Pressed F!");
            jointsArray = new float[6];
            // Base
            jointsArray = robotController.GetCurrentJointRotations();
            //Debug.Log(robotController.joints[0].robotPart.transform.rotation.eulerAngles.y/180*Math.PI);
            //jointsArray[0] = robotController.joints[0].robotPart.transform.rotation.eulerAngles.y; /// -180 * Math.PI;
            if (jointsArray[0] > 180)
            {
                jointsArray[0] -= 360;
            }

            jointsArray[0] *= -1;

            // Shoulder
            //Debug.Log(robotController.joints[1].robotPart.transform.rotation.eulerAngles.z/180*Math.PI);
            //jointsArray[1] = robotController.joints[1].robotPart.transform.rotation.eulerAngles.z; /// 180 * Math.PI;
            if (jointsArray[1] > 180)
            {
                jointsArray[1] -= 360;
            }

            jointsArray[1] *= -1;
            jointsArray[1] -= 90;

            // Elbow
            //Debug.Log(robotController.joints[2].robotPart.transform.rotation.eulerAngles.z / 180 * Math.PI);
            //jointsArray[2] = robotController.joints[2].robotPart.transform.rotation.eulerAngles.z; /// 180 * Math.PI;
            if (jointsArray[2] > 180)
            {
                jointsArray[2] -= 360;
            }
            
            // Off, model not accurate
            // Wrist01
            //Debug.Log(robotController.joints[3].robotPart.transform.rotation.eulerAngles.y/180*Math.PI);
            //joints[3] = robotController.joints[3].robotPart.transform.rotation.eulerAngles.y-90 / 180 * Math.PI;
                        
            // Wrist02 here, wrist01 for real robot
            //Debug.Log(robotController.joints[4].robotPart.transform.rotation.eulerAngles.z/180*Math.PI);
            //jointsArray[3] = (robotController.joints[4].robotPart.transform.rotation.eulerAngles.z);// / 180 * Math.PI;
            if (jointsArray[3] > 180)
            {
                jointsArray[3] -= 360;
            }

            jointsArray[3] -= 90;
            // Wrist03 here, wrist02 for real robot
            //Debug.Log(robotController.joints[5].robotPart.transform.rotation.eulerAngles.y/180*Math.PI);
            //jointsArray[4] = robotController.joints[5].robotPart.transform.rotation.eulerAngles.y; /// -180 * Math.PI;
            if (jointsArray[4] > 180)
            {
                jointsArray[4] -= 360;
            }

            jointsArray[4] *= -1;
            // Hand rotation, wrist03 for real robot
            //jointsArray[4] = robotController.joints[5].robotPart.transform.rotation.eulerAngles.y; /// -180 * Math.PI;
            if (jointsArray[5] > 180)
            {
                jointsArray[5] -= 360;
            }

            jointsArray[5] *= -1;
            jointsArray[5] = 0;         // Hand behaving weirdly in unity, 0 for safety atm
            
            
            
            jointsList.Add(jointsArray);
            
            WriteJson();
            jointsList.Clear();
        }
    }


    // HELPERS

    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
        {
            return RotationDirection.Positive;
        }
        else if (inputVal < 0)
        {
            return RotationDirection.Negative;
        }
        else
        {
            return RotationDirection.None;
        }
    }
    
    // USEFUL
    [Serializable]
    public class Joints
    {
        public float[] jointsTab;
    }

    public class JointsList
    {
        public Joints[] positions;
    }

    public class thisIsATest
    {
        public int test1;
        public int test2;
        public string test3;
    }

    private void WriteJson()
    {
        thisIsATest test = new thisIsATest();
        {
            test.test1 = 2;
            test.test2 = 4;
            test.test3 = "Hello world!";
        }
        JointsList list = new JointsList();
        {
            list.positions = new Joints[jointsList.Count];
            for (int i = 0; i < list.positions.Length; i++)
            {
                list.positions[i] = new Joints();
                list.positions[i].jointsTab = new[] {jointsList[i][0], jointsList[i][1], jointsList[i][2], jointsList[i][3], jointsList[i][4], jointsList[i][5]};
            }
        }
        Debug.Log(jointsList.Count);
        string jsonString = JsonUtility.ToJson(list);
        
        Console.WriteLine(jsonString);
        Debug.Log(jsonString);
        File.WriteAllText(@"R:\Transfer\APEBAV\manualPosition.json",jsonString);
    }
    
    
}
