using System.Collections.Generic;
using UnityEngine;
using UnknownMan.Player;

namespace UnknownMan.World
{
    public static class ProceduralVillageBuilder
    {
        private static readonly Dictionary<string, Material> materials = new();
        private static Transform root;

        public static void Build()
        {
            if (GameObject.Find("Village") != null) return;

            var village = new GameObject("Village");
            root = village.transform;
            BuildMaterials();
            BuildGround();
            BuildRoads();
            BuildHouses();
            BuildForest();
            BuildLandmarks();
            BuildNightLighting();
            BuildAtmosphere();
            BuildDistantFigure();
        }

        private static void BuildMaterials()
        {
            materials["ground"] = MakeMaterial("Ground", new Color(0.17f, 0.19f, 0.16f));
            materials["road"] = MakeMaterial("Road", new Color(0.10f, 0.11f, 0.10f));
            materials["wall"] = MakeMaterial("Wall", new Color(0.29f, 0.31f, 0.28f));
            materials["roof"] = MakeMaterial("Roof", new Color(0.09f, 0.10f, 0.09f));
            materials["wood"] = MakeMaterial("Wood", new Color(0.22f, 0.16f, 0.11f));
            materials["metal"] = MakeMaterial("Metal", new Color(0.25f, 0.28f, 0.29f), 0.2f);
            materials["leaf"] = MakeMaterial("Leaf", new Color(0.09f, 0.13f, 0.10f));
            materials["paper"] = MakeMaterial("Paper", new Color(0.70f, 0.65f, 0.53f));
            materials["stone"] = MakeMaterial("Stone", new Color(0.28f, 0.29f, 0.27f));
            materials["warning"] = MakeMaterial("Warning", new Color(0.38f, 0.31f, 0.20f));
        }

        private static Material MakeMaterial(string name, Color color, float metallic = 0f)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = name };
            material.color = color;
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.15f);
            return material;
        }

        private static void BuildGround()
        {
            CreateCube("Ground", new Vector3(0f, -0.35f, 35f), new Vector3(110f, 0.5f, 110f), materials["ground"]);
        }

        private static void BuildRoads()
        {
            CreateCube("MainRoad", new Vector3(0f, -0.05f, 28f), new Vector3(8f, 0.1f, 75f), materials["road"]);
            CreateCube("CrossRoad", new Vector3(10f, -0.04f, 34f), new Vector3(70f, 0.12f, 6f), materials["road"]);
        }

        private static void BuildHouses()
        {
            House("House_A", new Vector3(-15f, 0f, 22f), 8f, 5.5f, true);
            House("House_B", new Vector3(16f, 0f, 25f), 7f, 5f, false);
            House("House_C", new Vector3(-20f, 0f, 43f), 8f, 6f, false);
            House("House_D", new Vector3(21f, 0f, 46f), 7f, 5f, true);
            House("Shop", new Vector3(-10f, 0f, 55f), 9f, 5.5f, true);
            House("School", new Vector3(16f, 0f, 63f), 11f, 7f, false);
            House("Storage", new Vector3(-25f, 0f, 66f), 6f, 5f, false);
        }

        private static void House(string name, Vector3 center, float width, float depth, bool evidence)
        {
            var house = new GameObject(name);
            house.transform.SetParent(root);
            float wallHeight = 4.2f;
            float wallY = center.y + wallHeight * 0.5f;
            float halfW = width * 0.5f;
            float halfD = depth * 0.5f;
            float doorGap = 1.55f;

            // Four wall runs, with the front wall split to leave a real doorway.
            CreateCube(name + "_BackWall", center + new Vector3(0f, wallHeight * 0.5f, -halfD), new Vector3(width, wallHeight, 0.25f), materials["wall"], house.transform);
            CreateCube(name + "_LeftWall", center + new Vector3(-halfW, wallHeight * 0.5f, 0f), new Vector3(0.25f, wallHeight, depth), materials["wall"], house.transform);
            CreateCube(name + "_RightWall", center + new Vector3(halfW, wallHeight * 0.5f, 0f), new Vector3(0.25f, wallHeight, depth), materials["wall"], house.transform);
            float sideFront = (width - doorGap) * 0.5f;
            CreateCube(name + "_FrontLeft", center + new Vector3(-(doorGap + sideFront) * 0.5f, wallY, halfD), new Vector3(sideFront, wallHeight, 0.25f), materials["wall"], house.transform);
            CreateCube(name + "_FrontRight", center + new Vector3((doorGap + sideFront) * 0.5f, wallY, halfD), new Vector3(sideFront, wallHeight, 0.25f), materials["wall"], house.transform);

            var roof = CreateCube(name + "_Roof", center + Vector3.up * 4.5f, new Vector3(width + 0.6f, 0.6f, depth + 0.6f), materials["roof"], house.transform);
            roof.transform.rotation = Quaternion.Euler(0f, 0f, 2f);

            var door = CreateCube(name + "_Door", center + new Vector3(0f, 1.25f, halfD + 0.04f), new Vector3(1.4f, 2.5f, 0.12f), materials["wood"], house.transform);
            door.AddComponent<DoorInteractable>();

            if (evidence)
            {
                var diary = CreateCube(name + "_Evidence", center + new Vector3(1.5f, 1.2f, halfD - 1.0f), new Vector3(0.45f, 0.08f, 0.55f), materials["paper"], house.transform);
                diary.AddComponent<EvidenceInteractable>();
            }
        }

        private static void BuildForest()
        {
            var random = new System.Random(7);
            for (int i = 0; i < 55; i++)
            {
                float x = (float)(random.NextDouble() * 95.0 - 47.5);
                float z = (float)(random.NextDouble() * 92.0 + 5.0);
                if (Mathf.Abs(x) < 27f && z < 72f) continue;
                Tree(new Vector3(x, 0f, z), 0.7f + (float)random.NextDouble() * 0.9f);
            }
        }

        private static void Tree(Vector3 position, float scale)
        {
            var go = new GameObject("Tree");
            go.transform.SetParent(root);
            var trunk = CreateCube("Trunk", position + Vector3.up * (1.6f * scale), new Vector3(0.45f, 3.2f, 0.45f) * scale, materials["wood"], go.transform);
            var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "Canopy";
            canopy.transform.SetParent(go.transform);
            canopy.transform.localPosition = new Vector3(0f, 3.8f * scale, 0f);
            canopy.transform.localScale = Vector3.one * (3.0f * scale);
            canopy.GetComponent<Renderer>().sharedMaterial = materials["leaf"];
            canopy.GetComponent<Collider>().enabled = false;
        }

        private static void BuildLandmarks()
        {
            CreateCylinder("OldWell", new Vector3(5f, 0.35f, 66f), 1.5f, 0.7f, materials["stone"]);
            CreateCube("FinalGate", new Vector3(0f, 2f, 88f), new Vector3(9f, 4f, 0.8f), materials["metal"]);
            CreateCube("FinalMarker", new Vector3(0f, 0.1f, 84f), new Vector3(3f, 0.2f, 2f), materials["warning"]);
            var sign = CreateCube("FinalSign", new Vector3(0f, 2.3f, 84f), new Vector3(2.8f, 1.3f, 0.15f), materials["wood"]);
            var clue = sign.AddComponent<EvidenceInteractable>();
            _ = clue;
        }

        private static void BuildNightLighting()
        {
            var go = new GameObject("MoonLight");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.47f, 0.52f, 0.62f);
            light.intensity = 0.38f;
            light.shadows = LightShadows.Soft;
            go.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static void BuildAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.055f, 0.065f, 0.075f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.ambientLight = new Color(0.055f, 0.06f, 0.07f);
        }

        private static void BuildDistantFigure()
        {
            var figure = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            figure.name = "DistantSilhouette_Milestone1";
            figure.transform.SetParent(root);
            figure.transform.position = new Vector3(-5f, 1.5f, 54f);
            figure.transform.localScale = new Vector3(0.5f, 2.0f, 0.5f);
            var material = MakeMaterial("Silhouette", new Color(0.015f, 0.017f, 0.018f));
            figure.GetComponent<Renderer>().sharedMaterial = material;
            figure.GetComponent<Collider>().enabled = false;
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent ?? root);
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        private static GameObject CreateCylinder(string name, Vector3 position, float radius, float height, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;
            go.transform.SetParent(root);
            go.transform.position = position;
            go.transform.localScale = new Vector3(radius, height, radius);
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }
    }
}
