using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingUnityBinding : MonoBehaviour
{
    public Material material;

    void Start()
    {
        int width = 800;
        int height = 600;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        RayTracing.Scene scene = new RayTracing.Scene();
        scene.AddSphere(new RayTracing.Sphere() { Position = new Vector3(0, 0, -10), Radius = 2 });

        RayTracing.Camera camera = new RayTracing.Camera(2, 1, Vector3.zero, new RayTracing.Screen(width, height));
        camera.TakePhoto(scene);
        Color[] texColors = camera.CameraTexture;
        tex.SetPixels(texColors);

        tex.Apply();
        material.mainTexture = tex;
    }
}
