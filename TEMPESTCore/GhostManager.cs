using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Manager for ghost enemies, WIP its still a bit complex i plan to recreate the medium and this is a crucial component
    /// i kinda stopped understanding how this thing works halfway through it needs refactoring
    /// its a bit of an experiment on my end on how to handle stuff like this - h
    /// </summary>
    public class GhostManager : MonoBehaviour
    {
        public bool debugMode = false;

        // Tracks: Parent EID -> List of Ghosted components associated with it
        [SerializeField] private Dictionary<EnemyIdentifier, List<Ghosted>> _registry = new Dictionary<EnemyIdentifier, List<Ghosted>>();
        [SerializeField] private List<Coroutine> _activeSpawnRoutines = new List<Coroutine>();

        #region Core Logic

        public void RegisterGhost(EnemyIdentifier parent, Ghosted ghost)
        {
            if (parent == null || ghost == null) return;

            if (!_registry.ContainsKey(parent))
            {
                _registry.Add(parent, new List<Ghosted>());
                parent.onDeath.AddListener(() => OnParentDeath(parent));
                if (debugMode) Debug.Log($"<color=cyan>[GhostManager]</color> Subscribed to new Parent: {parent.gameObject.name}");
            }

            if (!_registry[parent].Contains(ghost))
            {
                _registry[parent].Add(ghost);
                if (debugMode) Debug.Log($"<color=cyan>[GhostManager]</color> Registered Ghost {ghost.gameObject.name} to Parent {parent.gameObject.name}");
            }
        }

        public void RequestSpawn(float delay, GameObject copy, Vector3 pos, Quaternion rot, EnemyIdentifier parentRef)
        {
            if (debugMode) Debug.Log($"<color=cyan>[GhostManager]</color> Spawn requested for {copy?.name} with delay {delay}s");
            _activeSpawnRoutines.Add(StartCoroutine(SpawnRoutine(delay, copy, pos, rot, parentRef)));
        }

        private IEnumerator SpawnRoutine(float delay, GameObject copy, Vector3 pos, Quaternion rot, EnemyIdentifier parentRef)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            if (copy != null)
            {
                copy.transform.SetPositionAndRotation(pos, rot);
                copy.SetActive(true);

                yield return new WaitForEndOfFrame();

                if (copy.TryGetComponent<Ghosted>(out var ghost))
                {
                    // Check if parent died during the spawn delay
                    if (!ghost.dontDieWithParent && (parentRef == null || parentRef.dead))
                    {
                        if (debugMode) Debug.Log($"<color=red>[GhostManager]</color> Parent died before spawn: Destroying {copy.name}");
                        Destroy(copy);
                        yield break;
                    }

                    ghost.Toggle(true);
                    RegisterGhost(parentRef, ghost);
                }

                if (debugMode) Debug.Log($"<color=cyan>[GhostManager]</color> Successfully birthed ghost: {copy.name}");
            }
        }

        private void OnParentDeath(EnemyIdentifier parent)
        {
            if (parent == null || !_registry.TryGetValue(parent, out List<Ghosted> ghosts)) return;

            if (debugMode) Debug.Log($"<color=orange>[GhostManager]</color> Parent DIED: {parent.name}. Processing {ghosts.Count} children.");

            for (int i = ghosts.Count - 1; i >= 0; i--)
            {
                var ghost = ghosts[i];
                if (ghost == null) continue;

                if (ghost.dontDieWithParent) continue;

                switch (ghost.parentDeathMode)
                {
                    case GhostedParentDeathMode.DestroyOnParentDeath:
                        Destroy(ghost.gameObject);
                        break;
                    case GhostedParentDeathMode.UnghostOnParentDeath:
                        ghost.Toggle(false);
                        break;
                    case GhostedParentDeathMode.StartTimer:
                        ghost.StartGhostTimer();
                        break;
                }
            }

            _registry.Remove(parent);
        }

        #endregion

        #region Debug & Utility Methods

        public void ToggleAllGhosts(bool value)
        {
            foreach (var list in _registry.Values)
            {
                foreach (var ghost in list)
                {
                    if (ghost != null) ghost.Toggle(value);
                }
            }
            if (debugMode) Debug.Log($"<color=yellow>[GhostManager]</color> Toggled all ghosts to: {value}");
        }

        public void DestroyAllGhosts()
        {
            foreach (var list in _registry.Values)
            {
                foreach (var ghost in list)
                {
                    if (ghost != null) Destroy(ghost.gameObject);
                }
            }
            _registry.Clear();
            if (debugMode) Debug.Log("<color=red>[GhostManager]</color> DESTROYED ALL GHOSTS");
        }

        public void KillAllGhostParents()
        {
            List<EnemyIdentifier> parents = new List<EnemyIdentifier>(_registry.Keys);
            foreach (var p in parents)
            {
                if (p != null && !p.dead) p.InstaKill();
            }
            if (debugMode) Debug.Log("<color=red>[GhostManager]</color> KILLED ALL GHOST PARENTS");
        }

        public void CleanRegistry()
        {
            List<EnemyIdentifier> toRemove = new List<EnemyIdentifier>();
            foreach (var kvp in _registry)
            {
                if (kvp.Key == null) toRemove.Add(kvp.Key);
                else kvp.Value.RemoveAll(g => g == null);
            }
            foreach (var r in toRemove) _registry.Remove(r);
        }

        public void DebugPrintGhosts()
        {
            StringBuilder sb = new StringBuilder("<color=cyan>[GhostManager] Active Ghosts:</color>\n");
            int count = 0;
            foreach (var list in _registry.Values)
            {
                foreach (var g in list)
                {
                    if (g != null)
                    {
                        sb.AppendLine($"- {g.gameObject.name} (Parent: {(g.parent != null ? g.parent.name : "None")})");
                        count++;
                    }
                }
            }
            Debug.Log($"{sb}Total Ghost Count: {count}");
        }

        public void DebugPrintParents()
        {
            StringBuilder sb = new StringBuilder("<color=cyan>[GhostManager] Registered Parents:</color>\n");
            foreach (var p in _registry.Keys)
            {
                if (p != null) sb.AppendLine($"- {p.name} (Children: {_registry[p].Count})");
            }
            Debug.Log($"{sb}Total Parent Count: {_registry.Count}");
        }

        public void StopAllSpawns()
        {
            foreach (var routine in _activeSpawnRoutines)
            {
                if (routine != null) StopCoroutine(routine);
            }
            _activeSpawnRoutines.Clear();
            if (debugMode) Debug.Log("<color=yellow>[GhostManager]</color> All pending spawns cancelled.");
        }

        #endregion

        private void OnDisable()
        {
            StopAllSpawns();
            foreach (var parent in _registry.Keys)
            {
                if (parent != null) parent.onDeath.RemoveListener(() => OnParentDeath(parent));
            }
        }
    }
}