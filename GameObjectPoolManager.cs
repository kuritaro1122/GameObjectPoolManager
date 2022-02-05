using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoBehaviour {
    private static GameObjectPoolManager Instance;
    [SerializeField] List<GameObjectPool> pools = new List<GameObjectPool>();

    void Awake() {
        Instance = this;
    }
    void Start() {
        foreach (var p in this.pools) {
            p.Init();
        }
    }

    public static bool CheckPool(GameObject original) {
        return Instance.CheckPool_(original);
    }
    private bool CheckPool_(GameObject original) {
        foreach (var p in this.pools) {
            if (original.GetInstanceID().Equals(p.InstanceID)) return true;
        }
        return false;
    }

    public static GameObject Get(GameObject original) {
        return Instance.GetObject(original);
    }
    public static GameObject Get(GameObject original, Transform parent) {
        GameObject obj = Get(original);
        obj.transform.parent = parent;
        return obj;
    }
    /*public static GameObject Get(GameObject original, Transform parent, bool instantiateInWorldSpace) {
        return null;
    }*/
    public static GameObject Get(GameObject original, Vector3 position, Quaternion rotation) {
        GameObject obj = Get(original);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }
    public static GameObject Get(GameObject original, Vector3 position, Quaternion rotation, Transform parent) {
        GameObject obj = Get(original, position, rotation);
        obj.transform.parent = parent;
        return obj;
    }
    /*public static void Destruction(GameObject original) {

    }*/

    private GameObject GetObject(GameObject original) {
        return GetPool(original).Get();
    }
    private GameObjectPool GetPool(GameObject original) {
        foreach (var p in this.pools) {
            if (p.InstanceID.Equals(original.GetInstanceID()))
                return p;
        }
        this.pools.Add(new GameObjectPool(original));
        return this.pools[this.pools.Count - 1];
    }

    [System.Serializable]
    private class GameObjectPool {
        [SerializeField] GameObject original;
        [SerializeField] List<GameObject> pool = new List<GameObject>();
        public int InstanceID { get { return this.original.GetInstanceID(); } }
        public GameObjectPool(GameObject original) {
            this.original = original;
        }
        private GameObject parent = null;
        public GameObject Get() {
            //if (this.pool == null || this.pool.Count < 1)
            //    this.pool = new List<GameObject>() { Create() };
            foreach (var o in this.pool) {
                //if (o == null) this.pool.Remove(o);
                if (o != null && o.activeInHierarchy == false) {
                    o.SetActive(true);
                    return o;
                }
            }
            for (int i = 0; i < this.pool.Count; i++) {
                if (this.pool[i] == null) {
                    this.pool[i] = Create();
                    return this.pool[i];
                }
            }
            this.pool.Add(Create());
            return this.pool[this.pool.Count - 1];
        }
        private GameObject Create() {
            GameObject obj = MonoBehaviour.Instantiate(this.original, parent.transform);
            obj.SetActive(false);
            return obj;
        }
        public void Init() {
            parent = new GameObject("GameObjectPool_" + this.original.name);
            if (this.original != null) {
                for (int i = 0; i < this.pool.Count; i++) {
                    if (this.pool[i] == null) this.pool[i] = Create();
                    else if (!this.pool[i].GetInstanceID().Equals(this.InstanceID)) this.pool[i] = Create();
                }
            }
        }
    }
}
