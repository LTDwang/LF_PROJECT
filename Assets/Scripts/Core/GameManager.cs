using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Time Control")]
    [Tooltip("游戏时间流速，1.0为正常速度，0.5为半速，2.0为双倍速，0.0为暂停")]
    public float gameTimeScale = 1.0f;

    [Header("Player Management")]
    [Tooltip("当前场景中的玩家实例")]
    private PlayerController currentPlayer;
    
    [Tooltip("Start 点位置缓存")]
    private Transform startPoint;

    public static GameManager Instance { get; private set; }
    
    /// <summary>
    /// 获取当前玩家实例
    /// </summary>
    public PlayerController CurrentPlayer => currentPlayer;
    
    /// <summary>
    /// 获取缩放后的 DeltaTime（用于 Update 循环）
    /// </summary>
    public static float ScaledDeltaTime => Instance != null ? Time.deltaTime * Instance.gameTimeScale : Time.deltaTime;
    
    /// <summary>
    /// 获取缩放后的 FixedDeltaTime（用于 FixedUpdate 循环）
    /// </summary>
    public static float ScaledFixedDeltaTime => Instance != null ? Time.fixedDeltaTime * Instance.gameTimeScale : Time.fixedDeltaTime;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化玩家管理
    /// </summary>
    private void Start()
    {
        FindOrCreatePlayer();
    }
    
    /// <summary>
    /// 每帧执行时间管理逻辑
    /// </summary>
    private void Update()
    {
        // 时间管理逻辑：确保 gameTimeScale 在合理范围内
        gameTimeScale = Mathf.Max(0f, gameTimeScale);
    }
    
    /// <summary>
    /// 查找名为 "Start" 的 GameObject，并缓存其 Transform
    /// </summary>
    /// <returns>找到的 Start 点 Transform，如果未找到则返回 null</returns>
    private Transform FindStartPoint()
    {
        if (startPoint != null)
        {
            return startPoint;
        }
        
        GameObject startObj = GameObject.Find("Start");
        if (startObj != null)
        {
            startPoint = startObj.transform;
            return startPoint;
        }
        else
        {
            Debug.LogWarning("GameManager: 场景中未找到名为 'Start' 的 GameObject");
            return null;
        }
    }
    
    /// <summary>
    /// 检测场景中的玩家，如果不存在则从 Start 点生成
    /// </summary>
    private void FindOrCreatePlayer()
    {
        // 检测场景中的玩家
        currentPlayer = FindObjectOfType<PlayerController>();
        
        if (currentPlayer != null)
        {
            Debug.Log("GameManager: 在场景中找到玩家实例");
            return;
        }
        
        // 如果不存在玩家，尝试从 Start 点生成
        Transform startTransform = FindStartPoint();
        if (startTransform == null)
        {
            Debug.LogError("GameManager: 无法生成玩家，未找到 Start 点");
            return;
        }
        
        // 从 Resources 加载玩家预制体
        GameObject playerPrefab = Resources.Load<GameObject>("Player/player");
        if (playerPrefab == null)
        {
            Debug.LogError("GameManager: 无法加载玩家预制体，路径: Resources/Player/player");
            return;
        }
        
        // 在 Start 点位置实例化玩家
        GameObject playerInstance = Instantiate(playerPrefab, startTransform.position, Quaternion.identity);
        currentPlayer = playerInstance.GetComponent<PlayerController>();
        
        if (currentPlayer == null)
        {
            Debug.LogError("GameManager: 玩家预制体上未找到 PlayerController 组件");
        }
        else
        {
            Debug.Log("GameManager: 成功在 Start 点生成玩家实例");
        }
    }
    
    /// <summary>
    /// 将玩家传送到 Start 点位置
    /// </summary>
    public void RespawnPlayer()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("GameManager: 无法重生玩家，当前玩家实例为空");
            FindOrCreatePlayer();
            return;
        }
        
        Transform startTransform = FindStartPoint();
        if (startTransform == null)
        {
            Debug.LogError("GameManager: 无法重生玩家，未找到 Start 点");
            return;
        }
        
        // 将玩家传送到 Start 点
        currentPlayer.transform.position = startTransform.position;
        Debug.Log("GameManager: 玩家已传送到 Start 点");
    }
    
    /// <summary>
    /// 摧毁当前玩家实例
    /// </summary>
    public void DestroyPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer.gameObject);
            currentPlayer = null;
            Debug.Log("GameManager: 玩家实例已摧毁");
        }
        
        // 摧毁后重新生成玩家
        FindOrCreatePlayer();
    }
    
    /// <summary>
    /// 重置关卡：重置玩家位置和其他游戏状态
    /// </summary>
    public void ResetLevel()
    {
        // 重置玩家位置
        RespawnPlayer();
        
        // 可以在这里扩展重置其他游戏状态
        // 例如：重置物品、敌人、关卡进度等
        
        Debug.Log("GameManager: 关卡已重置");
    }
}
