using System.Collections.Generic;
using UnityEngine;

namespace RayTracing
{
    class Screen
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Screen(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    class Camera
    {
        private float m_viewportWidth;
        private float m_viewportHeight;
        private float m_focalLength;

        private Vector3 m_position;

        public float ViewportWidth { get { return m_viewportWidth; } }
        public float ViewportHeight { get { return m_viewportHeight; } }
        public float FocalLength { get { return m_focalLength; } }

        public Vector3 Position { get { return m_position; } }

        private Color[] m_cameraTexture;
        public Color[] CameraTexture { get { return m_cameraTexture; } }
        private int m_texWidth;
        private int m_texHeight;

        public Camera(float viewportWidth, float viewportZ, Vector3 position, Screen screenInfo)
        {
            m_viewportWidth = viewportWidth;
            m_viewportHeight = m_viewportWidth / ((float)screenInfo.Width / screenInfo.Height);
            m_focalLength = viewportZ;

            m_position = position;

            m_texWidth = screenInfo.Width;
            m_texHeight = screenInfo.Height;
            m_cameraTexture = new Color[m_texWidth * m_texHeight];
            ClearCameraTexture();
        }

        public void TakePhoto(Scene scene)
        {
            Ray curRay = new Ray(m_position, Vector3.up);
            Vector3 curDirection = Vector3.up;

            Vector3 horizental = new Vector3(ViewportWidth, 0, 0);
            Vector3 vertical = new Vector3(0, ViewportHeight, 0);
            Vector3 left_lower = m_position - new Vector3(0, 0, FocalLength) - (horizental / 2) - (vertical / 2);

            for(int h = 0; h < m_texHeight; ++h)
                for(int w = 0; w < m_texWidth; ++w)
                {
                    float u = (float)w / (m_texWidth - 1);
                    float v = (float)h / (m_texHeight - 1);

                    curDirection = left_lower + horizental * u + vertical * v;
                    curRay.direction = curDirection;

                    Color col = scene.RayTracing(curRay);
                    WriteColor(col, w, h);
                }
        }

        private void WriteColor(Color col, int width, int height)
        {
            m_cameraTexture[height * m_texWidth + width] = col;
        }

        private void ClearCameraTexture()
        {
            ClearCameraTexture(Color.cyan);
        }

        private void ClearCameraTexture(Color color)
        {
            for (int i = 0; i < m_cameraTexture.Length; ++i)
                m_cameraTexture[i] = color;
        }
    }

    //Geometry
    class Sphere
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
    }

    class Scene
    {
        private List<Sphere> m_spheres = new List<Sphere>();

        private Color LightBlue = new Color(0.5f, 0.7f, 1.0f);

        public void AddSphere(Sphere sphere)
        {
            m_spheres.Add(sphere);
        }

        public Color RayTracingBackgroud(Ray ray)
        {
            float t = 0.5f * (ray.direction.y + 1.0f);
            return Color.Lerp(Color.white, LightBlue, t);
        }

        public Color RayTracing(Ray ray)
        {
            for (int i = 0; i < m_spheres.Count; ++i)
                if (HitSphere(m_spheres[i], ray))
                    return Color.red;

            return RayTracingBackgroud(ray);
        }

        private bool HitSphere(Sphere sphere, Ray ray)
        {
            Vector3 sphereToRayOrigin = ray.origin - sphere.Position;
            float a = Vector3.Dot(ray.direction, ray.direction);
            float b = 2 * Vector3.Dot(sphereToRayOrigin, ray.direction);
            float c = Vector3.Dot(sphereToRayOrigin, sphereToRayOrigin) - sphere.Radius * sphere.Radius;
            float discriminant = b * b - 4 * a * c;
            return discriminant > 0;
        }
    }
}