using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solution
{

    public class OOPMapGenerator : MonoBehaviour
    {
        [Header("Set MapGenerator")]
        public int X;
        public int Y;

        [Header("Custom ASCII Map Layout")]
        public bool useCustomLayout = false;
        [TextArea(6, 20)]
        public string customMapLayout =
            "......      \n" +
            "......      \n" +
            "..#...      \n" +
            "..##########\n" +
            "..P........E\n" +
            "............";

        private bool[,] validFloorMask;
        private Vector2Int? parsedExitPos;

        [Header("Set Player")]
        public OOPPlayer player;
        public Vector2Int playerStartPos;
        [Header("Set NPC")]
        public NPC Npc;
        public NPCSkill NpcSkill;
        [Header("Set Exit")]
        public OOPExit Exit;
        [Header("Set Wall")]
        public Identity Wall;

        [Header("Set Prefab")]
        public GameObject[] floorsPrefab;
        public GameObject[] wallsPrefab;
        public GameObject[] demonWallsPrefab;
        public GameObject[] itemsPrefab;
        public GameObject[] collectItemsPrefab;
        public GameObject[] EnemyPrefab;
        public GameObject[] SkillPrefab;

        [Header("Set Transform")]
        public Transform floorParent;
        public Transform wallParent;
        public Transform itemParent;
        public Transform enemyParent;

        [Header("Set object Count")]
        public int obsatcleCount;
        public int itemPotionCount;
        public int colloctItemCount;
        public int EnemyCount;
        public int SkillCount;

        public Identity[,] mapdata;
        [Header("Enemy on Map")]
        public List<OOPEnemy> EnemysOnMap = new List<OOPEnemy>();

        // block types ...
        [HideInInspector]
        public string empty = "";
        [HideInInspector]
        public string demonWall = "demonWall";
        [HideInInspector]
        public string potion = "potion";
        [HideInInspector]
        public string bonuesPotion = "bonuesPotion";
        [HideInInspector]
        public string exit = "exit";
        [HideInInspector]
        public string playerOnMap = "player";
        [HideInInspector]
        public string collectItem = "collectItem";
        [HideInInspector]
        public string enemy = "enemy";
        [HideInInspector]
        public string npc = "Npc";

        // Start is called before the first frame update
        private void Awake()
        {
            ClearGeneratedTiles();
            CreateMap();
        }
        void Start()
        {
            StartCoroutine(SetUPMap());
        }
        IEnumerator SetUPMap() {
            SetUpPlayer();
            SetUpExit();
            SetUpNpc();
            PlaceItemsOnMap(obsatcleCount, demonWallsPrefab, wallParent, demonWall);
            //PlaceItemsOnMap(itemPotionCount, itemsPrefab, itemParent, potion);
            PlaceItemsOnMap(colloctItemCount, collectItemsPrefab, itemParent, collectItem);
            PlaceItemsOnMap(SkillCount, SkillPrefab, itemParent, collectItem);
            //PlaceItemsOnMap(EnemyCount, EnemyPrefab, enemyParent, enemy);
            yield return new WaitForSeconds(0.5f);
            RandomDamageToListEnemies();
        }

        private void PlaceItemsOnMap(int count, GameObject[] prefab, Transform parent, string itemType, System.Action onComplete = null)
        {
            if (prefab == null || prefab.Length == 0 || count <= 0)
            {
                onComplete?.Invoke();
                return;
            }

            int placedCount = 0;
            int preventInfiniteLoop = 1000; // Increased loop prevention for safety

            while (placedCount < count)
            {
                if (--preventInfiniteLoop < 0)
                {
                    Debug.LogWarning("Could not place all items. Map may be too full.");
                    break;
                }

                int x = UnityEngine.Random.Range(0, X);
                int y = UnityEngine.Random.Range(0, Y);

                if (IsWalkableFloor(x, y) && mapdata[x, y] == null)
                {
                    SetUpItem(x, y, prefab, parent, itemType);
                    placedCount++;
                }
            }
            onComplete?.Invoke();
        }
        private void SetUpNpc()
        {
            if (Npc != null)
            {
                mapdata[Npc.positionX, Npc.positionY] = Npc;
                Npc.transform.position = new Vector3(Npc.positionX, Npc.positionY, 0);
                Npc.mapGenerator = this;
            }
            if (NpcSkill != null)
            {
                mapdata[NpcSkill.positionX, NpcSkill.positionY] = NpcSkill;
                NpcSkill.transform.position = new Vector3(NpcSkill.positionX, NpcSkill.positionY, 0);
                NpcSkill.mapGenerator = this;
            }
        }
        private void SetUpExit()
        {
            if (Exit == null) return;
            Vector2Int exitPos = parsedExitPos ?? new Vector2Int(X - 1, Y - 1);
            mapdata[exitPos.x, exitPos.y] = Exit;
            Exit.transform.position = new Vector3(exitPos.x, exitPos.y, 0);
        }

        private void SetUpPlayer()
        {
            if (player == null) return;
            player.mapGenerator = this;
            player.positionX = playerStartPos.x;
            player.positionY = playerStartPos.y;
            player.GridPos = playerStartPos;
            player.transform.position = new Vector3(playerStartPos.x, playerStartPos.y, -0.1f);
            mapdata[playerStartPos.x, playerStartPos.y] = player;
        }

        [ContextMenu("Preview Map in Editor")]
        public void PreviewMapInEditor()
        {
            ClearGeneratedTiles();
            CreateMap();
        }

        [ContextMenu("Clear Preview Map")]
        public void ClearGeneratedTiles()
        {
            ClearChildren(floorParent);
            ClearChildren(wallParent);
        }

        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                GameObject child = parent.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }

        private void CreateMap()
        {
            parsedExitPos = null;

            if (useCustomLayout && !string.IsNullOrWhiteSpace(customMapLayout))
            {
                CreateCustomLayoutMap();
                return;
            }

            mapdata = new Identity[X, Y];
            validFloorMask = new bool[X, Y];

            for (int x = -1; x < X + 1; x++)
            {
                for (int y = -1; y < Y + 1; y++)
                {
                    if (x == -1 || x == X || y == -1 || y == Y)
                    {
                        SpawnWallTile(x, y);
                    }
                    else
                    {
                        SpawnFloorTile(x, y);
                        mapdata[x, y] = null;
                        validFloorMask[x, y] = true;
                    }
                }
            }
        }

        private void CreateCustomLayoutMap()
        {
            string[] rawLines = customMapLayout.Replace("\r\n", "\n").Split('\n');
            List<string> lines = new List<string>();
            int maxWidth = 0;

            for (int i = 0; i < rawLines.Length; i++)
            {
                if (i == rawLines.Length - 1 && rawLines[i].Length == 0) continue;
                lines.Add(rawLines[i]);
                if (rawLines[i].Length > maxWidth)
                {
                    maxWidth = rawLines[i].Length;
                }
            }

            X = maxWidth;
            Y = lines.Count;
            mapdata = new Identity[X, Y];
            validFloorMask = new bool[X, Y];

            bool[,] hasTileInLayout = new bool[X, Y];
            HashSet<Vector2Int> spawnedWallPositions = new HashSet<Vector2Int>();

            for (int row = 0; row < Y; row++)
            {
                int y = (Y - 1) - row; // Top line in text = highest Y in world
                string line = lines[row];

                for (int x = 0; x < X; x++)
                {
                    char c = x < line.Length ? line[x] : ' ';

                    if (c == ' ')
                    {
                        // Void / outside map
                        mapdata[x, y] = Wall;
                        validFloorMask[x, y] = false;
                        hasTileInLayout[x, y] = false;
                    }
                    else if (c == '#')
                    {
                        // Interior wall
                        hasTileInLayout[x, y] = true;
                        validFloorMask[x, y] = false;
                        SpawnFloorTile(x, y);
                        SpawnWallTile(x, y);
                        spawnedWallPositions.Add(new Vector2Int(x, y));
                        mapdata[x, y] = Wall;
                    }
                    else
                    {
                        // Walkable floor ('.', 'P', 'E', etc.)
                        hasTileInLayout[x, y] = true;
                        validFloorMask[x, y] = true;
                        SpawnFloorTile(x, y);
                        mapdata[x, y] = null;

                        if (c == 'P' || c == 'p')
                        {
                            playerStartPos = new Vector2Int(x, y);
                        }
                        else if (c == 'E' || c == 'e')
                        {
                            parsedExitPos = new Vector2Int(x, y);
                        }
                    }
                }
            }

            // Auto-Border: surround any layout tile adjacent to Void/Outside with Wall tiles
            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    if (!hasTileInLayout[x, y]) continue;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = x + dx;
                            int ny = y + dy;
                            bool isVoidOrOutside = (nx < 0 || nx >= X || ny < 0 || ny >= Y) || !hasTileInLayout[nx, ny];

                            if (isVoidOrOutside)
                            {
                                Vector2Int wallPos = new Vector2Int(nx, ny);
                                if (spawnedWallPositions.Add(wallPos))
                                {
                                    SpawnWallTile(nx, ny);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SpawnFloorTile(int x, int y)
        {
            if (floorsPrefab == null || floorsPrefab.Length == 0) return;
            int r = Random.Range(0, floorsPrefab.Length);
            GameObject obj = Instantiate(floorsPrefab[r], new Vector3(x, y, 1), Quaternion.identity);
            obj.transform.parent = floorParent;
            obj.name = "floor_" + x + ", " + y;
        }

        private void SpawnWallTile(int x, int y)
        {
            if (wallsPrefab == null || wallsPrefab.Length == 0) return;
            int r = Random.Range(0, wallsPrefab.Length);
            GameObject obj = Instantiate(wallsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = wallParent;
            obj.name = "Wall_" + x + ", " + y;
        }

        public bool IsWalkableFloor(int x, int y)
        {
            if (x < 0 || x >= X || y < 0 || y >= Y) return false;
            if (validFloorMask == null) return true;
            return validFloorMask[x, y];
        }

        public Identity GetMapData(float x, float y)
        {
            int ix = Mathf.RoundToInt(x);
            int iy = Mathf.RoundToInt(y);

            if (ix >= X || ix < 0 || iy >= Y || iy < 0) {
                return Wall;
            }

            if (validFloorMask != null && !validFloorMask[ix, iy])
            {
                return Wall;
            }

            return mapdata[ix, iy];
        }

        public void SetUpItem(int x, int y,GameObject[] _itemsPrefab,Transform parrent,string _name)
        {
            int r = Random.Range(0, _itemsPrefab.Length);
            GameObject obj = Instantiate(_itemsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = parrent;
            mapdata[x, y] = obj.GetComponent<Identity>();
            mapdata[x, y].positionX = x;
            mapdata[x, y].positionY = y;
            mapdata[x, y].mapGenerator = this;
            if (_name != collectItem) {
                mapdata[x, y].Name = _name;
            }
            if (_name == enemy) {
                EnemysOnMap.Add(obj.GetComponent<OOPEnemy>());
            }
            obj.name = $"Object_{mapdata[x, y].Name} {x}, {y}";
        }
        public void SetUpItem(int x, int y, GameObject _itemsPrefab, Transform parrent, string _name)
        {
            _itemsPrefab.transform.parent = parrent;
            mapdata[x, y] = _itemsPrefab.GetComponent<Identity>();
            mapdata[x, y].positionX = x;
            mapdata[x, y].positionY = y;
            mapdata[x, y].mapGenerator = this;
            if (_name != collectItem)
            {
                mapdata[x, y].Name = _name;
            }
            if (_name == enemy)
            {
                EnemysOnMap.Add(_itemsPrefab.GetComponent<OOPEnemy>());
            }
            _itemsPrefab.name = $"Object_{mapdata[x, y].Name} {x}, {y}";
        }

        public OOPEnemy[] GetEnemies()
        {
            return EnemysOnMap.ToArray();
        }
        public void MoveEnemies()
        {
            foreach (var enemy in EnemysOnMap)
            {
                enemy.RandomMove();
            }
        }
        public void RandomDamageToListEnemies() {
            Debug.Log($"Damage to {EnemysOnMap.Count} EnemysOnMap");
            foreach (var enemy in EnemysOnMap)
            {
                int damage = Random.Range(1, 15);
                enemy.TakeDamage(damage);
            }
        }
    }
}