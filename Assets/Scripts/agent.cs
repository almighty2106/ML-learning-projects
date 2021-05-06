using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.UI;
using TMPro;

public class agent : Agent
{
    public Rigidbody ball;
    Rigidbody player;
    public float speed = 30f;
    public TextMeshPro display;

    float currentDiff=0f; //recording y position changes
    float previousDiff = 0f;//previous diff
    float previousY = 5.0f;//the previous Y position of Player
    bool ifCollied = false;//to see if ball and player collied 

    private void Start()
    {
        player = GetComponent<Rigidbody>();
    } 

    public override void OnEpisodeBegin()
    {
        //player = this.GetComponent<Rigidbody>();
        ball.transform.localPosition = new Vector3(Random.value * 10 - 5, 5.0f, Random.value * 10 - 5);
        ball.velocity = Vector3.zero;
        ball.rotation = Quaternion.Euler(Vector3.zero);
        ball.angularVelocity = Vector3.zero;

        player.transform.localPosition = Vector3.up;
        player.velocity = Vector3.zero;
        player.rotation = Quaternion.Euler(Vector3.zero);
        player.angularVelocity = Vector3.zero;

        currentDiff = 0.0f;
        previousDiff = 0.0f;
        previousY = 5.0f;
        ifCollied = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation(ball.transform.localPosition);//position 3D
        sensor.AddObservation(ball.velocity);//velocity 3D
        sensor.AddObservation(ball.rotation); //4d
        sensor.AddObservation(ball.angularVelocity);

        sensor.AddObservation(player.transform.localPosition);//same settings on Player
        sensor.AddObservation(player.velocity);
        sensor.AddObservation(player.rotation);
        sensor.AddObservation(player.angularVelocity);
           //Toal of 26 space sizes
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        if (player.transform.localPosition.y == 1.0f)
        {
            controlSignal.y = actionBuffers.ContinuousActions[2] * 10f;
        }

        player.AddForce(controlSignal * speed);

        currentDiff = ball.transform.localPosition.y - previousY;
        if (currentDiff > 0f && previousDiff < 0f && ifCollied) //The current value greater than 0 means that the ball has been touched (go up)
                                                                //and the previous value less than 0 means the ball is waiting to be touched.(go down)
        {
            AddReward(1f);
        }
        ifCollied = false;
        previousDiff = currentDiff; 
        previousY = ball.transform.localPosition.y;

        if (ball.transform.localPosition.y < 1.5f   
            // Means the ball has a low y position that player can't touch it with 'head'
            ||Mathf.Abs(player.transform.localPosition.x)>10f
            ||Mathf.Abs(player.transform.localPosition.y)>10f) //Check if play fall from plane 
        {
            EndEpisode(); //End this episode
        }

        display.text = GetCumulativeReward().ToString("Score: "+"0"); //N0 -- 0 decimal numbers

    }

    // public override void AgenRest

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == ball) //when ball and player collied, return true
        {
            ifCollied = true; 
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetAxis("Jump");
    }


}
