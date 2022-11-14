using UnityEngine;
using System.Collections.Generic;

namespace StartFramework.GameplayFramework
{
    [RequireComponent(typeof(LineRenderer))]
    public class Cable : MonoBehaviour
    {
        [SerializeField] private float gravity = 30f;
        [SerializeField] private int stiffness = 10;
        [SerializeField] private bool startPointLock;
        [SerializeField] private bool endPointLock;
        [SerializeField] private bool isHair; //起始点随父物体移动

        private LineRenderer lineRenderer;
        private List<Particle> particles = new List<Particle>();
        private List<Stick> sticks = new List<Stick>();

        private void Start()
        {
            Initialization();
        }

        private void FixedUpdate()
        {
            Simulation();
        }

        private void LateUpdate()
        {
            Rendering();
        }

        private void Initialization()
        {
            lineRenderer = GetComponent<LineRenderer>();

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 dot = lineRenderer.GetPosition(i);
                particles.Add(new Particle() { position = dot, oldPosition = dot });
            }
            for (int i = 0; i < particles.Count - 1; i++)
            {
                sticks.Add(new Stick(particles[i], particles[i + 1]));
            }

            if (startPointLock)
            {
                particles[0].locked = true;
            }
            if (endPointLock)
            {
                particles[particles.Count - 1].locked = true;
            }
            if (isHair)
            {
                particles[0].locked = true;
            }
        }

        private void Simulation()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle p = particles[i];
                if (p.locked == false)
                {
                    Vector2 temp = p.position;
                    p.position = p.position + (p.position - p.oldPosition) + Time.fixedDeltaTime * Time.fixedDeltaTime * new Vector2(0, -gravity);
                    p.oldPosition = temp;
                }
            }

            for (int i = 0; i < stiffness; i++)
            {
                for (int j = 0; j < sticks.Count; j++)
                {
                    Stick stick = sticks[j];

                    Vector2 delta = stick.particleB.position - stick.particleA.position;
                    float deltaLength = delta.magnitude;
                    float diff = (deltaLength - stick.length) / deltaLength;
                    if (stick.particleA.locked == false)
                        stick.particleA.position += 0.5f * diff * delta;
                    if (stick.particleB.locked == false)
                        stick.particleB.position -= 0.5f * diff * delta;
                }
            }

            if (isHair)
            {
                //particles[0].position = transform.position;
                particles[0].position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void Rendering()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                lineRenderer.SetPosition(i, particles[i].position);
            }
        }
    }

    public class Particle
    {
        public Vector2 position;
        public Vector2 oldPosition;
        public bool locked;
    }

    public class Stick
    {
        public Particle particleA;
        public Particle particleB;
        public float length;

        public Stick(Particle a, Particle b)
        {
            particleA = a;
            particleB = b;
            length = (a.position - b.position).magnitude;
        }
    }
}