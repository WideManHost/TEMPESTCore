using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TEMPESTCore
{
    /// <summary>
    /// Manager for ghost enemies, WIP its still a bit complex i plan to recreate the medium and this is a crucial component
    /// i kinda stopped understanding how this thing works halfway through it needs refactoring
    /// its a bit of an experiment on my end on how to handle stuff like this - h
    /// man we not doing this
    /// </summary>
    public enum GhostType
    {
        Parent = 0,
        Ghostable = 1,
        Blueprint = 2,
        Instance = 3
    }

    public class GhostManager : MonoBehaviour
    {
        public bool debugMode = false;
        public bool destroyOrphans = true;

        // Tracks: Parent EID -> List of Ghosted components associated with it
        [SerializeField] private Dictionary<EnemyIdentifier, List<Ghosted>> _registry = new Dictionary<EnemyIdentifier, List<Ghosted>>();
        private HashSet<Ghosted> _trackedGhosts = new HashSet<Ghosted>();
        private List<Ghosted> _unparentedGhosts = new List<Ghosted>();
        [SerializeField] private List<Coroutine> _activeSpawnRoutines = new List<Coroutine>();

        #region Core Logic
        /// <summary>
        /// Associates a ghost with a parent enemy. If no parent is provided, 
        /// the ghost is either tracked as an orphan or destroyed based on settings.
        /// </summary>
        public void RegisterGhost(EnemyIdentifier parentRef, Ghosted ghost)
        {
            if (ghost == null) return;

            //handle orphan ghosts
            if (parentRef == null)
            {
                if (destroyOrphans)
                {
                    Destroy(ghost.gameObject);
                }
                else if (!_unparentedGhosts.Contains(ghost))
                {
                    _unparentedGhosts.Add(ghost);
                }
                return;
            }

            //handle parent registry
            if (!_registry.ContainsKey(parentRef))
            {
                _registry.Add(parentRef, new List<Ghosted>());
                parentRef.onDeath.AddListener(() => OnParentDeath(parentRef));
            }

            _trackedGhosts.Add(ghost);

            //register parent child relations with the ghost.
            if (!_registry[parentRef].Contains(ghost))
                _registry[parentRef].Add(ghost);
        }

        public void RequestSpawn(float delay, GameObject copy, Vector3 pos, Quaternion rot, EnemyIdentifier deadGhostable)
        {
            if (debugMode) Debug.Log($"<color=cyan>[GhostManager]</color> Spawn requested for {copy?.name} with delay {delay}s");
            _activeSpawnRoutines.Add(StartCoroutine(SpawnRoutine(delay, copy, pos, rot, deadGhostable)));
        }

        private IEnumerator SpawnRoutine(float delay, GameObject copy, Vector3 pos, Quaternion rot, EnemyIdentifier requester, Ghosted ghostable = null)
        {
            GhostType thisType;
            if (ghostable == null) thisType = GhostType.Parent;
            else thisType = ghostable.type;
            if (requester != null)
            {
                if (thisType == GhostType.Ghostable)
                {
                    if (delay > 0f) yield return new WaitForSeconds(delay);

                    //Check if the copy got somehow destroyed before the timer expired
                    if (copy == null) { yield break; }

                    //Validates that this is a ghost and abort if it fails
                    if (!ghostable.dontDieWithParent)
                    {
                        if (requester == null || requester.dead)
                        {
                            ghostable.isAborting = true;
                            Destroy(copy);
                            yield break;
                        }
                        #region INITIALIZING
                        //sets the copy's transform to its current position after performing the checks
                        copy.transform.SetPositionAndRotation(pos, rot);

                        //Makes sure its registered before being enabled
                        ghostable.parent = requester;
                        RegisterGhost(requester, ghostable);

                        copy.SetActive(true);
                        ghostable.Toggle(true);
                    }
                        #endregion
                }
                else
                {
                    if (thisType == GhostType.Parent)
                    {

                    }
                }
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

                if (ghost.dontDieWithParent)
                {
                    continue;
                }

                if (ghost.type == GhostType.Blueprint)
                {
                    Destroy(ghost.gameObject);
                    continue;
                }

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
        public bool CanResurrect(Ghosted blueprint)
        {
            // 1. Is it a blueprint?
            if (blueprint.type != GhostType.Blueprint) return false;

            // 2. Is the parent alive?
            if (blueprint.parent == null || blueprint.parent.dead) return false;

            // 3. Optional: Find the "original" owner and see if they are dead
            var originalEID = blueprint.original.GetComponent<EnemyIdentifier>();
            if (originalEID == originalEID.dead || originalEID == false) return false;

            return true;
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