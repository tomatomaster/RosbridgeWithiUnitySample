using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;

public class RosBridgeCommunicator
{

    private string id;
    private WebSocket ws;
    public string host = "127.0.0.0";//Rosbridge Server address
    public string port = "9090";//Rosbridge Port
    private string topic = "/mobile_base/commands/velocity";
    private string type = "geometry_msgs/Twist";

    public RosBridgeCommunicator()
    {
        var random = new System.Random();
        //int random = Random.Range(0, 1000);
        this.id = random.Next(0,1000).ToString();
    }


    public void PublishTwistMessage(float forward, float side, float angle)
    {
        OpenSocket();
        Advertise();
        Publish(id, forward, side, 0, angle);
        CloseSocket();
    }
    private void Publish(string id, float forward, float side, float updown, float angle)
    {
        var adMsg = new AdvertiseMessage();
        adMsg.id = id;
        adMsg.topic = topic;
        adMsg.type = type;
        string adJson = JsonUtility.ToJson(adMsg);
        Debug.Log(adJson);
        ws.Send(adJson);
        var pubMsg = new PublishMessage(id, forward, side, updown, 0, 0, angle);
        pubMsg.id = id;
        pubMsg.topic = topic;
        string pubJson = JsonUtility.ToJson(pubMsg, true);
        Debug.Log(pubJson);
        ws.Send(pubJson);
    }

    private void OpenSocket()
    {
        var uri = string.Format("ws://{0}:{1}/", host, port);
        var protocols = new string[] { "http-only", "chat" };
        ws = new WebSocket(uri);
        ws.Connect();
    }

    private void CloseSocket()
    {
        ws.Close();
    }
    private void Advertise()
    {
        var adMsg = new AdvertiseMessage();
        adMsg.id = id;
        adMsg.topic = "/mobile_base/commands/velocity";
        adMsg.type = "geometry_msgs/Twist";
        string adJson = JsonUtility.ToJson(adMsg);
        Debug.Log(adJson);
        ws.Send(adJson);
    }

    /*
    JsonMessage Format
    */
    [System.Serializable]
    public class AdvertiseMessage
    {
        public string op = "advertise";
        public string id = "";
        public string topic = "";
        public string type = "";
    }

    [System.Serializable]
    public class PublishMessage
    {
        public string op = "publish";
        public string id = "";
        public string topic = "/cmd_vel";
        public TwistMessage msg;

        public PublishMessage(string id, float lx, float ly, float lz, float ax, float ay, float az)
        {
            this.id = id;
            msg = new TwistMessage();
            msg.linear = new Axis();
            msg.linear.x = lx;
            msg.linear.y = ly;
            msg.linear.z = lz;
            msg.angular = new Axis();
            msg.angular.x = ax;
            msg.angular.y = ay;
            msg.angular.z = az;
        }
    }

    //https://www.ntt-tx.co.jp/column/yasui_blog/20160309/
    [System.Serializable]
    public class TwistMessage
    {
        public Axis linear;
        public Axis angular;
    }


    [System.Serializable]
    public class Axis
    {
        public float x;
        public float y;
        public float z;
    }
}