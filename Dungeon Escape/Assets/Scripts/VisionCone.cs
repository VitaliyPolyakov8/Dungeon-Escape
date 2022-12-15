﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class VisionCone : MonoBehaviour
    {
        [Header("Vision")]
        public float vision_angle = 30f;
        public float vision_range = 5f;
        public float vision_near_range = 3f;
        public LayerMask obstacle_mask = ~(0);


        [Header("Material")]
        public Material cone_material;
        public Material cone_far_material;
        public int sort_order = 1;

        private MeshRenderer render;
        private MeshFilter mesh;

        private float timer = 0f;
        private AI _ai;

        private void Awake()
        {
  
            _ai = GetComponent<AI>();
            render = gameObject.AddComponent<MeshRenderer>();
            mesh = gameObject.AddComponent<MeshFilter>();
            render.sharedMaterial = cone_far_material;
            render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.receiveShadows = false;
            render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            render.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            render.allowOcclusionWhenDynamic = false;
            render.sortingOrder = sort_order;
          

        }

        private void Start()
        {
           
            InitMesh(mesh, false);

        }

        private void InitMesh(MeshFilter mesh, bool far)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();

            if (!far)
            {
                vertices.Add(new Vector3(0f, 0f, 0f));
                normals.Add(Vector3.up);
                uv.Add(Vector2.zero);
            }

            int minmax = Mathf.RoundToInt(vision_angle / 2f);

            int tri_index = 0;
            float step_jump = Mathf.Clamp(vision_angle, 0.01f, minmax);

            for (float i = -minmax; i <= minmax; i += step_jump)
            {
                float angle = (float)(i + 90f) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle) * vision_range, 0f, Mathf.Sin(angle) * vision_range);

                vertices.Add(dir);
                normals.Add(Vector2.up);
                uv.Add(Vector2.zero);

                if (far)
                {
                    vertices.Add(dir);
                    normals.Add(Vector2.up);
                    uv.Add(Vector2.zero);
                }

                if (tri_index > 0)
                {
                    if (far)
                    {
                        triangles.Add(tri_index);
                        triangles.Add(tri_index+1);
                        triangles.Add(tri_index-2);

                        triangles.Add(tri_index - 2);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index - 1);
                    }
                    else
                    {
                        triangles.Add(0);
                        triangles.Add(tri_index + 1);
                        triangles.Add(tri_index);
                    }
                }
                tri_index += far ? 2 : 1;
            }

            mesh.mesh.vertices = vertices.ToArray();
            mesh.mesh.triangles = triangles.ToArray();
            mesh.mesh.normals = normals.ToArray();
            mesh.mesh.uv = uv.ToArray();
        }

        private void Update()
        {
            timer += Time.deltaTime;



            if (_ai.Detect == true)
                render.sharedMaterial = cone_material;
            if (_ai.Detect == false)
                render.sharedMaterial = cone_far_material;
        float range = vision_range;


        UpdateMainLevel(mesh, range);


        }
    private void UpdateMainLevel(MeshFilter mesh, float range)
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0f, 0f, 0f));

        int minmax = Mathf.RoundToInt(vision_angle / 2f);
        float step_jump = Mathf.Clamp(vision_angle, 0.01f, minmax);
        for (float i = -minmax; i <= minmax; i += step_jump)
        {
            float angle = (float)(i + 90f) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle) * range, 0f, Mathf.Sin(angle) * range);

            RaycastHit hit;
            Vector3 pos_world = transform.TransformPoint(Vector3.zero);
            Vector3 dir_world = transform.TransformDirection(dir.normalized);
            bool ishit = Physics.Raycast(new Ray(pos_world, dir_world), out hit, range, obstacle_mask.value);
            if (ishit)
                dir = dir.normalized * hit.distance;
            Debug.DrawRay(pos_world, dir_world * (ishit ? hit.distance : range));

            vertices.Add(dir);
        }

        mesh.mesh.vertices = vertices.ToArray();
        mesh.mesh.RecalculateBounds();
    }

}
