using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class skyscript : MonoBehaviour
{
    //回転スピード
    [Range(1, 90)]
    public float rotateSpeed;
    public Material sky;

    float rotationRepeatValue;
    void Start()
    {
    }
    private void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0) * Time.deltaTime, Space.World);
    }
    //public async Task Wait(float seconds)
    //{
    //    await Task.Delay(System.TimeSpan.FromSeconds(seconds));

    //    //var ang = this.transform.rotation.eulerAngles;
    //    //ang.y += _rollValue;
    //    //if (ang.y > 360)ang.y -= 360;
    //    //this.transform.RotateAround(this.transform.position, Vector3.up, ang.y);
    //}

}
