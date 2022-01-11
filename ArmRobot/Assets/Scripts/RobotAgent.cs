using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using Unity.MLAgents.Actuators;
using System;
using System.Collections.Generic;
using System.IO;

public class RobotAgent : Agent
{
    public GameObject endEffector;
    public GameObject cube;
    public GameObject robot;

    RobotController robotController;
    TouchDetector touchDetector;
    TablePositionRandomizer tablePositionRandomizer;
    
    float[] jointsArray;
    private List<float[]> jointsList;



    void Start()
    {
        robotController = robot.GetComponent<RobotController>();
        touchDetector = cube.GetComponent<TouchDetector>();
        tablePositionRandomizer = cube.GetComponent<TablePositionRandomizer>();
        jointsList = new List<float[]>();
    }


    // AGENT

    public override void OnEpisodeBegin()
    {
       
        float[] defaultRotations = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        robotController.ForceJointsToRotations(defaultRotations);
        touchDetector.hasTouchedTarget = false;
        tablePositionRandomizer.Move();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (robotController.joints[0].robotPart == null)
        {
            // No robot is present, no observation should be added
            return;
        }

        // relative cube position
        Vector3 cubePosition = cube.transform.position - robot.transform.position;
        sensor.AddObservation(cubePosition);

        // relative end position
        Vector3 endPosition = endEffector.transform.position - robot.transform.position;
        sensor.AddObservation(endPosition);
        sensor.AddObservation(cubePosition - endPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // move
        for (int jointIndex = 0; jointIndex < actions.DiscreteActions.Length; jointIndex ++)
        {
            RotationDirection rotationDirection = ActionIndexToRotationDirection((int) actions.DiscreteActions[jointIndex]);
            robotController.RotateJoint(jointIndex, rotationDirection, false);

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


        }


        // Knocked the cube off the table
        if (cube.transform.position.y < -1.0)
        {
            SetReward(-1f);
            EndEpisode();
        }

        // end episode if we touched the cube
        if (touchDetector.hasTouchedTarget)
        {
            SetReward(1f);
            WriteJson();
            jointsList.Clear();
            EndEpisode();
        }

        
        //reward
        float distanceToCube = Vector3.Distance(endEffector.transform.position, cube.transform.position); // roughly 0.7f


        var jointHeight = 0f; // This is to reward the agent for keeping high up // max is roughly 3.0f
        for (int jointIndex = 0; jointIndex < robotController.joints.Length; jointIndex ++)
        {
            jointHeight += robotController.joints[jointIndex].robotPart.transform.position.y - cube.transform.position.y;
        }
        var reward = - distanceToCube + jointHeight / 100f;

        SetReward(reward * 0.1f);

    }


    // HELPERS

    static public RotationDirection ActionIndexToRotationDirection(int actionIndex)
    {
        return (RotationDirection)(actionIndex - 1);
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
        File.WriteAllText(@"R:\Transfer\APEBAV\testUnity.json",jsonString);
    }



}


