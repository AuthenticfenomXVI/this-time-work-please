using BepInEx;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HellsCity
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "org.hellscity.limbus-ultrakill";
        public const string PluginName = "Hell's City: Limbus × ULTRAKILL";
        public const string PluginVersion = "0.0.1-prototype";
        private bool _visible = true;
        private string _limbusPath;
        private readonly List<string> _assetCandidates = new List<string>();
        private readonly List<string> _missions = new List<string>();
        private int _missionIndex;
        private string _status = "Starting diagnostics…";
        private GameObject _testLevelRoot;

        private void Awake()
        {
            Logger.LogInfo($"{PluginName} {PluginVersion} loaded. This is a diagnostic prototype, not a playable campaign yet.");
            _limbusPath = SteamLimbusLocator.FindInstall(Logger);
            if (string.IsNullOrEmpty(_limbusPath))
                _status = "Limbus Company not found in scanned Steam libraries. Install it or verify the Steam library path.";
            else
            {
                _status = "Limbus install found; inspecting candidate files (not loading/extracting them).";
                InventoryCandidates(_limbusPath);
            }
            LoadMissionNames();
        }

private void Update()
{
    if (Input.GetKeyDown(KeyCode.F8)) _visible = !_visible;
    if (Input.GetKeyDown(KeyCode.F9)) BuildGreyboxTestLevel();
    if (Input.GetKeyDown(KeyCode.F10)) RemoveGreyboxTestLevel();

    if (Input.GetKeyDown(KeyCode.LeftBracket) && _missions.Count > 0)
        _missionIndex = (_missionIndex + _missions.Count - 1) % _missions.Count;
    if (Input.GetKeyDown(KeyCode.RightBracket) && _missions.Count > 0)
        _missionIndex = (_missionIndex + 1) % _missions.Count;
}
        private void LoadMissionNames()
        {
            // Read mission names from the data sheet when packaged beside the plugin.
            // The fallback is only a resilience path; Data/areas.json remains the design source of truth.
            var candidates = new[] {
                Path.Combine(Path.GetDirectoryName(Info.Location) ?? Paths.PluginPath, "Data", "areas.json"),
                Path.Combine(Paths.PluginPath, "HellsCity", "Data", "areas.json")
            };
            foreach (var file in candidates)
            {
                if (!File.Exists(file)) continue;
                try
                {
                    var json = File.ReadAllText(file);
                    foreach (Match match in Regex.Matches(json, "\\\"name\\\"\\s*:\\s*\\\"([^\\\"]+)\\\""))
                    {
                        var name = match.Groups[1].Value;
                        if (!string.IsNullOrWhiteSpace(name)) _missions.Add(name);
                    }
                    if (_missions.Count > 0) return;
                }
                catch (Exception ex) { Logger.LogWarning("Could not read campaign sheet: " + ex.Message); }
            }
            _missions.AddRange(new[] {
                "01 — Backstreets: Blood-Red Welcome",
                "02 — Abandoned L Corp Branch",
                "03 — Workshop Row",
                "04 — W Corp Transit Ruins",
                "05 — The Nest Perimeter",
                "06 — The Golden Bough Vault"
            });
        }

        private void InventoryCandidates(string root)
        {
            try
            {
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".bundle", ".assets", ".assetbundle", ".unity3d" };
                foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                {
                    if (allowed.Contains(Path.GetExtension(file))) _assetCandidates.Add(file);
                    if (_assetCandidates.Count >= 500) break;
                }
                Logger.LogInfo($"Limbus asset inventory: {_assetCandidates.Count} candidate files in the first scan window. No assets were opened or copied.");
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Could not inventory Limbus install: " + ex.Message);
            }
        }

private void BuildGreyboxTestLevel()
{
    RemoveGreyboxTestLevel();

    var cam = Camera.main;
    if (cam == null)
    {
        _status = "No main camera found.";
        Logger.LogWarning(_status);
        return;
    }

    var forward = cam.transform.forward;
    forward.y = 0f;

    if (forward.sqrMagnitude < 0.01f)
        forward = Vector3.forward;

    forward.Normalize();

    var origin = cam.transform.position + forward * 10f;
    origin.y = cam.transform.position.y - 2f;

    _testLevelRoot = new GameObject("HellsCity_GreyboxTestLevel");
    _testLevelRoot.transform.position = origin;
    _testLevelRoot.transform.rotation =
        Quaternion.LookRotation(forward, Vector3.up);

    // Starting platform
    MakeBlock("Start Platform",
        new Vector3(0f, -0.5f, 0f),
        new Vector3(12f, 1f, 10f),
        new Color(0.25f, 0.25f, 0.3f));

    // Alley floor
    MakeBlock("Alley Floor",
        new Vector3(0f, -0.5f, 15f),
        new Vector3(7f, 1f, 24f),
        new Color(0.32f, 0.25f, 0.25f));

    // Alley walls
    MakeBlock("Left Wall",
        new Vector3(-4f, 2f, 15f),
        new Vector3(1f, 5f, 24f),
        new Color(0.18f, 0.18f, 0.2f));

    MakeBlock("Right Wall",
        new Vector3(4f, 2f, 15f),
        new Vector3(1f, 5f, 24f),
        new Color(0.18f, 0.18f, 0.2f));

    // Open arena
    MakeBlock("Arena Floor",
        new Vector3(0f, -0.5f, 37f),
        new Vector3(22f, 1f, 16f),
        new Color(0.22f, 0.22f, 0.25f));

    // Ruined structures
    MakeBlock("Broken Column",
        new Vector3(-8f, 1.5f, 38f),
        new Vector3(2f, 4f, 5f),
        new Color(0.28f, 0.24f, 0.22f));

    MakeBlock("Collapsed Wall",
        new Vector3(8f, 1f, 34f),
        new Vector3(2f, 3f, 6f),
        new Color(0.28f, 0.24f, 0.22f));

    // Red exit marker
    MakeBlock("Exit Marker",
        new Vector3(0f, 1.5f, 44f),
        new Vector3(3f, 3f, 0.5f),
        new Color(0.8f, 0.05f, 0.05f));

    _status = "Greybox created. Press F10 to remove it.";
    Logger.LogInfo(_status);
}

private void MakeBlock(
    string blockName,
    Vector3 localPosition,
    Vector3 localScale,
    Color color)
{
    var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
    block.name = blockName;
    block.transform.SetParent(_testLevelRoot.transform, false);
    block.transform.localPosition = localPosition;
    block.transform.localScale = localScale;

    var renderer = block.GetComponent<Renderer>();
    if (renderer != null)
    {
        var shader = Shader.Find("Standard");
        if (shader != null)
        {
            var material = new Material(shader);
            material.color = color;
            renderer.material = material;
        }
    }
}

private void RemoveGreyboxTestLevel()
{
    if (_testLevelRoot == null) return;

    Destroy(_testLevelRoot);
    _testLevelRoot = null;
    _status = "Temporary greybox removed.";
}
        private void OnGUI()
        {
            if (!_visible) return;
            GUI.Box(new Rect(18, 18, 520, 264), "HELL'S CITY — CAMPAIGN PROTOTYPE");
            GUI.Label(new Rect(34, 48, 485, 24), "F8: hide/show  |  [ and ]: browse planned missions");
            GUI.Label(new Rect(34, 78, 485, 24), _missions.Count == 0 ? "No campaign sheet loaded." : _missions[_missionIndex]);
            GUI.Label(new Rect(34, 108, 485, 42), "Limbus: " + (string.IsNullOrEmpty(_limbusPath) ? "not detected" : "detected locally"));
            GUI.Label(new Rect(34, 150, 485, 42), "Asset candidates: " + _assetCandidates.Count + " (inventory only; compatibility not verified)");
            GUI.Label(new Rect(34, 188, 485, 42), "STATUS: " + _status);
            GUI.Label(new Rect(34, 218, 485, 24), "F9: create test level | F10: remove it");
            GUI.Label(new Rect(34, 242, 485, 24), "Greybox only; no enemies or mission transitions.");
        }
    }

    internal static class SteamLimbusLocator
    {
        private const string LimbusAppId = "1973530";

        public static string FindInstall(BepInEx.Logging.ManualLogSource log)
        {
            var candidates = new List<string>();
            var steamRoots = new List<string>();
            var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            AddIfExists(steamRoots, Path.Combine(pf86, "Steam"));
            AddIfExists(steamRoots, Path.Combine(pf, "Steam"));
            AddIfExists(steamRoots, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam"));

            foreach (var root in steamRoots.ToArray())
            {
                AddIfExists(candidates, Path.Combine(root, "steamapps", "common", "LimbusCompany"));
                AddIfExists(candidates, Path.Combine(root, "steamapps", "common", "Limbus Company"));
                var vdf = Path.Combine(root, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(vdf)) continue;
                try
                {
                    var text = File.ReadAllText(vdf);
                    foreach (Match m in Regex.Matches(text, "\\\"path\\\"\\s+\\\"([^\\\"]+)\\\""))
                    {
                        var library = m.Groups[1].Value.Replace("\\\\", "\\");
                        AddIfExists(candidates, Path.Combine(library, "steamapps", "common", "LimbusCompany"));
                        AddIfExists(candidates, Path.Combine(library, "steamapps", "common", "Limbus Company"));
                    }
                }
                catch (Exception ex) { log.LogDebug("Could not read Steam library list: " + ex.Message); }
            }

            foreach (var path in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var manifest = Path.GetFullPath(Path.Combine(path, "..", "..", "appmanifest_" + LimbusAppId + ".acf"));
                if (Directory.Exists(path) && File.Exists(manifest)) return path;
            }
            return candidates.FirstOrDefault(Directory.Exists);
        }

        private static void AddIfExists(List<string> list, string path)
        {
            if (Directory.Exists(path)) list.Add(path);
        }
    }
}
