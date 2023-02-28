using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nv
{
    public class TriangleExplosion : MonoBehaviour
    {
        [Header("Also destroy these whith the spawned meshes")]
        public GameObject[] destroyList;

        [Header("Also destroy these whith the spawned meshes")]
        public GameObject[] disableList;

        [Header("For every three vertices, creates a new fourth one and connects them")]
        public bool Create3dShards;

        public GameObject shrapnelExtraPrefab;
        public string shrapnelTag = "Untagged";
        public string shrapnelLayer = "Default";

        public float explosionMinForce = 500.0f;
        public float explosionMaxForce = 800.0f;
        public float explosionRadius = 5.0f;
        public float timeBeforeDestroy = 5.0f;

        public Vector3 triangleScale = Vector3.one;

        public bool useGravity = true;
        public Vector3 explosionPoint;

        [Header("If not null, will give all generated pieces this material")]
        public Material shrapnelMaterial;

        public IEnumerator SplitMesh(GameObject parent)
        {
            MeshFilter mesh_to_split = GetComponent<MeshFilter>();
            MeshRenderer mesh_details = GetComponent<MeshRenderer>();
            SkinnedMeshRenderer skinned_mesh_details = GetComponent<SkinnedMeshRenderer>();
            Collider collider = GetComponent<Collider>();

            Rigidbody drbody = GetComponent<Rigidbody>();
            if(drbody != null)
            {
                drbody.isKinematic = true;
                drbody.velocity = Vector3.zero;
            }

            for(int i = 0; i < disableList.Length; ++i)
            {
                disableList[i].SetActive(false);
            }

            Mesh M = null;

            if(mesh_to_split != null)
                M = mesh_to_split.sharedMesh;
            if(mesh_to_split == null && skinned_mesh_details != null)
                M = skinned_mesh_details.sharedMesh;

            //abort if no mesh to split
            if(M == null)
                yield break;

            //wait a frame
            yield return null;

            if(collider != null)
                collider.enabled = false;
            Material materialToUse = shrapnelMaterial;

            if(shrapnelMaterial == null && mesh_details != null)
                materialToUse = mesh_details.sharedMaterial;

            if(shrapnelMaterial == null && materialToUse == null && skinned_mesh_details != null)
                materialToUse = skinned_mesh_details.sharedMaterial;

            Vector3[] verts = M.vertices;
            Vector3[] normals = M.normals;
            Vector2[] uvs = M.uv;
            int[] triangleIndices = Create3dShards ? new int[] { 2, 1, 0, 0, 1, 3, 3, 1, 2, 2, 0, 3 } : new int[] { 0, 1, 2, 2, 1, 0 };
            for(int submesh = 0; submesh < M.subMeshCount; submesh++)
            {

                int[] indices = M.GetTriangles(submesh);

                for(int i = 0; i < indices.Length; i += 3)
                {
                    int count = Create3dShards ? 4 : 3;
                    Vector3[] newVerts = new Vector3[count];
                    Vector3[] newNormals = new Vector3[count];
                    Vector2[] newUvs = new Vector2[count];
                    for(int n = 0; n < 3; n++)
                    {
                        int index = indices[i + n];

                        newVerts[n] = verts[index];
                        if(index < uvs.Length)
                            newUvs[n] = uvs[index];
                        newNormals[n] = normals[index];
                    }

                    Mesh mesh = new Mesh();

                    if(Create3dShards)
                    {
                        Vector3 midpoint = Vector3.zero;
                        for(int n = 0; n < 3; ++n)
                            midpoint += newVerts[n] * .33f;

                        newNormals[3] = Vector3.Cross((newVerts[0] - newVerts[1]), (newVerts[0] - newVerts[2])).normalized;
                        midpoint += newNormals[3];
                        newVerts[3] = midpoint;
                        newUvs[3] = newUvs[2];  // this will look a little fucked up if actual textures are used. whatever, yolo
                    }

                    mesh.vertices = newVerts;
                    mesh.normals = newNormals;
                    mesh.uv = newUvs;
                    mesh.triangles = triangleIndices;

                    GameObject GO = new GameObject("Shard");
                    GO.tag = shrapnelTag;

                    //GO.layer = 2;// LayerMask.NameToLayer( "Particle" );
                    GO.transform.SetParent(parent.transform);
                    GO.transform.position = transform.position;
                    GO.transform.rotation = transform.rotation;
                    GO.transform.localScale = triangleScale;

                    if(materialToUse != null)
                        GO.AddComponent<MeshRenderer>().sharedMaterial = materialToUse;

                    GO.AddComponent<MeshFilter>().mesh = mesh;
                    BoxCollider bc = GO.AddComponent<BoxCollider>();
                    bc.size *= 1.1f;

                    Vector3 explosionPos = explosionPoint;
                    GO.AddComponent<Rigidbody>();

                    Rigidbody rbody = GO.GetComponent<Rigidbody>();
                    rbody.AddExplosionForce(Random.Range(explosionMinForce, explosionMaxForce), explosionPos, explosionRadius);
                    rbody.useGravity = useGravity;
                    rbody.mass = 0.01f;

                    GO.layer = UnityEngine.LayerMask.NameToLayer(shrapnelLayer); ;

                    if(shrapnelExtraPrefab != null)
                    {
                        //GameObject shObj = (GameObject)
                        Instantiate(shrapnelExtraPrefab, GO.transform, false);
                        //shObj.transform.localPosition = Vector3.zero;
                    }

                    Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
                }
            }

            if(mesh_details != null)
                mesh_details.enabled = false;
            if(skinned_mesh_details != null)
                skinned_mesh_details.enabled = false;

            yield return new WaitForSeconds(timeBeforeDestroy);

            if(destroyList != null)
            {
                for(int i = 0; i < destroyList.Length; ++i)
                {
                    Destroy(destroyList[i]);
                }
            }

            if(parent != null)
            {
                Destroy(gameObject);
            }

        }

        [ContextMenu("Explode")]
        public void ExplodeMesh()
        {
            StartCoroutine(SplitMesh(gameObject));
        }
    }
}