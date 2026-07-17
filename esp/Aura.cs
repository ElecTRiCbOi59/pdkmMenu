using GameNetcodeStuff;
using pdkmMenu;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Aura : MonoBehaviour
{
    private Material auraMaterial;
    private List<GameObject> ghostObjects = new List<GameObject>();
    private MonoBehaviour targetComponent; // 1. Added to save target

    public Color auraColor = Color.cyan;
    private static ESPConfig ESPSettings => pdkmMenu.Plugin.ESPSettings;
    public void Initialize<T>(T target) where T : MonoBehaviour
    {
        targetComponent = target; // 2. Save target here

        // Setup the aura Material (X-Ray Overlay)
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        auraMaterial = new Material(shader);
        auraMaterial.hideFlags = HideFlags.HideAndDontSave;

        auraMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        auraMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        auraMaterial.SetInt("_Cull", (int)CullMode.Off);
        auraMaterial.SetInt("_ZTest", (int)CompareFunction.Always);
        auraMaterial.SetInt("_ZWrite", 0);
        auraMaterial.renderQueue = 3500;

        // 3. Start the initial scan
        CreateAuras();
    }

    // 4. Added for Harmony patch to call
    public void RefreshGhosts()
    {
        foreach (var ghost in ghostObjects)
        {
            if (ghost != null) Destroy(ghost);
        }
        ghostObjects.Clear();

        CreateAuras();
    }

    // 5. Moved your exact original loop into this method
    private void CreateAuras()
    {
        if (targetComponent == null) return;
        if (targetComponent is Turret)
        {
            // We go to the parent (TurretContainer) to see the whole hierarchy
            GameObject turretRoot = targetComponent.transform.parent.gameObject;
            List<Renderer> turretRenders = GetRenderers(turretRoot);

            foreach (var originalRen in turretRenders)
            {
                if (originalRen == null) continue;

                string name = originalRen.name.ToLower();

                // 3. TARGET ONLY: The parts that make up the actual turret body
                // These are the main renderers found in your hierarchy log
                bool isBodyPart = name.Contains("meshcontainer") ||
                                  name.Contains("mount") ||
                                  name.Contains("rod") ||
                                  name.Contains("gunbody") ||
                                  name.Contains("magazine");

                if (isBodyPart)
                {
                    DoThing(originalRen);
                }
            }
            return;
        }
        if (targetComponent is SpikeRoofTrap)
        {
            // We go to the parent (TurretContainer) to see the whole hierarchy
            GameObject turretRoot = targetComponent.transform.parent.gameObject;
            List<Renderer> turretRenders = GetRenderers(turretRoot);

            foreach (var originalRen in turretRenders)
            {
                if (originalRen == null) continue;

                string name = originalRen.name.ToLower();

                // 3. TARGET ONLY: The parts that make up the actual turret body
                // These are the main renderers found in your hierarchy log
                bool isBodyPart =  name.Contains("movingbar") ||
                                            name.Contains("spikeroof") ||
                                            name.Contains("basesupport");

                if (isBodyPart)
                {
                    DoThing(originalRen);
                }
            }
            return;
        }
        if (targetComponent is PlayerControllerB)
        {
            SkinnedMeshRenderer meshRenderer = targetComponent.GetComponent<PlayerControllerB>().thisPlayerModel;
            if (meshRenderer != null || meshRenderer.gameObject.activeInHierarchy)
            {
                DoThing(meshRenderer);
            }
            return;
        }

        List<Renderer> allRenderers = GetRenderers(targetComponent);

        foreach (var originalRen in allRenderers)
            {
                if (originalRen == null || !originalRen.gameObject.activeInHierarchy) continue;
                if (originalRen is ParticleSystemRenderer || originalRen is TrailRenderer) continue;

                if (targetComponent is EnemyAI)
                {
                    if (!(originalRen is SkinnedMeshRenderer)) continue;
                }
                if (targetComponent is GrabbableObject)
                {
                    bool bad = originalRen.name.ToLower().Contains("scannode");
                    bool bad2 = originalRen.name.ToLower().Contains("radarboosterdot");
                    if (bad || bad2) continue;
                }
                //if(targetComponent is Turret)
                //{
                //    if (originalRen.name.ToLower().Contains("rangeindicator")) continue;
                //}
            DoThing(originalRen);
        }
    }
    private void DoThing(Renderer originalRen)
    {
        GameObject ghost = new GameObject("aura_Ghost_" + originalRen.name);
        ghost.transform.SetParent(originalRen.transform, false);
        ghost.transform.localPosition = Vector3.zero;
        ghost.transform.localRotation = Quaternion.identity;
        ghost.transform.localScale = Vector3.one;

        if (originalRen is SkinnedMeshRenderer smr)
        {
            SkinnedMeshRenderer ghostSmr = ghost.AddComponent<SkinnedMeshRenderer>();
            ghostSmr.sharedMesh = smr.sharedMesh;
            ghostSmr.bones = smr.bones;
            ghostSmr.rootBone = smr.rootBone;
            ApplyauraMaterials(ghostSmr, smr.sharedMaterials.Length);
        }
        else if (originalRen is MeshRenderer mr)
        {
            MeshFilter mf = originalRen.GetComponent<MeshFilter>();
            if (mf != null)
            {
                ghost.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
                MeshRenderer ghostMr = ghost.AddComponent<MeshRenderer>();
                ApplyauraMaterials(ghostMr, mr.sharedMaterials.Length);
            }
        }
        ghostObjects.Add(ghost);
    }

    private void ApplyauraMaterials(Renderer ren, int count)
    {
        Material[] mats = new Material[count];
        for (int i = 0; i < count; i++) mats[i] = auraMaterial;
        ren.sharedMaterials = mats;
        ren.shadowCastingMode = ShadowCastingMode.Off;
        ren.receiveShadows = false;
    }

    private List<Renderer> GetRenderers(Object obj)
    {
        List<Renderer> list = new List<Renderer>();
        if (obj == null) return list;
        if (obj is GameObject go) list.AddRange(go.GetComponentsInChildren<Renderer>());
        else if (obj is Component comp) list.AddRange(comp.GetComponentsInChildren<Renderer>());

        return list;
    }

    void Update()
    {
        var settings = ESPSettings;
        if (auraMaterial != null)
        {
            Color c = auraColor;
            if (settings != null)
                c.a = settings.AurasOpacity.Value;
            auraMaterial.SetColor("_Color", c);
        }
    }

    void OnDestroy()
    {
        foreach (var ghost in ghostObjects)
        {
            if (ghost != null) Destroy(ghost);
        }
        if (auraMaterial != null) Destroy(auraMaterial);
    }
}