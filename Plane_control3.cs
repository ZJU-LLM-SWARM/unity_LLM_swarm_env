using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class Plane_Control3 : MonoBehaviour
{
    public Transform drone;
    public float speed = 30f;
    public Rigidbody rb;
    public float angle_speed = 1f;
    public float ad_Angel = 0f;
    public float ws_Angel = 0f;
    public bool left;
    public bool right;
    public bool front;
    public bool back;
    public bool Up;
    public bool Down;

    public bool left_turn;
    public bool right_turn;
    public float local_Position_x;
    public float local_Position_y;
    public float local_Position_z;
    public float local_Rotation_x;
    public float local_Rotation_y;
    public float local_Rotation_z;
    float curpos_x = 0f;
    float curpos_y = 0f;
    float curpos_z = 0f;
    float lastpos_x = 0f;
    float lastpos_y = 0f;
    float lastpos_z = 0f;
    public float drone_speed_x;
    public float drone_speed_y;
    public float drone_speed_z;
    int count=0;

    public float target_x ;
    public float target_y ;
    public float target_z ;
    public float distance_drone3;
    public bool drone3_found;
    float left_right_move;
    float front_back_move;
    public GameObject uavs;
    // void get_d2()
    // {
    //     left = control_center.drone2_left;
    //     right = control_center.drone2_right;
    //     front = control_center.drone2_front;
    //     back = control_center.drone2_back;
    //     Up = control_center.drone2_up;
    //     Down = control_center.drone2_down;
    //     left_turn = control_center.drone2_left_turn;
    //     right_turn = control_center.drone2_right_turn;
    // }
    void Start()
    {
        drone = gameObject.GetComponent<Transform>();//获取组件
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {   
        get_self_information();
        // get_d2();
        information_update();

        control();
    }
    void information_update()
    {
        left = uavs.GetComponent<control_center>().drone3_left;
        right = uavs.GetComponent<control_center>().drone3_right;
        front = uavs.GetComponent<control_center>().drone3_front;
        back = uavs.GetComponent<control_center>().drone3_back;
        Up = uavs.GetComponent<control_center>().drone3_up;
        Down = uavs.GetComponent<control_center>().drone3_down;
        left_turn = uavs.GetComponent<control_center>().drone3_left_turn;
        right_turn = uavs.GetComponent<control_center>().drone3_right_turn;
        target_x = uavs.GetComponent<control_center>().target_x;
        target_y = uavs.GetComponent<control_center>().target_y;
        target_z = uavs.GetComponent<control_center>().target_z;
        local_Position_x = transform.localPosition.x;
        local_Position_y = transform.localPosition.y;
        local_Position_z = transform.localPosition.z;
        distance_drone3 = math_distance(local_Position_x, local_Position_y, local_Position_z, target_x, target_y, target_z);
        drone3_found = found(distance_drone3);
        uavs.GetComponent<control_center>().drone3_found = drone3_found;
        uavs.GetComponent<control_center>().distance_drone3_target = distance_drone3;
        left_right_move = uavs.GetComponent<control_center>().drone3_speed_xcontrol;
        front_back_move = uavs.GetComponent<control_center>().drone3_speed_zcontrol;
    }
    float math_distance(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        float distance;
        distance = Mathf.Sqrt((x1 - x2) * (x1 - x2) + (z1-z2)*(z1-z2));
        return distance;
    }
    bool found(float distance)
    {
        if(distance<100f)
            return true;
        else
            return false;
    }
    void get_self_information()
    {
        local_Position_x = drone.localPosition.x;
        uavs.GetComponent<control_center>().drone3_localPosition_x = local_Position_x;
        local_Position_y = drone.localPosition.y;
        uavs.GetComponent<control_center>().drone3_localPosition_y = local_Position_y;
        local_Position_z = drone.localPosition.z;
        uavs.GetComponent<control_center>().drone3_localPosition_z = local_Position_z;
        local_Rotation_x = drone.localRotation.eulerAngles.x;
        uavs.GetComponent<control_center>().drone3_localRotation_x = local_Rotation_x;
        local_Rotation_y = drone.localRotation.eulerAngles.y;
        uavs.GetComponent<control_center>().drone3_localRotation_y = local_Rotation_y;
        local_Rotation_z = drone.localRotation.eulerAngles.z;
        uavs.GetComponent<control_center>().drone3_localRotation_z = local_Rotation_z;
        count++;
        if (count == 6)
        {
            curpos_x = transform.position.x;
            curpos_y = transform.position.y;
            curpos_z = transform.position.z;
            drone_speed_x = (curpos_x - lastpos_x)/Time.deltaTime/6;
            drone_speed_y = (curpos_y - lastpos_y)/Time.deltaTime/6;
            drone_speed_z = (curpos_z - lastpos_z)/Time.deltaTime/6;
            lastpos_x = curpos_x;
            lastpos_y = curpos_y;
            lastpos_z = curpos_z;
            uavs.GetComponent<control_center>().drone3_speed_x = drone_speed_x;
            uavs.GetComponent<control_center>().drone3_speed_y = drone_speed_y;
            uavs.GetComponent<control_center>().drone3_speed_z = drone_speed_z;
            count = 0;
        }
        
    }
    void control()
    {
        // rb.AddForce(0, 5f*9.8f, 0);
        if (left)
        {
            transform.Translate(-speed * Time.deltaTime * Mathf.Cos(ad_Angel /10 * Mathf.Deg2Rad), speed*Time.deltaTime * Mathf.Sin(ad_Angel / 10 * Mathf.Deg2Rad), 0);//Mathf.Cos(Angel * Mathf.Deg2Rad)是Angel°的余弦值，Mathf.Sin(Angel * Mathf.Deg2Rad)是Angel°的正弦值，下同
            if (ad_Angel < 100f)
            {
                this.transform.Rotate(0, 0, 0.1f);
                ad_Angel++;
            }
        }
        if (right)
        {
            transform.Translate(speed*Time.deltaTime * Mathf.Cos(ad_Angel /10 * Mathf.Deg2Rad), speed*Time.deltaTime * Mathf.Sin(- ad_Angel /10 * Mathf.Deg2Rad), 0);
            if (ad_Angel > -100f)
            {
                this.transform.Rotate(0, 0, -0.1f);
                ad_Angel--;
            }
        }
        if (!left && !right)
        {
            if(ad_Angel>0)
            {
                this.transform.Rotate(0, 0, -0.1f);
                ad_Angel--;
            }
            if(ad_Angel<0)
            {
                this.transform.Rotate(0, 0, 0.1f);
                ad_Angel++;
            }
        }

        if (front)
        {
            transform.Translate(0, speed*Time.deltaTime * Mathf.Sin(ws_Angel /10 * Mathf.Deg2Rad), speed*Time.deltaTime * Mathf.Cos(ws_Angel/10 * Mathf.Deg2Rad));
            if (ws_Angel < 100f)
            {
                this.transform.Rotate(0.1f, 0, 0);
                ws_Angel++;
            }
        }
        if (back)
        {
            transform.Translate(0, speed*Time.deltaTime * Mathf.Sin(-ws_Angel /10 * Mathf.Deg2Rad), -speed*Time.deltaTime * Mathf.Cos(ws_Angel /10 * Mathf.Deg2Rad));
        if (ws_Angel > -100f)
            {
                this.transform.Rotate(-0.1f, 0, 0);
                ws_Angel--;
            }
        }
        if (!front && !back)
        {
            if(ws_Angel>0)
            {
                this.transform.Rotate(-0.1f, 0, 0);
                ws_Angel--;
            }
            if(ws_Angel<0)
            {
                this.transform.Rotate(0.1f, 0, 0);
                ws_Angel++;
            }
        }
        if (front_back_move!=0)
        {
            transform.Translate(0, 0, front_back_move*Time.deltaTime);
        }
        if (left_right_move!=0)
        {
            transform.Translate(left_right_move*Time.deltaTime, 0, 0);
        }
        
        if (left_turn)
        {
            this.transform.Rotate(0, -angle_speed, 0, Space.World);
        }
        if (right_turn)
        {
            this.transform.Rotate(0, angle_speed, 0, Space.World);
        }
        if (Up)
            transform.Translate(0, speed*Time.deltaTime, 0);
        if (Down)
        {
            if(transform.position.y>0.5f)
                transform.Translate(0, -speed*Time.deltaTime, 0);
        }
    }
}
